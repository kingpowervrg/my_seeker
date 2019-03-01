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
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(6)
            {
                {typeof(global::System.Collections.Generic.List<global::JigsawChipJson>), 0 },
                {typeof(global::System.Collections.Generic.List<global::JigsawDataJson>), 1 },
                {typeof(global::RectJson), 2 },
                {typeof(global::JigsawChipJson), 3 },
                {typeof(global::JigsawDataJson), 4 },
                {typeof(global::JigsawData), 5 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::Utf8Json.Formatters.ListFormatter<global::JigsawChipJson>();
                case 1: return new global::Utf8Json.Formatters.ListFormatter<global::JigsawDataJson>();
                case 2: return new Utf8Json.Formatters.RectJsonFormatter();
                case 3: return new Utf8Json.Formatters.JigsawChipJsonFormatter();
                case 4: return new Utf8Json.Formatters.JigsawDataJsonFormatter();
                case 5: return new Utf8Json.Formatters.JigsawDataFormatter();
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

namespace Utf8Json.Formatters
{
    using System;
    using Utf8Json;


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

}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
