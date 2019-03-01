using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngineCore.Editor
{
    public enum JsonObjectHelperTypes
    {
        [JsonObjectHelperDisplayName("打包")]
        Pack,
        [JsonObjectHelperDisplayName("打包(Unity5)")]
        PackV5,
        [JsonObjectHelperDisplayName("技能动画")]
        CharacterAct,
        [JsonObjectHelperDisplayName("曲线编辑器")]
        CurveAnimation,
        [JsonObjectHelperDisplayName("配置文件")]
        Config,
        [JsonObjectHelperDisplayName("Release Stripping配置")]
        RelaseStripping,
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class JsonObjectHelperDisplayNameAttribute : Attribute
    {
        readonly string displayName;

        public JsonObjectHelperDisplayNameAttribute(string displayName)
        {
            this.displayName = displayName;
        }

        public string DisplayName
        {
            get { return displayName; }
        }
    }
}
