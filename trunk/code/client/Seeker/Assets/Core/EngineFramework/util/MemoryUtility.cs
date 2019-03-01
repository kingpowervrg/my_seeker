/********************************************************************
	created:  2018-4-25 11:32:21
	filename: MemoryUtility.cs
	author:	  songguangze@outlook.com
	
	purpose:  内存流相关操作工具库
*********************************************************************/
using System;
using System.IO;

namespace EngineCore
{

    public static class MemoryUtility
    {
        /// <summary>
        /// 内存流中写入Int
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <param name="writeValue"></param>
        /// <param name="writeOffset"></param>
        public static void WriteIntToMemoryStream(MemoryStream memoryStream, int writeValue, int writeOffset = 0)
        {
            byte[] bytesValues = BitConverter.GetBytes(writeValue);
            memoryStream.Write(bytesValues, writeOffset, bytesValues.Length);
        }

    }
}