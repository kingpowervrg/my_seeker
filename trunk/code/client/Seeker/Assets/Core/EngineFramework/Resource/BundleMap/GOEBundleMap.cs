using System.IO;

namespace EngineCore
{
    public class BundleMapItem
    {
        const char splitchar = ':';
        string _name = string.Empty, _finalName = string.Empty, _md5 = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        public string FinalName
        {
            get
            {
                return _finalName;
            }
            set
            {
                _finalName = value;
            }
        }
        public int Size { get; set; }
        public string MD5
        {
            get
            {
                return _md5;
            }
            set
            {
                _md5 = value;
            }
        }

#if UNITY_EDITOR
        public bool Create(string path)
        {
            _name = Path.GetFileName(path);
            _finalName = string.Empty;
            Size = (int)EngineFileUtil.GetFileLength(path);
            MD5 = string.Empty;
            FileStream fs = new FileStream(path, FileMode.Open);
            if (null != fs)
            {
                MD5 = SysUtil.GetMD5Str(fs);
                fs.Close();
            }
            if (MD5 == string.Empty)
            {
                return false;
            }
            return true;
        }
#endif
        public void FromString(string str)
        {
            string[] strs = JsonUtil.StringToStringArray(str, splitchar);
            _name = strs[0];
            _finalName = strs[1];
            Size = int.Parse(strs[2]);
            _md5 = strs[3];
        }

        public override string ToString()
        {
            return Name
                + splitchar + FinalName
                    + splitchar + Size
                        + splitchar + MD5;
        }
    }

}

