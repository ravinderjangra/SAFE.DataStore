using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAFE.DataStore
{
    public interface IDatabase
    {
        Task<Result<Pointer>> AddAsync(string key, object data);

        Task<Result<Pointer>> AddAsync<T>(string key, T data);

        Task CreateIndex<T>(string[] propertyPath);

        Task<Result<Pointer>> Delete<T>(string key);

        Task<(IEnumerable<T> data, IEnumerable<string> errors)> FindAsync<T>(string whereProperty, object isValue);

        Task<Result<T>> FindByKeyAsync<T>(string key);

        Task<IEnumerable<T>> GetAllAsync<T>();

        Task<Result<T>> Update<T>(string key, T newValue);
    }
}
