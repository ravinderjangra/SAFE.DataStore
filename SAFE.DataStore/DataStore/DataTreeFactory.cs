using System;
using System.Threading.Tasks;

namespace SAFE.DataStore.Factories
{
    internal class DataTreeFactory
    {
        public static async Task<DataTree> CreateAsync(Func<MdLocator, Task> onHeadAddressChange)
        {
            var head = await MdAccess.CreateAsync(level: 0);
            var dataTree = new DataTree(head, onHeadAddressChange);
            return dataTree;
        }
    }
}