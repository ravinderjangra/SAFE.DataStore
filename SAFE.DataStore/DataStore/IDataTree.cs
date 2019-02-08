using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAFE.DataStore
{
    internal interface IDataTree
    {
        MdLocator MdLocator { get; }

        Task<Result<Pointer>> AddAsync(string key, StoredValue value);

        Task<IEnumerable<(Pointer, StoredValue)>> GetAllPointerValuesAsync();

        Task<IEnumerable<StoredValue>> GetAllValuesAsync();
    }
}