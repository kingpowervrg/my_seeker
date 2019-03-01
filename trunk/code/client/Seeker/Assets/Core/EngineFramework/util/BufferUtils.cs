/********************************************************************
	created:  2019-1-26 16:38:30
	filename: BufferUtils.cs
	author:	  songguangze@outlook.com
	
	purpose:   二制组帮助类
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace EngineCore.Utility
{
    public static class BufferUtils
    {

        /// <summary>
        /// Buffer 转字符串
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isUppercase"></param>
        /// <returns></returns>
        public static string BufferToHexString(byte[] buffer, bool isUppercase)
        {
            StringBuilder result = new StringBuilder(buffer.Length * 2);

            for (int i = 0; i < buffer.Length; i++)
                result.Append(buffer[i].ToString(isUppercase ? "X2" : "x2"));

            return result.ToString();
        }

        /// <summary>
        /// 两个Buffer是否相等
        /// </summary>
        /// <param name="buffer1"></param>
        /// <param name="buffer2"></param>
        /// <returns></returns>
        public static bool IsEqual(byte[] buffer1, byte[] buffer2)
        {
            if (buffer1.Length != buffer2.Length)
                return false;

            if (object.ReferenceEquals(buffer1, buffer2))
                return true;

            for (int i = 0; i < buffer1.Length; i++)
            {
                if (buffer1[i] != buffer2[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        ///  计算文件的MD5检验码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] ComputeFileMD5(string filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                MD5 dstPrefabMd5Code = MD5.Create();
                byte[] fileMd5Buffer = dstPrefabMd5Code.ComputeHash(fs);

                return fileMd5Buffer;
            }
        }
    }
}