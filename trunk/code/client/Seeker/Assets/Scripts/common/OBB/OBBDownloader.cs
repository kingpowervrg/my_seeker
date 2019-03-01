using UnityEngine;
namespace SeekerGame
{
    public class OBBDownloader
    {
        static string mainPath = null;
        static string patchPath = null;
        static bool need_download = false;
        public static bool Check()
        {
            string content = "odd touched";
            bool need_download = false;

            IGooglePlayObbDownloader googlePlayObbDownloader = GooglePlayObbDownloadManager.GetGooglePlayObbDownloader();
            if (googlePlayObbDownloader == null)
            {
                content = "Use GooglePlayDownloader only on Android device!";
                return false;
            }

            string expPath = googlePlayObbDownloader.GetExpansionFilePath();
            if (string.IsNullOrEmpty(expPath))
            {
                content = "External storage is not available!";
            }
            else
            {
                string mainPath = googlePlayObbDownloader.GetMainOBBPath();

                if (string.IsNullOrEmpty(mainPath))
                {
                    need_download = true;

                    content = "need download OBBs";

                    googlePlayObbDownloader.FetchOBB();
                }

            }
            Debug.Log(content);

            return need_download;
        }

    }
}
