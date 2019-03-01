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
        class ResIdxMap : ResComponent
    {
        private Dictionary<int, string> mDicIntIdx = new Dictionary<int, string>();

        internal void RegResIdx(string assetname, string bundlename)
        {
            if ( mDicIntIdx.ContainsKey( assetname.GetHashCode() ) )
            {
                //Logger.GetFile( LogFile.Res ).LogError( "ResIdxMap:" + assetname + " is exist in "
                    //+ GetBundleName(assetname));
                return;
            }
            mDicIntIdx.Add( assetname.GetHashCode(), bundlename );
        }

        internal void UnRegisterByAssetName(string assetname)
        {
            mDicIntIdx.Remove( assetname.GetHashCode() );
        }

        internal string GetBundleName(string assetname)
        {
            string bundlename;
            if ( mDicIntIdx.TryGetValue( assetname.GetHashCode(), out bundlename ) )
            {
                return bundlename;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
