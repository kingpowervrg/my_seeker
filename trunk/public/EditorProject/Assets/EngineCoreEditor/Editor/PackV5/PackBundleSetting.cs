
using GOEngine.Implement;

namespace EngineCore.Editor
{
#if UNITY_EDITOR
    //打包项//
    public class PackBundleSetting
    {
        private int minSize = 0;
        private int maxSize = int.MaxValue;

        [DisplayName("Bundle名")]
        public string BundleName
        {
            get; set;
        }

        [DisplayName("子目录")]
        public string SubFolder
        {
            get; set;
        }
        [DisplayName("搜索Filter")]
        public string SearchFilters
        {
            get; set;
        }
        [DisplayName("必要搜索Filter")]
        public string NecessaryFilters
        {
            get; set;
        }
        [DisplayName("排除搜索Filter")]
        public string ExcludeFilters
        {
            get; set;
        }

        [DisplayName("是否搜索子目录")]
        public bool SearchSubDir
        {
            get; set;
        }

        [DisplayName("最小文件尺寸")]
        public int MinSize
        {
            get { return minSize; }
            set { minSize = value; }
        }

        [DisplayName("最大文件尺寸")]
        public int MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        [JsonFieldAttribute(JsonFieldTypes.PackType)]
        [DisplayName("打包类型")]
        public string PackType
        {
            get;
            set;
        }

        [DisplayName("是否可再取得Asset后卸载bundle")]
        public bool CanForceReleaseBundle
        {
            get;
            set;
        }
    }
#endif

}
