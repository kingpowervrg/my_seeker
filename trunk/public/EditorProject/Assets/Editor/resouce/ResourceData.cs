using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resource
{
    public class ResourceData
    {
        public static string m_RelativeResourceDir = "Res/";
        public static string m_AbsoluteResourceDir
        {
            get { return UnityEngine.Application.dataPath + "/" + m_RelativeResourceDir; }
        }

        public static string configPath
        {
            get { return SceneTools.getConfigPath(); }
        }

        public static string propConfigPath
        {
            get { return configPath + "exhibit.xlsx"; }
        }

        public static string sceneDataConfigPath
        {
            get { return configPath + "scenedata.json"; }
        }

        private static List<BaseItem> m_exhibitItem = new List<BaseItem>();
        public static List<BaseItem> exhibitItem
        {
            get
            {
                if (m_exhibitItem == null || m_exhibitItem.Count == 0)
                {
                    m_exhibitItem = new List<BaseItem>();
                    try
                    {
                        System.Reflection.PropertyInfo[] proInfo;
                        conf.ConfHelper.LoadFromExcel<BaseItem>(propConfigPath, 0, out m_exhibitItem, out proInfo);
                    }
                    catch (System.IO.IOException ex)
                    {
                        m_exhibitItem = new List<BaseItem>();
                        UnityEditor.EditorUtility.DisplayDialog("Title", "先关表!!!", "是");
                    }
                }
                return m_exhibitItem;
            }
        }

        public readonly static List<string> m_srcShader = new List<string> { "Mobile/Diffuse", "Legacy Shaders/Transparent/Diffuse","Legacy Shaders/Diffuse", "Legacy Shaders/Transparent/Cutout/Diffuse" };
        public readonly static List<string> m_destShader = new List<string> { "Seeker/Exhibit/Diffuse", "Seeker/Exhibit/Alpha", "Seeker/Exhibit/Diffuse", "Seeker/Exhibit/Cutoff" };
    }
}
