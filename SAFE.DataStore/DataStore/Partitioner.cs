using System;
using System.Collections.Generic;
using System.Text;

namespace SAFE.DataStore.Data
{
    internal static class Partitioner
    {
        private const long V = 2862933555777941757;

        /// <param name="value">The value for which we calculate the correct partition, out of the available count of partitions.</param>
        /// <param name="availableCount">Must be a number larger than 0</param>
        /// <returns>Minimum value returned is 0.</returns>
        public static int GetPartition(string value, int availableCount)
        {
            Validate(availableCount);
            ulong hash = GetKey(value);
            return JumpConsistentHash(hash, availableCount);
        }

        public static int GetPartition(Guid value, int availableCount)
        {
            Validate(availableCount);
            ulong hash = GetKey(value);
            return JumpConsistentHash(hash, availableCount);
        }

        public static int GetPartition(byte[] value, int availableCount)
        {
            Validate(availableCount);
            ulong hash = Hash(value);
            return JumpConsistentHash(hash, availableCount);
        }

        public static int GetPartition(List<byte> value, int availableCount)
        {
            Validate(availableCount);
            ulong hash = Hash(value.ToArray());
            return JumpConsistentHash(hash, availableCount);
        }

        static void Validate(int availableCount)
        {
            if (availableCount < 1)
                throw new ArgumentOutOfRangeException("Must have at least one available partition");
        }

        static ulong GetKey(string value)
        {
            return Hash(Encoding.UTF8.GetBytes(value));
        }

        static ulong GetKey(Guid guid)
        {
            return Hash(guid.ToByteArray());
        }

        static ulong Hash(byte[] bytes)
        {
            return HashFNV1a(bytes);
        }

        static int JumpConsistentHash(ulong key, int buckets)
        {
            long b = 1;
            long j = 0;

            while (j < buckets)
            {
                b = j;
                key = (key * V) + 1;

                var x = (double)(b + 1);
                var y = (double)(1L << 31);
                var z = (double)((key >> 33) + 1);

                j = (long)(x * (y / z));
            }

            return (int)b;
        }

        // FNV-1a (64-bit) non-cryptographic hash function.
        // Adapted from: http://github.com/jakedouglas/fnv-java
        static ulong HashFNV1a(byte[] bytes)
        {
            const ulong fnv64Offset = 14695981039346656037;
            const ulong fnv64Prime = 0x100000001b3;
            ulong hash = fnv64Offset;

            for (var i = 0; i < bytes.Length; i++)
            {
                hash = hash ^ bytes[i];
                hash *= fnv64Prime;
            }

            return hash;
        }
    }
}
