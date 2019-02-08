using System.Threading.Tasks;

namespace SAFE.DataStore.Client
{
    internal class InMemClient : IStorageClient
    {
        public InMemClient()
        {
            MdAccess.UseInMemoryDb();
        }

        public async Task<IDatabase> GetOrAddDataBaseAsync(string dbName)
        {
            var indexLocation = new MdLocator(System.Text.Encoding.UTF8.GetBytes($"{dbName}_indexer"), DataProtocol.DEFAULT_PROTOCOL, null, null);
            var indexMd = await MdAccess.LocateAsync(indexLocation).ConfigureAwait(false);
            if (!indexMd.HasValue)
                throw new System.Exception(indexMd.ErrorMsg);
            var indexHead = new MdHead(indexMd.Value, dbName);
            var indexer = await Indexer.GetOrAddAsync(indexHead);

            var dbLocation = new MdLocator(System.Text.Encoding.UTF8.GetBytes(dbName), DataProtocol.DEFAULT_PROTOCOL, null, null);
            var dbMd = await MdAccess.LocateAsync(dbLocation).ConfigureAwait(false);
            if (!dbMd.HasValue)
                throw new System.Exception(dbMd.ErrorMsg);
            var dbHead = new MdHead(dbMd.Value, dbName);
            var dbResult = await Database.GetOrAddAsync(dbHead, indexer);
            return dbResult.Value;
        }
    }
}