using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Station_Data_Converter.objects
{
    /// <summary>
    /// A special class to read in the coordinates of an Item
    /// The JSON switches the type of the coordinates between String[] and String[][]
    /// JSON.Net can handle this by converting both to a List<String[]>
    /// </summary>
    public class CoordinateJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string[]>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JArray.Load(reader);
            if (token[0].Type == JTokenType.Array)
            {
                return token.ToObject<List<String[]>>();
            } else
            {
                return new List<String[]> { token.ToObject<String[]>() };
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
