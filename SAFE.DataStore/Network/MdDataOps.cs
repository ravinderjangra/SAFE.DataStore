using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeApp;
using SafeApp.Utilities;

namespace SAFE.DataStore.Network
{
    internal class MdDataOps
    {
        readonly MDataInfo _mdInfo;

        public Session Session { get; set; }

        public MdDataOps(Session session, MDataInfo mdInfo)
        {
            Session = session;
            _mdInfo = mdInfo;
        }

        public async Task<int> GetKeyCountAsync()
        {
            var keys = await Session.MData.ListKeysAsync(_mdInfo).ConfigureAwait(false);
            return keys.Count;
        }

        public async Task<bool> ContainsKeyAsync(string key)
        {
            var keyEntries = await Session.MData.ListKeysAsync(_mdInfo).ConfigureAwait(false);
            var keys = keyEntries.Select(c => c.Key);
            var encryptedKey = await Session.MDataInfoActions.EncryptEntryKeyAsync(_mdInfo, key.ToUtfBytes());
            return keys.Any(c => c.SequenceEqual(encryptedKey));
        }

        public async Task<IEnumerable<string>> GetKeysAsync()
        {
            var keyEntries = await Session.MData.ListKeysAsync(_mdInfo).ConfigureAwait(false);
            var keyTasks = keyEntries.Select(c => Session.MDataInfoActions.DecryptAsync(_mdInfo, c.Key));
            return (await Task.WhenAll(keyTasks)).Select(c => c.ToUtfString());
        }

        public async Task<T> GetValueAsync<T>(string key)
        {
            var ret = await GetStringValueAsync(key);
            return ret.Item1.Parse<T>();
        }

        public async Task<ulong> GetEntryVersionAsync(string key)
        {
            var keyBytes = key.ToUtfBytes();
            var encryptedKey = await Session.MDataInfoActions.EncryptEntryKeyAsync(_mdInfo, keyBytes);
            var mdRef = await Session.MData.GetValueAsync(_mdInfo, encryptedKey).ConfigureAwait(false);
            return mdRef.Item2;
        }

        public async Task<(string, ulong)> GetStringValueAsync(string key)
        {
            var keyBytes = key.ToUtfBytes();
            var encryptedKey = await Session.MDataInfoActions.EncryptEntryKeyAsync(_mdInfo, keyBytes);
            var mdRef = await Session.MData.GetValueAsync(_mdInfo, encryptedKey).ConfigureAwait(false);
            var val = await Session.MDataInfoActions.DecryptAsync(_mdInfo, mdRef.Item1);
            return (val.ToUtfString(), mdRef.Item2);
        }

        public async Task<IEnumerable<T>> GetValuesAsync<T>()
        {
            var entries = new ConcurrentBag<T>();

            using (var entriesHandle = await Session.MDataEntries.GetHandleAsync(_mdInfo))
            {
                // Fetch and decrypt entries
                var encryptedEntries = await Session.MData.ListEntriesAsync(entriesHandle);
                Parallel.ForEach(encryptedEntries, entry =>
                {
                    if (entry.Value.Content.Count != 0)
                    {
                        var decryptedValue = Session.MDataInfoActions.DecryptAsync(_mdInfo, entry.Value.Content).GetAwaiter().GetResult();
                        if (decryptedValue.ToUtfString().TryParse(out T result))
                            entries.Add(result);
                    }
                });
            }
            return entries;
        }

        public async Task<List<MDataEntry>> GetEntriesAsync()
        {
            var entries = new ConcurrentBag<MDataEntry>();

            using (var entriesHandle = await Session.MDataEntries.GetHandleAsync(_mdInfo))
            {
                // Fetch and decrypt entries
                var encryptedEntries = await Session.MData.ListEntriesAsync(entriesHandle);
                Parallel.ForEach(encryptedEntries, entry =>
                {
                    if (entry.Value.Content.Count != 0)
                    {
                        var decryptedKey = Session.MDataInfoActions.DecryptAsync(_mdInfo, entry.Key.Key).GetAwaiter().GetResult();
                        var decryptedValue = Session.MDataInfoActions.DecryptAsync(_mdInfo, entry.Value.Content).GetAwaiter().GetResult();
                        entries.Add(new MDataEntry()
                        {
                            Key = new MDataKey() { Key = decryptedKey },
                            Value = new MDataValue { Content = decryptedValue, EntryVersion = entry.Value.EntryVersion }
                        });
                    }
                });
            }
            return entries.ToList();
        }

        public async Task AddObjectAsync(string key, object value)
        {
            using (var entryActionsH = await Session.MDataEntryActions.NewAsync().ConfigureAwait(false))
            {
                // insert value
                var insertObj = new Dictionary<string, object>
                {
                    { key, value }
                };
                await InsertEntriesAsync(entryActionsH, insertObj).ConfigureAwait(false);

                // commit
                await CommitEntryMutationAsync(entryActionsH).ConfigureAwait(false);
            }
        }

        public async Task UpdateObjectAsync(string key, object value, ulong version)
        {
            using (var entryActionsH = await Session.MDataEntryActions.NewAsync().ConfigureAwait(false))
            {
                // update value
                var updateObj = new Dictionary<string, (object, ulong)>
                    {
                        { key, (value, version + 1) },
                    };
                await UpdateEntriesAsync(entryActionsH, updateObj).ConfigureAwait(false);
                await CommitEntryMutationAsync(entryActionsH).ConfigureAwait(false);
            }
        }

        public async Task DeleteObjectAsync(string key, ulong version)
        {
            using (var entryActionsH = await Session.MDataEntryActions.NewAsync().ConfigureAwait(false))
            {
                // delete
                var deleteObj = new Dictionary<string, ulong>
                {
                    { key, version + 1 }
                };
                await DeleteEntriesAsync(entryActionsH, deleteObj).ConfigureAwait(false);

                // commit
                await CommitEntryMutationAsync(entryActionsH).ConfigureAwait(false);
            }
        }

        // Populate the md entry actions handle.
        public async Task InsertEntriesAsync(NativeHandle entryActionsH, Dictionary<string, object> data)
        {
            foreach (var pair in data)
            {
                var encryptedKey = await Session.MDataInfoActions.EncryptEntryKeyAsync(_mdInfo, pair.Key.ToUtfBytes());
                var encryptedValue = await Session.MDataInfoActions.EncryptEntryValueAsync(_mdInfo, pair.Value.Json().ToUtfBytes());
                await Session.MDataEntryActions.InsertAsync(entryActionsH, encryptedKey, encryptedValue);
            }
        }

        // Populate the md entry actions handle.
        public async Task UpdateEntriesAsync(NativeHandle entryActionsH, Dictionary<string, (object, ulong)> data)
        {
            foreach (var pair in data)
            {
                var val = pair.Value.Item1;
                var version = pair.Value.Item2;

                var encryptedKey = await Session.MDataInfoActions.EncryptEntryKeyAsync(_mdInfo, pair.Key.ToUtfBytes());
                var encryptedValue = await Session.MDataInfoActions.EncryptEntryValueAsync(_mdInfo, val.Json().ToUtfBytes());

                await Session.MDataEntryActions.UpdateAsync(entryActionsH, encryptedKey, encryptedValue, version);
            }
        }

        // Populate the md entry actions handle.
        public async Task DeleteEntriesAsync(NativeHandle entryActionsH, Dictionary<string, ulong> data)
        {
            foreach (var pair in data)
            {
                var version = pair.Value;
                var encryptedKey = await Session.MDataInfoActions.EncryptEntryKeyAsync(_mdInfo, pair.Key.ToUtfBytes());
                await Session.MDataEntryActions.DeleteAsync(entryActionsH, encryptedKey, version);
            }
        }

        // Commit the operations in the md entry actions handle.
        public async Task CommitEntryMutationAsync(NativeHandle entryActionsH)
        {
            await Session.MData.MutateEntriesAsync(_mdInfo, entryActionsH); // <----------------------------------------------    Commit ------------------------
        }
    }
}
