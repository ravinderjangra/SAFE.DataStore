using System.Threading.Tasks;
using SAFE.DataStore.Network;

namespace SAFE.DataStore.Client
{
    internal class DatabaseFactory
    {
        public static async Task<Result<IDatabase>> CreateForApp(SafeApp.Session session, string appId, string databaseId)
        {
            var manager = new MdHeadManager(session, appId, DataProtocol.DEFAULT_PROTOCOL);
            await manager.InitializeManager();

            MdAccess.SetCreator((level) => manager.CreateNewMdNode(level, DataProtocol.DEFAULT_PROTOCOL));
            MdAccess.SetLocator(manager.LocateMdNode);

            var indexerDbId = $"{databaseId}_indexer";
            var indexerMdHead = await manager.GetOrAddHeadAsync(indexerDbId);
            var indexer = await Indexer.GetOrAddAsync(indexerMdHead);

            var databaseMdHead = await manager.GetOrAddHeadAsync(databaseId);
            var dbResult = await Database.GetOrAddAsync(databaseMdHead, indexer);
            return dbResult;
        }
    }
}