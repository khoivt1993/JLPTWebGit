using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JLPTWeb.Helper
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Sentence
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("results")]
        public List<Result> Results { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("transcription")]
        public string Transcription { get; set; }

        [JsonProperty("mobileId")]
        [JsonConverter(typeof(DecodingChoiceConverter))]
        public long MobileId { get; set; }

        [JsonProperty("mean")]
        public string Mean { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public partial class Sentence
    {
        public static Sentence FromJson(string json) => JsonConvert.DeserializeObject<Sentence>(json, Helper.ConverterSentence.Settings);
    }

    public static class SerializeSentence
    {
        public static string ToJson(this Sentence self) => JsonConvert.SerializeObject(self, Helper.ConverterSentence.Settings);
    }

    internal static class ConverterSentence
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class DecodingChoiceConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    var integerValue = serializer.Deserialize<long>(reader);
                    return integerValue;
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    long l;
                    if (Int64.TryParse(stringValue, out l))
                    {
                        return l;
                    }
                    break;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value);
            return;
        }

        public static readonly DecodingChoiceConverter Singleton = new DecodingChoiceConverter();
    }
}