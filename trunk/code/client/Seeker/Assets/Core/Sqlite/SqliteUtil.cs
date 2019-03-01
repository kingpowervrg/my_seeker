using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SqliteDriver
{
    public static class SqliteUtil
    {
        static object GetIntermData(BinaryReader br, int type)
        {
            object obj = new object();
            switch (type)
            {
                case 1:
                    int int_data = br.ReadInt32();
                    obj = (object)int_data;
                    break;
                case 2:
                    string str_data = br.ReadString();
                    obj = (object)str_data;
                    break;
                case 3:
                    float float_data = br.ReadSingle();
                    obj = (object)float_data;
                    break;
                case 4:
                    bool bool_data = br.ReadBoolean();
                    obj = (object)bool_data;
                    break;
                case 5:
                    short short_data = br.ReadInt16();
                    obj = (object)short_data;
                    break;
                case 6:
                    byte byte_data = br.ReadByte();
                    obj = (object)byte_data;
                    break;
                case 7:
                    long long_data = br.ReadInt64();
                    obj = (object)long_data;
                    break;
                case 8:
                    double double_data = br.ReadDouble();
                    obj = (object)double_data;
                    break;
                case 11:
                    int arr_len_int = br.ReadInt32();
                    int[] arr_int = new int[arr_len_int];
                    for (int i = 0; i < arr_len_int; i++)
                        arr_int[i] = br.ReadInt32();
                    obj = (object)arr_int;
                    break;
                case 12:
                    int arr_len_str = br.ReadInt32();
                    string[] arr_str = new string[arr_len_str];
                    for (int i = 0; i < arr_len_str; i++)
                        arr_str[i] = br.ReadString();
                    obj = (object)arr_str;
                    break;
                case 13:
                    int arr_len_float = br.ReadInt32();
                    float[] arr_float = new float[arr_len_float];
                    for (int i = 0; i < arr_len_float; i++)
                        arr_float[i] = br.ReadSingle();
                    obj = (object)arr_float;
                    break;
                case 14:
                    int arr_len_bool = br.ReadInt32();
                    bool[] arr_bool = new bool[arr_len_bool];
                    for (int i = 0; i < arr_len_bool; i++)
                        arr_bool[i] = br.ReadBoolean();
                    obj = (object)arr_bool;
                    break;
                case 15:
                    int arr_len_short = br.ReadInt32();
                    short[] arr_short = new short[arr_len_short];
                    for (int i = 0; i < arr_len_short; i++)
                        arr_short[i] = (short)br.ReadInt32();
                    obj = (object)arr_short;
                    break;
                case 16:
                    int arr_len_byte = br.ReadInt32();
                    byte[] arr_byte = new byte[arr_len_byte];
                    for (int i = 0; i < arr_len_byte; i++)
                        arr_byte[i] = (byte)br.ReadInt32();
                    obj = (object)arr_byte;
                    break;
                case 17:
                    int arr_len_long = br.ReadInt32();
                    long[] arr_long = new long[arr_len_long];
                    for (int i = 0; i < arr_len_long; i++)
                        arr_long[i] = br.ReadInt64();
                    obj = (object)arr_long;
                    break;
                case 18:
                    int arr_len_double = br.ReadInt32();
                    double[] arr_double = new double[arr_len_double];
                    for (int i = 0; i < arr_len_double; i++)
                        arr_double[i] = br.ReadDouble();
                    obj = (object)arr_double;
                    break;
            }
            return obj;
        }
        public static void GetRowIntermData(BinaryReader br, int col_cnt, Dictionary<string, object> dic_interm_val, List<int> dic_type, List<string> field_list)
        {
            for (int i = 0; i < col_cnt; i++)
            {
                dic_interm_val[field_list[i]] = GetIntermData(br, dic_type[i]);
            }
        }

        static byte[] sqliteBuffer = new byte[8 * 1024];
        static System.IO.MemoryStream sqliteMS = new MemoryStream(sqliteBuffer);
        static System.IO.BinaryReader sqliteBR = new System.IO.BinaryReader(sqliteMS);
#if SERVER_LIB
        static object lockObj = new object();
#endif
        public static object GetArrayData(this DataTable reader, int columnIdx, int type)
        {
#if SERVER_LIB
            lock (lockObj)
#endif
            {
                reader.GetBytes(columnIdx, 0, sqliteBuffer, 0, 8 * 1024);
                sqliteMS.Position = 0;
                return GetIntermData(sqliteBR, type);
            }

        }
    }
}
