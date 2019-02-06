using System;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SAFE.DataStore
{
    internal static class Serializer
    {
        static JsonSerializerSettings _serializerSettings;

        static Serializer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                Culture = new System.Globalization.CultureInfo(string.Empty)
                {
                    NumberFormat = new System.Globalization.NumberFormatInfo
                    {
                        CurrencyDecimalDigits = 31
                    }
                },
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ContractResolver = new PrivateMemberContractResolver()
            };
            _serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            JsonConvert.DefaultSettings = () => _serializerSettings;
        }

        public static string Json(this object some)
        {
            var data = JsonConvert.SerializeObject(some, _serializerSettings);
            return data;
        }

        public static byte[] AsBytes(this object some)
        {
            return Encoding.UTF8.GetBytes(some.Json());
        }

        public static string GetJson(this byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static T Parse<T>(this string json)
        {
            JsonConvert.DefaultSettings = () => _serializerSettings;
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object ParseJson(this string json, string assemblyQualifiedName)
        {
            var type = Type.GetType(assemblyQualifiedName);
            var obj = JsonConvert.DeserializeObject(json, type);
            return obj;
        }

        public static object Parse(this string json)
        {
            JsonConvert.DefaultSettings = () => _serializerSettings;
            var obj = JsonConvert.DeserializeObject(json);
            return obj;
        }

        public static bool TryParse<T>(this string json, out T result)
        {
            try
            {
                result = json.Parse<T>();
                return true;
            }
            catch
            {
                // _logger.Error(ex, "Error in {0} when handling msg.", ..);
                result = default(T);
                return false;
            }
        }

        public static T Parse<T>(this byte[] data)
        {
            return Parse<T>(Encoding.UTF8.GetString(data));
        }

        public static object Parse(this byte[] data)
        {
            return Parse(Encoding.UTF8.GetString(data));
        }

        public static bool TryParse<T>(byte[] data, out T result)
        {
            try
            {
                result = Parse<T>(Encoding.UTF8.GetString(data));
                return true;
            }
            catch
            {
                // _logger.Error(ex, "Error in {0} when handling msg.", ..);
                result = default(T);
                return false;
            }
        }

        public static bool TryParse(byte[] data, out object result)
        {
            try
            {
                result = Parse(Encoding.UTF8.GetString(data));
                return true;
            }
            catch
            {
                // _logger.Error(ex, "Error in {0} when handling msg.", ..);
                result = null;
                return false;
            }
        }

        public static T ParseJObject<T>(this object obj)
        {
            return Newtonsoft.Json.Linq.JObject.FromObject(obj).ToObject<T>();
        }
    }

    // http://stackoverflow.com/questions/4066947/private-setters-in-json-net
    internal class PrivateMemberContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            // TODO: Maybe cache
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
}
