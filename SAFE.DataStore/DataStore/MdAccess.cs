using System;
using System.Threading.Tasks;

namespace SAFE.DataStore
{
    internal class MdAccess
    {
        static Func<MdLocator, Task<Result<IMdNode>>> _locator;
        static Func<int, Task<IMdNode>> _creator;

        public static void SetLocator(Func<MdLocator, Task<Result<IMdNode>>> locator)
        {
            _locator = locator;
        }

        public static void SetCreator(Func<int, Task<IMdNode>> creator)
        {
            _creator = creator;
        }

        public static Task<Result<IMdNode>> LocateAsync(MdLocator location)
        {
            return _locator(location);
        }

        public static Task<IMdNode> CreateAsync(int level)
        {
            return _creator(level);
        }

        public static void UseInMemoryDb()
        {
            SetCreator(level => Task.FromResult(InMemoryMd.Create(level)));
            SetLocator(location => Task.FromResult(Result.OK(InMemoryMd.Locate(location))));
        }
    }
}
