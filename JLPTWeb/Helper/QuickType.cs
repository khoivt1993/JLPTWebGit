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

    public partial class News
    {
        [JsonProperty("news_priority_number")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long NewsPriorityNumber { get; set; }

        [JsonProperty("news_prearranged_time")]
        public DateTimeOffset NewsPrearrangedTime { get; set; }

        [JsonProperty("news_id")]
        public string NewsId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_with_ruby")]
        public string TitleWithRuby { get; set; }

        [JsonProperty("news_file_ver")]
        public bool NewsFileVer { get; set; }

        [JsonProperty("news_creation_time")]
        public DateTimeOffset NewsCreationTime { get; set; }

        [JsonProperty("news_preview_time")]
        public DateTimeOffset NewsPreviewTime { get; set; }

        [JsonProperty("news_publication_time")]
        public DateTimeOffset NewsPublicationTime { get; set; }

        [JsonProperty("news_publication_status")]
        public bool NewsPublicationStatus { get; set; }

        [JsonProperty("has_news_web_image")]
        public bool HasNewsWebImage { get; set; }

        [JsonProperty("has_news_web_movie")]
        public bool HasNewsWebMovie { get; set; }

        [JsonProperty("has_news_easy_image")]
        public bool HasNewsEasyImage { get; set; }

        [JsonProperty("has_news_easy_movie")]
        public bool HasNewsEasyMovie { get; set; }

        [JsonProperty("has_news_easy_voice")]
        public bool HasNewsEasyVoice { get; set; }

        [JsonProperty("news_web_image_uri")]
        public string NewsWebImageUri { get; set; }

        [JsonProperty("news_web_movie_uri")]
        public string NewsWebMovieUri { get; set; }

        [JsonProperty("news_easy_image_uri")]
        public string NewsEasyImageUri { get; set; }

        [JsonProperty("news_easy_movie_uri")]
        public string NewsEasyMovieUri { get; set; }

        [JsonProperty("news_easy_voice_uri")]
        public string NewsEasyVoiceUri { get; set; }

        [JsonProperty("news_display_flag")]
        public bool NewsDisplayFlag { get; set; }

        [JsonProperty("news_web_url")]
        public Uri NewsWebUrl { get; set; }
    }
    
    public partial class News
    {
        public static List<Dictionary<string, List<News>>> FromJson(string json) => JsonConvert.DeserializeObject<List<Dictionary<string, List<News>>>>(json, Helper.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<Dictionary<string, List<News>>> self) => JsonConvert.SerializeObject(self, Helper.Converter.Settings);
    }

    internal static class Converter
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
  
    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
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
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}