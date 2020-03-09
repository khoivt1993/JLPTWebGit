﻿using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JLPTWeb.Helper
{
    // <auto-generated />
    //
    // To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
    //
    //    using Kanji;
    //
    //    var welcome = Kanji.FromJson(jsonString);



    public partial class Kanji
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("results")]
        public List<ResultKanji> Results { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public partial class ResultKanji
    {
        [JsonProperty("comp")]
        public string Comp { get; set; }

        [JsonProperty("level")]
        [JsonConverter(typeof(ParseStringConverterKanji))]
        public long Level { get; set; }

        [JsonProperty("kun")]
        public string Kun { get; set; }

        [JsonProperty("kanji")]
        public string Kanji { get; set; }

        [JsonProperty("freq")]
        [JsonConverter(typeof(ParseStringConverterKanji))]
        public long Freq { get; set; }

        [JsonProperty("stroke_count")]
        [JsonConverter(typeof(ParseStringConverterKanji))]
        public long StrokeCount { get; set; }

        [JsonProperty("examples")]
        public List<Example> Examples { get; set; }

        [JsonProperty("mobileId")]
        public long MobileId { get; set; }

        [JsonProperty("mean")]
        public string Mean { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("compDetail")]
        public object CompDetail { get; set; }

        [JsonProperty("on")]
        public string On { get; set; }
    }

    public partial class Example
    {
        [JsonProperty("p")]
        public string P { get; set; }

        [JsonProperty("w")]
        public string W { get; set; }

        [JsonProperty("m")]
        public string M { get; set; }

        [JsonProperty("h", NullValueHandling = NullValueHandling.Ignore)]
        public string H { get; set; }
    }

    public partial class Kanji
    {
        public static Kanji FromJson(string json) => JsonConvert.DeserializeObject<Kanji>(json, JLPTWeb.Helper.ConverterKanji.Settings);
    }

    public static class SerializeKanji
    {
        public static string ToJson(this Kanji self) => JsonConvert.SerializeObject(self, JLPTWeb.Helper.ConverterKanji.Settings);
    }

    internal static class ConverterKanji
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

    internal class ParseStringConverterKanji : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if ("".Equals(value))
            {
                return null;
            }
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

        public static readonly ParseStringConverterKanji Singleton = new ParseStringConverterKanji();
    }

}