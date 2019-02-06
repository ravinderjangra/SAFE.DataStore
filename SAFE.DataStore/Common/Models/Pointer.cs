namespace SAFE.DataStore
{
    public class Pointer
    {
        public MdLocator MdLocator { get; set; } // The address of the Md this points at.

        public string MdKey { get; set; } // The key under which the value is stored in that Md.

        public string ValueType { get; set; } // The type of the value stored.
    }
}
