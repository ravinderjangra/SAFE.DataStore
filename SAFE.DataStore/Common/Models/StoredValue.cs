using Newtonsoft.Json;

namespace SAFE.DataStore
{
    internal class StoredValue
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
