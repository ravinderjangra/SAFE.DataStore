using System.Threading.Tasks;
using SAFE.DataStore.Client;
using SAFE.DataStore.Client.Auth;

namespace SAFE.DataStore.Tests
{
    public class TestBase
    {
        readonly string _appId = "testapp";
        IStorageClient _client;

        protected async Task InitClient(bool inMem = false)
        {
            var clientFactory = new ClientFactory(GetAppInfo());

            if (inMem)
                _client = clientFactory.GetInMemoryClient();
            else
                _client = await clientFactory.GetMockNetworkClient(Credentials.Random);
        }

        AppInfo GetAppInfo()
        {
            return new AppInfo
            {
                Id = _appId,
                Name = "testapp",
                Scope = string.Empty,
                Vendor = "test"
            };
        }

        protected Authentication GetAuth()
        {
            return new Authentication(GetAppInfo());
        }

        protected Task<IDatabase> GetDatabase(string dbName)
        {
            return _client.GetOrAddDataBaseAsync(dbName);
        }
    }
}
