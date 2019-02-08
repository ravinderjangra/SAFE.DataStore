using System.Threading.Tasks;

namespace SAFE.DataStore.Factories
{
    internal class TypeStoreFactory
    {
        public const string TYPE_STORE_HEAD_KEY = "TYPE_STORE_HEAD";

        public static async Task<ITypeStore> GetOrAddTypeStoreAsync(IMdNode dbInfoMd)
        {
            IMdNode typeStoreHead;
            var typeStoreResult = await dbInfoMd.GetValueAsync(TYPE_STORE_HEAD_KEY).ConfigureAwait(false);
            if (!typeStoreResult.HasValue)
            {
                typeStoreHead = await MdAccess.CreateAsync(0)
                    .ConfigureAwait(false);
                await dbInfoMd.AddAsync(TYPE_STORE_HEAD_KEY, new StoredValue(typeStoreHead.MdLocator))
                    .ConfigureAwait(false);
            }
            else
            {
                var typeStoreHeadLocation = typeStoreResult.Value.Payload.Parse<MdLocator>();
                typeStoreHead = (await MdAccess.LocateAsync(typeStoreHeadLocation)
                    .ConfigureAwait(false)).Value;
            }

            Task OnHeadChange(MdLocator newLocation) => dbInfoMd.SetAsync(TYPE_STORE_HEAD_KEY, new StoredValue(newLocation));

            var dataTree = new DataTree(typeStoreHead, OnHeadChange);

            return new TypeStore(dataTree);
        }
    }
}