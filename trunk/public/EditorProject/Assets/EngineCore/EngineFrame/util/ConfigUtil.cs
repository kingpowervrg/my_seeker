using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 用于协助读取conf文件
/// 这个类通常不会直接被使用
/// </summary>
public class ConfigUtil
{
    public static void LoadRes(string name, Action<string, UnityEngine.Object> OnLoadFile)
    {
        EngineCore.EngineCoreEvents.EngineEvent.GetAssetEvent.SafeInvoke(name, OnLoadFile, EngineCore.LoadPriority.Default);
    }


    public static void ReleaseAsset(string name, UnityEngine.Object obj)
    {
        EngineCore.EngineCoreEvents.EngineEvent.ReleaseAssetEvent.SafeInvoke(name, obj);
    }

    public static void RemoveAsset(string name)
    {
        EngineCore.EngineCoreEvents.EngineEvent.RemoveAssetEvent.SafeInvoke(name, true, true);
    }

    /**type 声明 1 int 11 int[] 2 string 12 string[] 3 float 13 float[] 4 bool 14 bool[] 
     * 5 short 15 short[] 6 byte 16 byte[] 7 long 17 long[] 8 double 18 double[]
     * 
     * */
    public static void ReadColumnData(BinaryReader br, int col_cnt, List<int> dic_type, List<string> field_list)
    {
        for (int i = 0; i < col_cnt; i++)
        {

            string name = br.ReadString();
            string type = br.ReadString();
            field_list.Add(name);
            if (type.StartsWith("int"))
            {
                if (type.Contains("["))
                {
                    dic_type.Add(11);
                }
                else
                {
                    dic_type.Add(1);
                }
            }
            else if (type.StartsWith("string"))
            {
                if (type.Contains("["))
                {
                    dic_type.Add(12);
                }
                else
                {
                    dic_type.Add(2);
                }
            }
            else if (type.StartsWith("float"))
            {
                if (type.Contains("["))
                {
                    dic_type.Add(13);
                }
                else
                {
                    dic_type.Add(3);
                }
            }
            else if (type.StartsWith("bool") || type.StartsWith("boolean"))
            {
                if (type.Contains("["))
                {
                    dic_type.Add(14);
                }
                else
                {
                    dic_type.Add(4);
                }
            }
            else if (type.StartsWith("short"))
            {
                if (type.Contains("["))
                {
                    dic_type.Add(15);
                }
                else
                {
                    dic_type.Add(5);
                }
            }
            else if (type.StartsWith("byte"))
            {
                if (type.Contains("["))
                {
                    dic_type.Add(16);
                }
                else
                {
                    dic_type.Add(6);
                }
            }
            else if (type.StartsWith("long"))
            {
                if (type.Contains("["))
                {
                    dic_type.Add(17);
                }
                else
                {
                    dic_type.Add(7);
                }
            }
            else if (type.Contains("double"))
            {
                if (type.Contains("["))
                {
                    dic_type.Add(18);
                }
                else
                {
                    dic_type.Add(8);
                }
            }

        }
    }
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
}


