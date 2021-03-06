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
    //    using KanjiFromHira;
    //
    //    var welcome = Welcome.FromJson(jsonString);
    
        public partial class KanjiFromHira
        {
            [JsonProperty("status")]
            public long Status { get; set; }

            [JsonProperty("results")]
            public List<Result> Results { get; set; }

            [JsonProperty("total")]
            public long Total { get; set; }
        }

        public partial class ResultKanjiFromHira
    {
            [JsonProperty("comp")]
            public string Comp { get; set; }

            [JsonProperty("level")]
            [JsonConverter(typeof(ParseStringConverter))]
            public long? Level { get; set; }

            [JsonProperty("kun")]
            public string Kun { get; set; }

            [JsonProperty("kanji")]
            public string Kanji { get; set; }

            [JsonProperty("freq")]
            [JsonConverter(typeof(ParseStringConverter))]
            public long? Freq { get; set; }

            [JsonProperty("stroke_count")]
            [JsonConverter(typeof(ParseStringConverter))]
            public long StrokeCount { get; set; }

            [JsonProperty("example_on", NullValueHandling = NullValueHandling.Ignore)]
            public ExampleOn ExampleOn { get; set; }

            [JsonProperty("examples")]
            public List<ExampleKun> Examples { get; set; }

            [JsonProperty("mobileId")]
            public long MobileId { get; set; }

            [JsonProperty("mean")]
            public string Mean { get; set; }

            [JsonProperty("detail")]
            public string Detail { get; set; }

            [JsonProperty("compDetail")]
            public List<CompDetail> CompDetail { get; set; }

            [JsonProperty("on")]
            public string On { get; set; }

            [JsonProperty("example_kun", NullValueHandling = NullValueHandling.Ignore)]
            public Dictionary<string, List<ExampleKun>> ExampleKun { get; set; }
        }

        public partial class CompDetail
        {
            [JsonProperty("w")]
            public string W { get; set; }

            [JsonProperty("h")]
            public string H { get; set; }
        }

        public partial class ExampleKun
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

        public partial class ExampleOn
        {
            [JsonProperty("セン", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> セン { get; set; }

            [JsonProperty("ホウ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> ホウ { get; set; }

            [JsonProperty("ナ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> ナ { get; set; }

            [JsonProperty("カン", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> カン { get; set; }

            [JsonProperty("カイ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> カイ { get; set; }

            [JsonProperty("ケイ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> ケイ { get; set; }

            [JsonProperty("セキ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> セキ { get; set; }

            [JsonProperty("ウ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> ウ { get; set; }

            [JsonProperty("オ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> オ { get; set; }

            [JsonProperty("キョ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> キョ { get; set; }

            [JsonProperty("オウ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> オウ { get; set; }

            [JsonProperty("カ", NullValueHandling = NullValueHandling.Ignore)]
            public List<ExampleKun> カ { get; set; }
        }

        public partial class KanjiFromHira
        {
            public static KanjiFromHira FromJson(string json) => JsonConvert.DeserializeObject<KanjiFromHira>(json, JLPTWeb.Helper.ConverterKanjiFromHira.Settings);
        }

        public static class SerializeKanjiFromHira
    {
            public static string ToJson(this KanjiFromHira self) => JsonConvert.SerializeObject(self, JLPTWeb.Helper.ConverterKanjiFromHira.Settings);
        }

        internal static class ConverterKanjiFromHira
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

        internal class ParseStringConverterKanjiFromHira : JsonConverter
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