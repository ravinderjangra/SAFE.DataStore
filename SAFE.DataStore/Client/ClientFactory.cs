using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using SAFE.DataStore.Client.Auth;

namespace SAFE.DataStore.Client
{
    public class ClientFactory
    {
        readonly string _appId;
        readonly Authentication _authentication;

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

            // NamedPipes ACL not working in netcore, see https://github.com/dotnet/corefx/issues/31190

            // Start websocket server and listen for message
            using (var server = new ResponseSocket("@tcp://localhost:5556"))
            {
                var authResponse = server.ReceiveFrameString();
                server.SendFrame("Auth response received.");

                // Create session from response
                var session = await _authentication.ProcessAuthenticationResponse(authResponse);
                return new SAFEClient(session, _appId);
            }
        }

        // Get Alpha-2 / Fleming Client
    }
}