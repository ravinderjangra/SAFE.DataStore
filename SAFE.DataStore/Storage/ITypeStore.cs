using System.Threading.Tasks;

namespace SAFE.DataStore
{
    public interface ITypeStore
    {
        Task AddAsync(string type, MdLocator location);

        Task<System.Collections.Generic.IEnumerable<(string, MdLocator)>> GetAllAsync();

        Task<Result<Pointer>> UpdateAsync(string type, MdLocator location);
    }
}
