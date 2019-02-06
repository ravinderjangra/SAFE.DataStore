using System.Threading.Tasks;
using SAFE.DataStore.Client.Auth;

namespace SAFE.DataStore.Client
{
    public class ClientFactory
    {
        string _appId;
        Authentication _authentication;

        public ClientFactory(AppInfo appInfo)
        {
            _appId = appInfo.Id;
            _authentication = new Authentication(appInfo);
        }

        public IStorageClient GetInMemoryClient()
        {
            return new InMemClient();
        }

        public async Task<IStorageClient> GetMockNetworkClient(Credentials credentials)
        {
            var session = await _authentication.MockAuthenticationAsync(credentials);
            return new SAFEClient(session.Value, _appId);
        }

        public async Task<IStorageClient> GetMockNetworkClientViaBrowserAuth()
        {
            // Authentication with the SAFE browser
            await _authentication.AuthenticationWithBrowserAsync();

            // Start named pipe server and listen for message
            var authResponse = App.PipeComm.ReceiveNamedPipeServerMessage();

            // Create session from response
            var session = await _authentication.ProcessAuthenticationResponse(authResponse);
            return new SAFEClient(session, _appId);
        }

        // GetAlpha-2Client
    }
}
