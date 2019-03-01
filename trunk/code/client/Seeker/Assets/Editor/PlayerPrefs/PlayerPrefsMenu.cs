using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// 绘制顶级导航菜单
    /// </summary>
    static public class NGUIMenu
    {

        [MenuItem("PlayerPrefs/ClearPrefs", false, 12)]
        static public void ClearPrefs() { ClearAllData(); }
       

        /// <summary>
        /// 显示帮助
        /// </summary>
        static public void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        [MenuItem("PlayerPrefs/ClearRedPoints", false, 12)]
        static public void ClearRedPoints() { ClearRedData(); }

        static public void ClearRedData()
        {
            PlayerPrefs.DeleteKey("E_NEW_FRIEND");
            PlayerPrefs.DeleteKey("E_NEW_EMAIL");
            PlayerPrefs.DeleteKey("E_NEW_ACTIVITY");
            PlayerPrefs.DeleteKey("E_NEW_ACHIEVEMENT");
            PlayerPrefs.DeleteKey("E_NEW_NOTICE");
            PlayerPrefs.DeleteKey("E_NEW_POLICE");
        }
    }
}