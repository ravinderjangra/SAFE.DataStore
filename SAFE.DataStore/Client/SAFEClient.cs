using System.Threading.Tasks;

namespace SAFE.DataStore.Client
{
    internal class SAFEClient : IStorageClient
    {
        readonly SafeApp.Session _session;
        readonly string _appId;

        public SAFEClient(SafeApp.Session session, string appId)
        {
            _session = session;
            _appId = appId;
        }

        public async Task<IDatabase> GetOrAddDataBaseAsync(string dbName)
        {
            var dbResult = await DatabaseFactory.CreateForApp(_session, _appId, dbName);
            return dbResult.Value;
        }
    }
}
