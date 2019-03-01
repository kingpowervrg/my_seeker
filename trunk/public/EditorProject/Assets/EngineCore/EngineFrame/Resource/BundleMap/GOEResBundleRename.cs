using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EngineCore
{
#if UNITY_EDITOR
    public
#else
    internal
#endif
 class BundleRename : ResComponent
    {
        internal BundleInfoMap bundlemap = new BundleInfoMap();

        internal void Read(string stream, bool register = true)
        {
            bundlemap.Read(stream, register);
        }

        internal void AppendRead(string stream, bool register = true)
        {
            bundlemap.AppendRead(stream, register);
        }

        internal string GetBundleNameFromOriginalName(string originalName)
        {
            BundleInfoMapItem bmi = GetBundleItemFromOriginalName(originalName);
            if (null == bmi)
                return null;
            else
                return bmi.FinalName;
        }

        internal BundleInfoMapItem GetBundleItemFromOriginalName(string originalName)
        {
            BundleInfoMapItem bmi = null;
            if (bundlemap.BundleMap.TryGetValue(originalName, out bmi))
            {
                return bmi;
            }
            else
            {
                //if (!originalName.Equals(SysConf.BUNDLEMAP_FILE))
                    //Logger.GetFile(LogFile.Res).LogError(originalName + " not found in BundleMap");

                return null;
            }
        }
    }
}
