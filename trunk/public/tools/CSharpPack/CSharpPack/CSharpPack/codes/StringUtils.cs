using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSharpPack.codes
{
    public class StringUtils
    {
        public static bool IsValidJson(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return false;

            jsonStr = jsonStr.Trim();

            if ((jsonStr.StartsWith("{") && jsonStr.EndsWith("}")) || //For object
       (jsonStr.StartsWith("[") && jsonStr.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(jsonStr);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
