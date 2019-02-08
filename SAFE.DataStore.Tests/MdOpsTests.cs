using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAFE.DataStore.Client.Auth;
using SafeApp.Utilities;

namespace SAFE.DataStore.Tests
{
    [TestClass]
    public class MdOpsTests : TestBase
    {
        [TestMethod]
        public async Task CanAddUpdateAndDeleteEntries()
        {
            try
            {
                var session = await GetAuth().MockAuthenticationAsync(Credentials.Random);
                Assert.IsTrue(session.HasValue);
                Assert.IsNotNull(session.Value);

                var networkOperations = new Network.NetworkDataOps(session.Value);
                var md = await networkOperations.CreateEmptyMd(DataProtocol.DEFAULT_PROTOCOL);

                var mdOps = new Network.MdDataOps(session.Value, md);
                await mdOps.AddObjectAsync(AuthHelpers.GetRandomString(10), AuthHelpers.GetRandomString(10));
                var entries = await mdOps.GetEntriesAsync();
                Assert.AreEqual(1, entries.Count);

                var key = AuthHelpers.GetRandomString(10);
                await mdOps.AddObjectAsync(key, AuthHelpers.GetRandomString(10));
                entries = await mdOps.GetEntriesAsync();
                Assert.AreEqual(2, entries.Count);

                var newValue = AuthHelpers.GetRandomString(10);
                await mdOps.UpdateObjectAsync(key, newValue, 0);
                entries = await mdOps.GetEntriesAsync();

                var fetchedValue = entries
                    .Where(e => e.Key.Key.ToUtfString() == key)
                    .FirstOrDefault().Value.Content
                    .ToUtfString()
                    .Parse<string>();

                Assert.AreEqual(
                    newValue,
                    fetchedValue);

                await mdOps.DeleteObjectAsync(key, 1);
                entries = await mdOps.GetEntriesAsync();
                Assert.AreEqual(1, entries.Count);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}