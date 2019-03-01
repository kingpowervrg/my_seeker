#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace Utf8Json.Resolvers
{
    using System;
    using Utf8Json;

    public class GeneratedResolver : global::Utf8Json.IJsonFormatterResolver
    {
        public static readonly global::Utf8Json.IJsonFormatterResolver Instance = new GeneratedResolver();

        GeneratedResolver()
        {

        }

        public global::Utf8Json.IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::Utf8Json.IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::Utf8Json.IJsonFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(36)
            {
                {typeof(global::System.Collections.Generic.List<string>), 0 },
                {typeof(global::System.Collections.Generic.List<global::CartoonVideoNamesJson>), 1 },
                {typeof(global::System.Collections.Generic.List<global::CartoonItemJson>), 2 },
                {typeof(global::System.Collections.Generic.List<global::SeekerGame.DropOutJsonData>), 3 },
                {typeof(global::System.Collections.Generic.List<global::JigsawChipJson>), 4 },
                {typeof(global::System.Collections.Generic.List<global::JigsawDataJson>), 5 },
                {typeof(global::System.Collections.Generic.Dictionary<string, global::JigsawChipJson>), 6 },
                {typeof(global::System.Collections.Generic.List<global::PersuadeItemData>), 7 },
                {typeof(global::System.Collections.Generic.List<global::PersuadeData>), 8 },
                {typeof(global::System.Collections.Generic.List<long>), 9 },
                {typeof(global::System.Collections.Generic.List<global::PropUseGuidData>), 10 },
                {typeof(global::System.Collections.Generic.List<global::RankData>), 11 },
                {typeof(global::System.Collections.Generic.List<global::ScanAnchorJsonData>), 12 },
                {typeof(global::System.Collections.Generic.List<global::TaskOnBuild>), 13 },
                {typeof(global::System.Collections.Generic.List<global::SeekerGame.TitleBenifitData>), 14 },
                {typeof(global::SeekerGame.AppsflyerData), 15 },
                {typeof(global::SeekerGame.AppsflyerData4Android), 16 },
                {typeof(global::CartoonVideoNamesJson), 17 },
                {typeof(global::CartoonItemJson), 18 },
                {typeof(global::CartoonData), 19 },
                {typeof(global::SeekerGame.DropOutJsonData), 20 },
                {typeof(global::SeekerGame.FeedbackDetailData), 21 },
                {typeof(global::RectJson), 22 },
                {typeof(global::JigsawChipJson), 23 },
                {typeof(global::JigsawDataJson), 24 },
                {typeof(global::JigsawData), 25 },
                {typeof(global::JigsawGameData), 26 },
                {typeof(global::PersuadeItemData), 27 },
                {typeof(global::PersuadeData), 28 },
                {typeof(global::PersuadeGroupData), 29 },
                {typeof(global::PropUseGuidData), 30 },
                {typeof(global::RankData), 31 },
                {typeof(global::ScanAnchorJsonData), 32 },
                {typeof(global::ScanJsonData), 33 },
                {typeof(global::TaskOnBuild), 34 },
                {typeof(global::SeekerGame.TitleBenifitData), 35 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::Utf8Json.Formatters.ListFormatter<string>();
                case 1: return new global::Utf8Json.Formatters.ListFormatter<global::CartoonVideoNamesJson>();
                case 2: return new global::Utf8Json.Formatters.ListFormatter<global::CartoonItemJson>();
                case 3: return new global::Utf8Json.Formatters.ListFormatter<global::SeekerGame.DropOutJsonData>();
                case 4: return new global::Utf8Json.Formatters.ListFormatter<global::JigsawChipJson>();
                case 5: return new global::Utf8Json.Formatters.ListFormatter<global::JigsawDataJson>();
                case 6: return new global::Utf8Json.Formatters.DictionaryFormatter<string, global::JigsawChipJson>();
                case 7: return new global::Utf8Json.Formatters.ListFormatter<global::PersuadeItemData>();
                case 8: return new global::Utf8Json.Formatters.ListFormatter<global::PersuadeData>();
                case 9: return new global::Utf8Json.Formatters.ListFormatter<long>();
                case 10: return new global::Utf8Json.Formatters.ListFormatter<global::PropUseGuidData>();
                case 11: return new global::Utf8Json.Formatters.ListFormatter<global::RankData>();
                case 12: return new global::Utf8Json.Formatters.ListFormatter<global::ScanAnchorJsonData>();
                case 13: return new global::Utf8Json.Formatters.ListFormatter<global::TaskOnBuild>();
                case 14: return new global::Utf8Json.Formatters.ListFormatter<global::SeekerGame.TitleBenifitData>();
                case 15: return new Utf8Json.Formatters.SeekerGame.AppsflyerDataFormatter();
                case 16: return new Utf8Json.Formatters.SeekerGame.AppsflyerData4AndroidFormatter();
                case 17: return new Utf8Json.Formatters.CartoonVideoNamesJsonFormatter();
                case 18: return new Utf8Json.Formatters.CartoonItemJsonFormatter();
                case 19: return new Utf8Json.Formatters.CartoonDataFormatter();
                case 20: return new Utf8Json.Formatters.SeekerGame.DropOutJsonDataFormatter();
                case 21: return new Utf8Json.Formatters.SeekerGame.FeedbackDetailDataFormatter();
                case 22: return new Utf8Json.Formatters.RectJsonFormatter();
                case 23: return new Utf8Json.Formatters.JigsawChipJsonFormatter();
                case 24: return new Utf8Json.Formatters.JigsawDataJsonFormatter();
                case 25: return new Utf8Json.Formatters.JigsawDataFormatter();
                case 26: return new Utf8Json.Formatters.JigsawGameDataFormatter();
                case 27: return new Utf8Json.Formatters.PersuadeItemDataFormatter();
                case 28: return new Utf8Json.Formatters.PersuadeDataFormatter();
                case 29: return new Utf8Json.Formatters.PersuadeGroupDataFormatter();
                case 30: return new Utf8Json.Formatters.PropUseGuidDataFormatter();
                case 31: return new Utf8Json.Formatters.RankDataFormatter();
                case 32: return new Utf8Json.Formatters.ScanAnchorJsonDataFormatter();
                case 33: return new Utf8Json.Formatters.ScanJsonDataFormatter();
                case 34: return new Utf8Json.Formatters.TaskOnBuildFormatter();
                case 35: return new Utf8Json.Formatters.SeekerGame.TitleBenifitDataFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning disable 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace Utf8Json.Formatters.SeekerGame
{
    using System;
    using Utf8Json;


    public sealed class AppsflyerDataFormatter : global::Utf8Json.IJsonFormatter<global::SeekerGame.AppsflyerData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AppsflyerDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("af_status"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("is_first_launch"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("campaign"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("media_source"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("af_message"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("af_status"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("is_first_launch"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("campaign"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("media_source"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("af_message"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SeekerGame.AppsflyerData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.af_status);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteBoolean(value.is_first_launch);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.campaign);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.media_source);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.af_message);
            
            writer.WriteEndObject();
        }

        public global::SeekerGame.AppsflyerData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __af_status__ = default(string);
            var __af_status__b__ = false;
            var __is_first_launch__ = default(bool);
            var __is_first_launch__b__ = false;
            var __campaign__ = default(string);
            var __campaign__b__ = false;
            var __media_source__ = default(string);
            var __media_source__b__ = false;
            var __af_message__ = default(string);
            var __af_message__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __af_status__ = reader.ReadString();
                        __af_status__b__ = true;
                        break;
                    case 1:
                        __is_first_launch__ = reader.ReadBoolean();
                        __is_first_launch__b__ = true;
                        break;
                    case 2:
                        __campaign__ = reader.ReadString();
                        __campaign__b__ = true;
                        break;
                    case 3:
                        __media_source__ = reader.ReadString();
                        __media_source__b__ = true;
                        break;
                    case 4:
                        __af_message__ = reader.ReadString();
                        __af_message__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SeekerGame.AppsflyerData();
            if(__af_status__b__) ____result.af_status = __af_status__;
            if(__is_first_launch__b__) ____result.is_first_launch = __is_first_launch__;
            if(__campaign__b__) ____result.campaign = __campaign__;
            if(__media_source__b__) ____result.media_source = __media_source__;
            if(__af_message__b__) ____result.af_message = __af_message__;

            return ____result;
        }
    }


    public sealed class AppsflyerData4AndroidFormatter : global::Utf8Json.IJsonFormatter<global::SeekerGame.AppsflyerData4Android>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public AppsflyerData4AndroidFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("af_status"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("is_first_launch"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("campaign"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("media_source"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("af_message"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("af_status"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("is_first_launch"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("campaign"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("media_source"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("af_message"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SeekerGame.AppsflyerData4Android value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.af_status);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.is_first_launch);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.campaign);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.media_source);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteString(value.af_message);
            
            writer.WriteEndObject();
        }

        public global::SeekerGame.AppsflyerData4Android Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __af_status__ = default(string);
            var __af_status__b__ = false;
            var __is_first_launch__ = default(string);
            var __is_first_launch__b__ = false;
            var __campaign__ = default(string);
            var __campaign__b__ = false;
            var __media_source__ = default(string);
            var __media_source__b__ = false;
            var __af_message__ = default(string);
            var __af_message__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __af_status__ = reader.ReadString();
                        __af_status__b__ = true;
                        break;
                    case 1:
                        __is_first_launch__ = reader.ReadString();
                        __is_first_launch__b__ = true;
                        break;
                    case 2:
                        __campaign__ = reader.ReadString();
                        __campaign__b__ = true;
                        break;
                    case 3:
                        __media_source__ = reader.ReadString();
                        __media_source__b__ = true;
                        break;
                    case 4:
                        __af_message__ = reader.ReadString();
                        __af_message__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SeekerGame.AppsflyerData4Android();
            if(__af_status__b__) ____result.af_status = __af_status__;
            if(__is_first_launch__b__) ____result.is_first_launch = __is_first_launch__;
            if(__campaign__b__) ____result.campaign = __campaign__;
            if(__media_source__b__) ____result.media_source = __media_source__;
            if(__af_message__b__) ____result.af_message = __af_message__;

            return ____result;
        }
    }


    public sealed class DropOutJsonDataFormatter : global::Utf8Json.IJsonFormatter<global::SeekerGame.DropOutJsonData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public DropOutJsonDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("count"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("rate"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Datas"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("count"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("rate"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Datas"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SeekerGame.DropOutJsonData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.count);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.rate);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt64(value.value);
            writer.WriteRaw(this.____stringByteKeys[3]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SeekerGame.DropOutJsonData>>().Serialize(ref writer, value.Datas, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SeekerGame.DropOutJsonData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __count__ = default(int);
            var __count__b__ = false;
            var __rate__ = default(int);
            var __rate__b__ = false;
            var __value__ = default(long);
            var __value__b__ = false;
            var __Datas__ = default(global::System.Collections.Generic.List<global::SeekerGame.DropOutJsonData>);
            var __Datas__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __count__ = reader.ReadInt32();
                        __count__b__ = true;
                        break;
                    case 1:
                        __rate__ = reader.ReadInt32();
                        __rate__b__ = true;
                        break;
                    case 2:
                        __value__ = reader.ReadInt64();
                        __value__b__ = true;
                        break;
                    case 3:
                        __Datas__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SeekerGame.DropOutJsonData>>().Deserialize(ref reader, formatterResolver);
                        __Datas__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SeekerGame.DropOutJsonData();
            if(__count__b__) ____result.count = __count__;
            if(__rate__b__) ____result.rate = __rate__;
            if(__value__b__) ____result.value = __value__;
            if(__Datas__b__) ____result.Datas = __Datas__;

            return ____result;
        }
    }


    public sealed class FeedbackDetailDataFormatter : global::Utf8Json.IJsonFormatter<global::SeekerGame.FeedbackDetailData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public FeedbackDetailDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("GuestID"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("FacebookID"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("PlayerID"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("GuestID"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("FacebookID"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("PlayerID"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SeekerGame.FeedbackDetailData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.GuestID);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.FacebookID);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.PlayerID);
            
            writer.WriteEndObject();
        }

        public global::SeekerGame.FeedbackDetailData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __GuestID__ = default(string);
            var __GuestID__b__ = false;
            var __FacebookID__ = default(string);
            var __FacebookID__b__ = false;
            var __PlayerID__ = default(string);
            var __PlayerID__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __GuestID__ = reader.ReadString();
                        __GuestID__b__ = true;
                        break;
                    case 1:
                        __FacebookID__ = reader.ReadString();
                        __FacebookID__b__ = true;
                        break;
                    case 2:
                        __PlayerID__ = reader.ReadString();
                        __PlayerID__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SeekerGame.FeedbackDetailData();
            if(__GuestID__b__) ____result.GuestID = __GuestID__;
            if(__FacebookID__b__) ____result.FacebookID = __FacebookID__;
            if(__PlayerID__b__) ____result.PlayerID = __PlayerID__;

            return ____result;
        }
    }


    public sealed class TitleBenifitDataFormatter : global::Utf8Json.IJsonFormatter<global::SeekerGame.TitleBenifitData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TitleBenifitDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("TitleBenifitDataList"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("TitleBenifitDataList"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::SeekerGame.TitleBenifitData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.type);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.value);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SeekerGame.TitleBenifitData>>().Serialize(ref writer, value.TitleBenifitDataList, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::SeekerGame.TitleBenifitData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __type__ = default(string);
            var __type__b__ = false;
            var __value__ = default(string);
            var __value__b__ = false;
            var __TitleBenifitDataList__ = default(global::System.Collections.Generic.List<global::SeekerGame.TitleBenifitData>);
            var __TitleBenifitDataList__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __type__ = reader.ReadString();
                        __type__b__ = true;
                        break;
                    case 1:
                        __value__ = reader.ReadString();
                        __value__b__ = true;
                        break;
                    case 2:
                        __TitleBenifitDataList__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::SeekerGame.TitleBenifitData>>().Deserialize(ref reader, formatterResolver);
                        __TitleBenifitDataList__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::SeekerGame.TitleBenifitData();
            if(__type__b__) ____result.type = __type__;
            if(__value__b__) ____result.value = __value__;
            if(__TitleBenifitDataList__b__) ____result.TitleBenifitDataList = __TitleBenifitDataList__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace Utf8Json.Formatters
{
    using System;
    using Utf8Json;


    public sealed class CartoonVideoNamesJsonFormatter : global::Utf8Json.IJsonFormatter<global::CartoonVideoNamesJson>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CartoonVideoNamesJsonFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_names"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_names"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::CartoonVideoNamesJson value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Serialize(ref writer, value.M_names, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::CartoonVideoNamesJson Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_names__ = default(global::System.Collections.Generic.List<string>);
            var __M_names__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_names__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<string>>().Deserialize(ref reader, formatterResolver);
                        __M_names__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::CartoonVideoNamesJson();
            if(__M_names__b__) ____result.M_names = __M_names__;

            return ____result;
        }
    }


    public sealed class CartoonItemJsonFormatter : global::Utf8Json.IJsonFormatter<global::CartoonItemJson>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CartoonItemJsonFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Item_id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_cartoons"), 1},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("Item_id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_cartoons"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::CartoonItemJson value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt64(value.Item_id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::CartoonVideoNamesJson>>().Serialize(ref writer, value.M_cartoons, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::CartoonItemJson Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __Item_id__ = default(long);
            var __Item_id__b__ = false;
            var __M_cartoons__ = default(global::System.Collections.Generic.List<global::CartoonVideoNamesJson>);
            var __M_cartoons__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __Item_id__ = reader.ReadInt64();
                        __Item_id__b__ = true;
                        break;
                    case 1:
                        __M_cartoons__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::CartoonVideoNamesJson>>().Deserialize(ref reader, formatterResolver);
                        __M_cartoons__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::CartoonItemJson();
            if(__Item_id__b__) ____result.Item_id = __Item_id__;
            if(__M_cartoons__b__) ____result.M_cartoons = __M_cartoons__;

            return ____result;
        }
    }


    public sealed class CartoonDataFormatter : global::Utf8Json.IJsonFormatter<global::CartoonData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public CartoonDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_cartoons"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_cartoons"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::CartoonData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::CartoonItemJson>>().Serialize(ref writer, value.M_cartoons, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::CartoonData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_cartoons__ = default(global::System.Collections.Generic.List<global::CartoonItemJson>);
            var __M_cartoons__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_cartoons__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::CartoonItemJson>>().Deserialize(ref reader, formatterResolver);
                        __M_cartoons__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::CartoonData();
            if(__M_cartoons__b__) ____result.M_cartoons = __M_cartoons__;

            return ____result;
        }
    }


    public sealed class RectJsonFormatter : global::Utf8Json.IJsonFormatter<global::RectJson>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RectJsonFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_x"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_y"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_w"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_h"), 3},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_x"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_y"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_w"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_h"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::RectJson value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteSingle(value.M_x);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.M_y);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.M_w);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.M_h);
            
            writer.WriteEndObject();
        }

        public global::RectJson Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_x__ = default(float);
            var __M_x__b__ = false;
            var __M_y__ = default(float);
            var __M_y__b__ = false;
            var __M_w__ = default(float);
            var __M_w__b__ = false;
            var __M_h__ = default(float);
            var __M_h__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_x__ = reader.ReadSingle();
                        __M_x__b__ = true;
                        break;
                    case 1:
                        __M_y__ = reader.ReadSingle();
                        __M_y__b__ = true;
                        break;
                    case 2:
                        __M_w__ = reader.ReadSingle();
                        __M_w__b__ = true;
                        break;
                    case 3:
                        __M_h__ = reader.ReadSingle();
                        __M_h__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::RectJson();
            if(__M_x__b__) ____result.M_x = __M_x__;
            if(__M_y__b__) ____result.M_y = __M_y__;
            if(__M_w__b__) ____result.M_w = __M_w__;
            if(__M_h__b__) ____result.M_h = __M_h__;

            return ____result;
        }
    }


    public sealed class JigsawChipJsonFormatter : global::Utf8Json.IJsonFormatter<global::JigsawChipJson>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public JigsawChipJsonFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_chip_name"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_tex_anme"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_tex_size"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_chip_name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_tex_anme"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_tex_size"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::JigsawChipJson value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.M_chip_name);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.M_tex_anme);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::RectJson>().Serialize(ref writer, value.M_tex_size, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::JigsawChipJson Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_chip_name__ = default(string);
            var __M_chip_name__b__ = false;
            var __M_tex_anme__ = default(string);
            var __M_tex_anme__b__ = false;
            var __M_tex_size__ = default(global::RectJson);
            var __M_tex_size__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_chip_name__ = reader.ReadString();
                        __M_chip_name__b__ = true;
                        break;
                    case 1:
                        __M_tex_anme__ = reader.ReadString();
                        __M_tex_anme__b__ = true;
                        break;
                    case 2:
                        __M_tex_size__ = formatterResolver.GetFormatterWithVerify<global::RectJson>().Deserialize(ref reader, formatterResolver);
                        __M_tex_size__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::JigsawChipJson();
            if(__M_chip_name__b__) ____result.M_chip_name = __M_chip_name__;
            if(__M_tex_anme__b__) ____result.M_tex_anme = __M_tex_anme__;
            if(__M_tex_size__b__) ____result.M_tex_size = __M_tex_size__;

            return ____result;
        }
    }


    public sealed class JigsawDataJsonFormatter : global::Utf8Json.IJsonFormatter<global::JigsawDataJson>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public JigsawDataJsonFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_template_id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_dimention"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_chips"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_template_id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_dimention"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_chips"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::JigsawDataJson value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.M_template_id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.M_dimention);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::JigsawChipJson>>().Serialize(ref writer, value.M_chips, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::JigsawDataJson Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_template_id__ = default(int);
            var __M_template_id__b__ = false;
            var __M_dimention__ = default(int);
            var __M_dimention__b__ = false;
            var __M_chips__ = default(global::System.Collections.Generic.List<global::JigsawChipJson>);
            var __M_chips__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_template_id__ = reader.ReadInt32();
                        __M_template_id__b__ = true;
                        break;
                    case 1:
                        __M_dimention__ = reader.ReadInt32();
                        __M_dimention__b__ = true;
                        break;
                    case 2:
                        __M_chips__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::JigsawChipJson>>().Deserialize(ref reader, formatterResolver);
                        __M_chips__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::JigsawDataJson();
            if(__M_template_id__b__) ____result.M_template_id = __M_template_id__;
            if(__M_dimention__b__) ____result.M_dimention = __M_dimention__;
            if(__M_chips__b__) ____result.M_chips = __M_chips__;

            return ____result;
        }
    }


    public sealed class JigsawDataFormatter : global::Utf8Json.IJsonFormatter<global::JigsawData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public JigsawDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_jon_datas"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_jon_datas"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::JigsawData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::JigsawDataJson>>().Serialize(ref writer, value.M_jon_datas, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::JigsawData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_jon_datas__ = default(global::System.Collections.Generic.List<global::JigsawDataJson>);
            var __M_jon_datas__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_jon_datas__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::JigsawDataJson>>().Deserialize(ref reader, formatterResolver);
                        __M_jon_datas__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::JigsawData();
            if(__M_jon_datas__b__) ____result.M_jon_datas = __M_jon_datas__;

            return ____result;
        }
    }


    public sealed class JigsawGameDataFormatter : global::Utf8Json.IJsonFormatter<global::JigsawGameData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public JigsawGameDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_game_template_id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_game_dimention"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_game_chips"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_game_template_id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_game_dimention"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_game_chips"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::JigsawGameData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.M_game_template_id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.M_game_dimention);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<string, global::JigsawChipJson>>().Serialize(ref writer, value.M_game_chips, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::JigsawGameData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_game_template_id__ = default(int);
            var __M_game_template_id__b__ = false;
            var __M_game_dimention__ = default(int);
            var __M_game_dimention__b__ = false;
            var __M_game_chips__ = default(global::System.Collections.Generic.Dictionary<string, global::JigsawChipJson>);
            var __M_game_chips__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_game_template_id__ = reader.ReadInt32();
                        __M_game_template_id__b__ = true;
                        break;
                    case 1:
                        __M_game_dimention__ = reader.ReadInt32();
                        __M_game_dimention__b__ = true;
                        break;
                    case 2:
                        __M_game_chips__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<string, global::JigsawChipJson>>().Deserialize(ref reader, formatterResolver);
                        __M_game_chips__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::JigsawGameData();
            if(__M_game_template_id__b__) ____result.M_game_template_id = __M_game_template_id__;
            if(__M_game_dimention__b__) ____result.M_game_dimention = __M_game_dimention__;
            if(__M_game_chips__b__) ____result.M_game_chips = __M_game_chips__;

            return ____result;
        }
    }


    public sealed class PersuadeItemDataFormatter : global::Utf8Json.IJsonFormatter<global::PersuadeItemData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PersuadeItemDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("itemId"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("talkType"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("persuadeType"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("content"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("evidenceID"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("feedbackIndex"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("nextIndex"), 6},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("forwardIndex"), 7},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("itemId"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("talkType"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("persuadeType"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("content"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("evidenceID"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("feedbackIndex"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("nextIndex"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("forwardIndex"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PersuadeItemData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.itemId);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt32(value.talkType);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteInt32(value.persuadeType);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.content);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteInt64(value.evidenceID);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteInt32(value.feedbackIndex);
            writer.WriteRaw(this.____stringByteKeys[6]);
            writer.WriteInt32(value.nextIndex);
            writer.WriteRaw(this.____stringByteKeys[7]);
            writer.WriteInt32(value.forwardIndex);
            
            writer.WriteEndObject();
        }

        public global::PersuadeItemData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __itemId__ = default(int);
            var __itemId__b__ = false;
            var __talkType__ = default(int);
            var __talkType__b__ = false;
            var __persuadeType__ = default(int);
            var __persuadeType__b__ = false;
            var __content__ = default(string);
            var __content__b__ = false;
            var __evidenceID__ = default(long);
            var __evidenceID__b__ = false;
            var __feedbackIndex__ = default(int);
            var __feedbackIndex__b__ = false;
            var __nextIndex__ = default(int);
            var __nextIndex__b__ = false;
            var __forwardIndex__ = default(int);
            var __forwardIndex__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __itemId__ = reader.ReadInt32();
                        __itemId__b__ = true;
                        break;
                    case 1:
                        __talkType__ = reader.ReadInt32();
                        __talkType__b__ = true;
                        break;
                    case 2:
                        __persuadeType__ = reader.ReadInt32();
                        __persuadeType__b__ = true;
                        break;
                    case 3:
                        __content__ = reader.ReadString();
                        __content__b__ = true;
                        break;
                    case 4:
                        __evidenceID__ = reader.ReadInt64();
                        __evidenceID__b__ = true;
                        break;
                    case 5:
                        __feedbackIndex__ = reader.ReadInt32();
                        __feedbackIndex__b__ = true;
                        break;
                    case 6:
                        __nextIndex__ = reader.ReadInt32();
                        __nextIndex__b__ = true;
                        break;
                    case 7:
                        __forwardIndex__ = reader.ReadInt32();
                        __forwardIndex__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PersuadeItemData();
            if(__itemId__b__) ____result.itemId = __itemId__;
            if(__talkType__b__) ____result.talkType = __talkType__;
            if(__persuadeType__b__) ____result.persuadeType = __persuadeType__;
            if(__content__b__) ____result.content = __content__;
            if(__evidenceID__b__) ____result.evidenceID = __evidenceID__;
            if(__feedbackIndex__b__) ____result.feedbackIndex = __feedbackIndex__;
            if(__nextIndex__b__) ____result.nextIndex = __nextIndex__;
            if(__forwardIndex__b__) ____result.forwardIndex = __forwardIndex__;

            return ____result;
        }
    }


    public sealed class PersuadeDataFormatter : global::Utf8Json.IJsonFormatter<global::PersuadeData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PersuadeDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("npcId"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("name"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("introduce"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("evidenceIds"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("m_items"), 5},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("npcId"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("introduce"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("evidenceIds"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("m_items"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PersuadeData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteInt64(value.npcId);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteString(value.name);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteString(value.introduce);
            writer.WriteRaw(this.____stringByteKeys[4]);
            formatterResolver.GetFormatterWithVerify<long[]>().Serialize(ref writer, value.evidenceIds, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[5]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::PersuadeItemData>>().Serialize(ref writer, value.m_items, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::PersuadeData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __id__ = default(int);
            var __id__b__ = false;
            var __npcId__ = default(long);
            var __npcId__b__ = false;
            var __name__ = default(string);
            var __name__b__ = false;
            var __introduce__ = default(string);
            var __introduce__b__ = false;
            var __evidenceIds__ = default(long[]);
            var __evidenceIds__b__ = false;
            var __m_items__ = default(global::System.Collections.Generic.List<global::PersuadeItemData>);
            var __m_items__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __id__ = reader.ReadInt32();
                        __id__b__ = true;
                        break;
                    case 1:
                        __npcId__ = reader.ReadInt64();
                        __npcId__b__ = true;
                        break;
                    case 2:
                        __name__ = reader.ReadString();
                        __name__b__ = true;
                        break;
                    case 3:
                        __introduce__ = reader.ReadString();
                        __introduce__b__ = true;
                        break;
                    case 4:
                        __evidenceIds__ = formatterResolver.GetFormatterWithVerify<long[]>().Deserialize(ref reader, formatterResolver);
                        __evidenceIds__b__ = true;
                        break;
                    case 5:
                        __m_items__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::PersuadeItemData>>().Deserialize(ref reader, formatterResolver);
                        __m_items__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PersuadeData();
            if(__id__b__) ____result.id = __id__;
            if(__npcId__b__) ____result.npcId = __npcId__;
            if(__name__b__) ____result.name = __name__;
            if(__introduce__b__) ____result.introduce = __introduce__;
            if(__evidenceIds__b__) ____result.evidenceIds = __evidenceIds__;
            if(__m_items__b__) ____result.m_items = __m_items__;

            return ____result;
        }
    }


    public sealed class PersuadeGroupDataFormatter : global::Utf8Json.IJsonFormatter<global::PersuadeGroupData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PersuadeGroupDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("persuadeGroup"), 0},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("persuadeGroup"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PersuadeGroupData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::PersuadeData>>().Serialize(ref writer, value.persuadeGroup, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::PersuadeGroupData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __persuadeGroup__ = default(global::System.Collections.Generic.List<global::PersuadeData>);
            var __persuadeGroup__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __persuadeGroup__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::PersuadeData>>().Deserialize(ref reader, formatterResolver);
                        __persuadeGroup__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PersuadeGroupData();
            if(__persuadeGroup__b__) ____result.persuadeGroup = __persuadeGroup__;

            return ____result;
        }
    }


    public sealed class PropUseGuidDataFormatter : global::Utf8Json.IJsonFormatter<global::PropUseGuidData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public PropUseGuidDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("taskID"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("propIDs"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("typeForAOT"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("taskID"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("propIDs"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("typeForAOT"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::PropUseGuidData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt64(value.taskID);
            writer.WriteRaw(this.____stringByteKeys[1]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<long>>().Serialize(ref writer, value.propIDs, formatterResolver);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::PropUseGuidData>>().Serialize(ref writer, value.typeForAOT, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::PropUseGuidData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __taskID__ = default(long);
            var __taskID__b__ = false;
            var __propIDs__ = default(global::System.Collections.Generic.List<long>);
            var __propIDs__b__ = false;
            var __typeForAOT__ = default(global::System.Collections.Generic.List<global::PropUseGuidData>);
            var __typeForAOT__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __taskID__ = reader.ReadInt64();
                        __taskID__b__ = true;
                        break;
                    case 1:
                        __propIDs__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<long>>().Deserialize(ref reader, formatterResolver);
                        __propIDs__b__ = true;
                        break;
                    case 2:
                        __typeForAOT__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::PropUseGuidData>>().Deserialize(ref reader, formatterResolver);
                        __typeForAOT__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::PropUseGuidData();
            if(__taskID__b__) ____result.taskID = __taskID__;
            if(__propIDs__b__) ____result.propIDs = __propIDs__;
            if(__typeForAOT__b__) ____result.typeForAOT = __typeForAOT__;

            return ____result;
        }
    }


    public sealed class RankDataFormatter : global::Utf8Json.IJsonFormatter<global::RankData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public RankDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("type"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("value"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("typeForAOT"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("type"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("value"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("typeForAOT"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::RankData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteString(value.type);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.value);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::RankData>>().Serialize(ref writer, value.typeForAOT, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::RankData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __type__ = default(string);
            var __type__b__ = false;
            var __value__ = default(string);
            var __value__b__ = false;
            var __typeForAOT__ = default(global::System.Collections.Generic.List<global::RankData>);
            var __typeForAOT__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __type__ = reader.ReadString();
                        __type__b__ = true;
                        break;
                    case 1:
                        __value__ = reader.ReadString();
                        __value__b__ = true;
                        break;
                    case 2:
                        __typeForAOT__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::RankData>>().Deserialize(ref reader, formatterResolver);
                        __typeForAOT__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::RankData();
            if(__type__b__) ____result.type = __type__;
            if(__value__b__) ____result.value = __value__;
            if(__typeForAOT__b__) ____result.typeForAOT = __typeForAOT__;

            return ____result;
        }
    }


    public sealed class ScanAnchorJsonDataFormatter : global::Utf8Json.IJsonFormatter<global::ScanAnchorJsonData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ScanAnchorJsonDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_clue_id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_x"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_y"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_w"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_h"), 4},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_clue_id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_x"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_y"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_w"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_h"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::ScanAnchorJsonData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.M_clue_id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteSingle(value.M_x);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.M_y);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.M_w);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.M_h);
            
            writer.WriteEndObject();
        }

        public global::ScanAnchorJsonData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_clue_id__ = default(int);
            var __M_clue_id__b__ = false;
            var __M_x__ = default(float);
            var __M_x__b__ = false;
            var __M_y__ = default(float);
            var __M_y__b__ = false;
            var __M_w__ = default(float);
            var __M_w__b__ = false;
            var __M_h__ = default(float);
            var __M_h__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_clue_id__ = reader.ReadInt32();
                        __M_clue_id__b__ = true;
                        break;
                    case 1:
                        __M_x__ = reader.ReadSingle();
                        __M_x__b__ = true;
                        break;
                    case 2:
                        __M_y__ = reader.ReadSingle();
                        __M_y__b__ = true;
                        break;
                    case 3:
                        __M_w__ = reader.ReadSingle();
                        __M_w__b__ = true;
                        break;
                    case 4:
                        __M_h__ = reader.ReadSingle();
                        __M_h__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::ScanAnchorJsonData();
            if(__M_clue_id__b__) ____result.M_clue_id = __M_clue_id__;
            if(__M_x__b__) ____result.M_x = __M_x__;
            if(__M_y__b__) ____result.M_y = __M_y__;
            if(__M_w__b__) ____result.M_w = __M_w__;
            if(__M_h__b__) ____result.M_h = __M_h__;

            return ____result;
        }
    }


    public sealed class ScanJsonDataFormatter : global::Utf8Json.IJsonFormatter<global::ScanJsonData>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public ScanJsonDataFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_id"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_tex_name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_tex_x"), 2},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_tex_y"), 3},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_tex_w"), 4},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_tex_h"), 5},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("M_anchors"), 6},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("M_id"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_tex_name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_tex_x"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_tex_y"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_tex_w"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_tex_h"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("M_anchors"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::ScanJsonData value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt32(value.M_id);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.M_tex_name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            writer.WriteSingle(value.M_tex_x);
            writer.WriteRaw(this.____stringByteKeys[3]);
            writer.WriteSingle(value.M_tex_y);
            writer.WriteRaw(this.____stringByteKeys[4]);
            writer.WriteSingle(value.M_tex_w);
            writer.WriteRaw(this.____stringByteKeys[5]);
            writer.WriteSingle(value.M_tex_h);
            writer.WriteRaw(this.____stringByteKeys[6]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::ScanAnchorJsonData>>().Serialize(ref writer, value.M_anchors, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::ScanJsonData Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __M_id__ = default(int);
            var __M_id__b__ = false;
            var __M_tex_name__ = default(string);
            var __M_tex_name__b__ = false;
            var __M_tex_x__ = default(float);
            var __M_tex_x__b__ = false;
            var __M_tex_y__ = default(float);
            var __M_tex_y__b__ = false;
            var __M_tex_w__ = default(float);
            var __M_tex_w__b__ = false;
            var __M_tex_h__ = default(float);
            var __M_tex_h__b__ = false;
            var __M_anchors__ = default(global::System.Collections.Generic.List<global::ScanAnchorJsonData>);
            var __M_anchors__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __M_id__ = reader.ReadInt32();
                        __M_id__b__ = true;
                        break;
                    case 1:
                        __M_tex_name__ = reader.ReadString();
                        __M_tex_name__b__ = true;
                        break;
                    case 2:
                        __M_tex_x__ = reader.ReadSingle();
                        __M_tex_x__b__ = true;
                        break;
                    case 3:
                        __M_tex_y__ = reader.ReadSingle();
                        __M_tex_y__b__ = true;
                        break;
                    case 4:
                        __M_tex_w__ = reader.ReadSingle();
                        __M_tex_w__b__ = true;
                        break;
                    case 5:
                        __M_tex_h__ = reader.ReadSingle();
                        __M_tex_h__b__ = true;
                        break;
                    case 6:
                        __M_anchors__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::ScanAnchorJsonData>>().Deserialize(ref reader, formatterResolver);
                        __M_anchors__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::ScanJsonData();
            if(__M_id__b__) ____result.M_id = __M_id__;
            if(__M_tex_name__b__) ____result.M_tex_name = __M_tex_name__;
            if(__M_tex_x__b__) ____result.M_tex_x = __M_tex_x__;
            if(__M_tex_y__b__) ____result.M_tex_y = __M_tex_y__;
            if(__M_tex_w__b__) ____result.M_tex_w = __M_tex_w__;
            if(__M_tex_h__b__) ____result.M_tex_h = __M_tex_h__;
            if(__M_anchors__b__) ____result.M_anchors = __M_anchors__;

            return ____result;
        }
    }


    public sealed class TaskOnBuildFormatter : global::Utf8Json.IJsonFormatter<global::TaskOnBuild>
    {
        readonly global::Utf8Json.Internal.AutomataDictionary ____keyMapping;
        readonly byte[][] ____stringByteKeys;

        public TaskOnBuildFormatter()
        {
            this.____keyMapping = new global::Utf8Json.Internal.AutomataDictionary()
            {
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("TaskID"), 0},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("Name"), 1},
                { JsonWriter.GetEncodedPropertyNameWithoutQuotation("TaskOnBuilds"), 2},
            };

            this.____stringByteKeys = new byte[][]
            {
                JsonWriter.GetEncodedPropertyNameWithBeginObject("TaskID"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("Name"),
                JsonWriter.GetEncodedPropertyNameWithPrefixValueSeparator("TaskOnBuilds"),
                
            };
        }

        public void Serialize(ref JsonWriter writer, global::TaskOnBuild value, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteRaw(this.____stringByteKeys[0]);
            writer.WriteInt64(value.TaskID);
            writer.WriteRaw(this.____stringByteKeys[1]);
            writer.WriteString(value.Name);
            writer.WriteRaw(this.____stringByteKeys[2]);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::TaskOnBuild>>().Serialize(ref writer, value.TaskOnBuilds, formatterResolver);
            
            writer.WriteEndObject();
        }

        public global::TaskOnBuild Deserialize(ref JsonReader reader, global::Utf8Json.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __TaskID__ = default(long);
            var __TaskID__b__ = false;
            var __Name__ = default(string);
            var __Name__b__ = false;
            var __TaskOnBuilds__ = default(global::System.Collections.Generic.List<global::TaskOnBuild>);
            var __TaskOnBuilds__b__ = false;

            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                var stringKey = reader.ReadPropertyNameSegmentRaw();
                int key;
                if (!____keyMapping.TryGetValueSafe(stringKey, out key))
                {
                    reader.ReadNextBlock();
                    goto NEXT_LOOP;
                }

                switch (key)
                {
                    case 0:
                        __TaskID__ = reader.ReadInt64();
                        __TaskID__b__ = true;
                        break;
                    case 1:
                        __Name__ = reader.ReadString();
                        __Name__b__ = true;
                        break;
                    case 2:
                        __TaskOnBuilds__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.List<global::TaskOnBuild>>().Deserialize(ref reader, formatterResolver);
                        __TaskOnBuilds__b__ = true;
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }

                NEXT_LOOP:
                continue;
            }

            var ____result = new global::TaskOnBuild();
            if(__TaskID__b__) ____result.TaskID = __TaskID__;
            if(__Name__b__) ____result.Name = __Name__;
            if(__TaskOnBuilds__b__) ____result.TaskOnBuilds = __TaskOnBuilds__;

            return ____result;
        }
    }

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
