using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DotNetCore.CAP.Dashboard
{
    public static class JsonSerializeOptions
    {
        public static readonly JsonSerializerSettings Default = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new JsonConverter[]
            {
                new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            }
        };
    }
}