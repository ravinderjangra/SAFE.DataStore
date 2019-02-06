using System.Threading.Tasks;

namespace SAFE.DataStore.Client
{
    public interface IStorageClient
    {
        Task<IDatabase> GetOrAddDataBaseAsync(string dbName);
    }
}
