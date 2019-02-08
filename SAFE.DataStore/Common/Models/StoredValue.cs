using Newtonsoft.Json;

namespace SAFE.DataStore
{
    internal class StoredValue
    {
        [JsonConstructor]
        StoredValue()
        {
        }

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