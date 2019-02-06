using Newtonsoft.Json;

namespace SAFE.DataStore
{
    public static class DataProtocol
    {
        #pragma warning disable SA1310 // Field names should not contain underscore
        public const ulong DEFAULT_PROTOCOL = 20100;
        public const ulong MD_HEAD = 20101;
        public const ulong MD_POINTER = 20102;
        public const ulong MD_VALUE = 20103;
        #pragma warning restore SA1310 // Field names should not contain underscore
    }

    public enum MdType
    {
        Values = 0,
        Pointers = 1
    }

    public class MdMetadata
    {
        public const int Capacity = 999; // Since 1 entry is reserved for metadata itself.
    }

    public class MdLocator
    {
        #pragma warning disable SA1502 // Element should not be on a single line
        [JsonConstructor]
        MdLocator() { }
        #pragma warning restore SA1502 // Element should not be on a single line

        public MdLocator(byte[] xorName, ulong typeTag, byte[] secEncKey, byte[] nonce)
        {
            XORName = xorName;
            TypeTag = typeTag;
            SecEncKey = secEncKey;
            Nonce = nonce;
        }

        /// <summary>
        /// The address of the Md this points at.
        /// </summary>
        public byte[] XORName { get; set; }

        /// <summary>
        /// Md type tag / protocol
        /// </summary>
        public ulong TypeTag { get; set; }

        /// <summary>
        /// Secret encryption key
        /// </summary>
        public byte[] SecEncKey { get; set; }

        public byte[] Nonce { get; set; }
    }

    public class Pointer
    {
        public MdLocator MdLocator { get; set; } // The address of the Md this points at.

        public string MdKey { get; set; } // The key under which the value is stored in that Md.

        public string ValueType { get; set; } // The type of the value stored.
    }

    public class StoredValue
    {
        #pragma warning disable SA1502 // Element should not be on a single line
        [JsonConstructor]
        StoredValue() { }
        #pragma warning restore SA1502 // Element should not be on a single line

        public StoredValue(object data)
        {
            Payload = data.Json();
            ValueType = data.GetType().Name;
        }

        public string Payload { get; set; }

        public string ValueType { get; set; }

        public T Parse<T>()
        {
            return Payload.Parse<T>();
        }
    }
}
