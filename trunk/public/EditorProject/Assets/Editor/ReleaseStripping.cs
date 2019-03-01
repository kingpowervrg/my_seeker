using System.Collections.Generic;


namespace EngineCore.Editor
{
    public class ReleaseStripping
    {
        public static IReleaseStripping releaseStrippingImpl = new ReleaseStrippingForTheChief();

        public static Dictionary<string, object> GetStrippingAssets()
        {
            return releaseStrippingImpl.GetStrippingAssets();
        }
    }
}