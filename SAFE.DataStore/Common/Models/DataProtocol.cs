namespace SAFE.DataStore
{
    internal static class DataProtocol
    {
#pragma warning disable SA1310 // Field names should not contain underscore
        public const ulong DEFAULT_PROTOCOL = 20100;
        public const ulong MD_HEAD = 20101;
        public const ulong MD_POINTER = 20102;
        public const ulong MD_VALUE = 20103;
#pragma warning restore SA1310 // Field names should not contain underscore
    }
}
