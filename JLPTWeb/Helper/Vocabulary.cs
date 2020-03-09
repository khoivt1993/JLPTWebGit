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

    public partial class Vocabulary
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("found")]
        public bool Found { get; set; }

        [JsonProperty("data")]
        public List<Datum> Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("means")]
        public List<Mean> Means { get; set; }

        [JsonProperty("images")]
        public List<object> Images { get; set; }

        [JsonProperty("_rev")]
        public string Rev { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("related_words", NullValueHandling = NullValueHandling.Ignore)]
        public RelatedWords RelatedWords { get; set; }

        [JsonProperty("phonetic")]
        public string Phonetic { get; set; }

        [JsonProperty("synsets", NullValueHandling = NullValueHandling.Ignore)]
        public List<Synset> Synsets { get; set; }

        [JsonProperty("sounds")]
        public List<object> Sounds { get; set; }

        [JsonProperty("mobileId")]
        public long MobileId { get; set; }

        [JsonProperty("jlpt", NullValueHandling = NullValueHandling.Ignore)]
        public long? Jlpt { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("pushed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Pushed { get; set; }

        [JsonProperty("opposite_word", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> OppositeWord { get; set; }
    }

    public partial class VocExample
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("mean")]
        public string Mean { get; set; }

        [JsonProperty("transcription")]
        public string Transcription { get; set; }
    }

    public partial class Mean
    {
        [JsonProperty("examples")]
        public List<VocExample> Examples { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("mean")]
        public string MeanMean { get; set; }

        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }
    }

    public partial class RelatedWords
    {
        [JsonProperty("word")]
        public List<string> Word { get; set; }
    }

    public partial class Synset
    {
        [JsonProperty("entry")]
        public List<Entry> Entry { get; set; }

        [JsonProperty("pos")]
        public string Pos { get; set; }

        [JsonProperty("base_form")]
        public string BaseForm { get; set; }
    }

    public partial class Entry
    {
        [JsonProperty("synonym")]
        public List<string> Synonym { get; set; }

        [JsonProperty("definition_id")]
        public string DefinitionId { get; set; }
    }
    

    public enum TypeEnum { Word };

    public partial class Vocabulary
    {
        public static Vocabulary FromJson(string json) => JsonConvert.DeserializeObject<Vocabulary>(json, Helper.ConverterVoc.Settings);
    }

    public static class SerializeVoc
    {
        public static string ToJson(this Vocabulary self) => JsonConvert.SerializeObject(self, Helper.ConverterVoc.Settings);
    }

    internal static class ConverterVoc
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    
    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "word")
            {
                return TypeEnum.Word;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            if (value == TypeEnum.Word)
            {
                serializer.Serialize(writer, "word");
                return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}