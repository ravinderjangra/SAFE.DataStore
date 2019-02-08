using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAFE.DataStore
{
    internal class DataTree : IDataTree
    {
        readonly Func<MdLocator, Task> _onHeadAddressChange;
        IMdNode _head;
        IMdNode _currentLeaf;

        public MdLocator MdLocator => _head.MdLocator;

        public DataTree(IMdNode head, Func<MdLocator, Task> onHeadAddressChange)
        {
            _head = head;
            _onHeadAddressChange = onHeadAddressChange;
        }

        /// <summary>
        /// Adds data to a tree structure
        /// that grows in an unbalanced way.
        /// </summary>
        /// <param name="key">Key under which the value will be stored.</param>
        /// <param name="value">The value to store.</param>
        /// <returns>A pointer to the value that was added, to be used for indexing.</returns>
        public async Task<Result<Pointer>> AddAsync(string key, StoredValue value)
        {
            if (_head.IsFull)
            {
                // create new head, add pointer to current head in to it.
                // the level > 0 indicates its role as pointer holder
                var newHead = await MdAccess.CreateAsync(_head.Level + 1).ConfigureAwait(false);
                var pointer = new Pointer
                {
                    MdLocator = _head.MdLocator,
                    ValueType = typeof(Pointer).Name
                };
                await newHead.AddAsync(pointer).ConfigureAwait(false);
                _head = newHead;
                await _onHeadAddressChange(newHead.MdLocator).ConfigureAwait(false);
            }

            return await DirectlyAddToLeaf(key, value).ConfigureAwait(false);
        }

        /// <summary>
        /// Instead of traversing through the tree on every add,
        /// we keep a reference to current leaf, and add to it directly.
        /// </summary>
        /// <param name="key">Key under which the value will be stored.</param>
        /// <param name="value">The value to store.</param>
        /// <returns>A pointer to the value that was added.</returns>
        async Task<Result<Pointer>> DirectlyAddToLeaf(string key, StoredValue value)
        {
            if (_currentLeaf == null)
            {
                _currentLeaf = _head;
            }
            else if (_currentLeaf.IsFull)
            {
                var result = await _head.AddAsync(key, value).ConfigureAwait(false);
                var leafResult = await MdAccess.LocateAsync(result.Value.MdLocator);
                if (leafResult.HasValue)
                    _currentLeaf = leafResult.Value;
                return result;
            }

            return await _currentLeaf.AddAsync(key, value);
        }

        public Task<IEnumerable<StoredValue>> GetAllValuesAsync()
        {
            return _head.GetAllValuesAsync();
        }

        public Task<IEnumerable<(Pointer, StoredValue)>> GetAllPointerValuesAsync()
        {
            return _head.GetAllPointerValuesAsync();
        }
    }
}