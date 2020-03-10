using System;
using System.Data;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace L1.Shared.Helpers
{
    public class JsonTypeHandler : SqlMapper.ITypeHandler
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterContractResolver()
        };

        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public object Parse(Type destinationType, object value)
        {
            var data = JsonConvert.DeserializeObject(value as string, destinationType, settings);

            return data;
        }
    }
}