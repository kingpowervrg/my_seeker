/********************************************************************
	created:  2018-9-19 18:54:15
	filename: ReleaseStrippingEntry.cs
	author:	  songguangze@outlook.com
	
	purpose:  Release 版本剥离资源配置
*********************************************************************/
using System.Collections.Generic;

namespace EngineCore.Editor
{
    [JsonObjectType(JsonObjectHelperTypes.RelaseStripping)]
    public class ReleaseStrippingWindow : JsonObjectUIHelper
    {

        

        public override bool CanDelete()
        {
            return true;
        }

        public override bool CanNew()
        {
            return true;
        }

        public override bool CanSave()
        {
            return true;
        }

        public override bool DoesNeedStarted()
        {
            return false;
        }

        public override Dictionary<string, string> EnumOptions(object target, string paramName)
        {
            throw new System.NotImplementedException();
        }

        public override string GetFileExt()
        {
            return string.Empty;
        }

        public override string GetSearchDir()
        {
            return string.Empty;
        }

        public override void MakeEditUI(object target)
        {
        }

        public override object OnNew()
        {
            return null;
        }

        public override object OnSelect(string strFullName)
        {
            return null;
        }

        public override bool SearchForDir()
        {
            return false;
        }
    }
}