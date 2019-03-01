using System;
using System.IO;
using System.Collections.Generic;
namespace EngineCore
{
    public class BundleInfoMapItem
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

    public struct BundleInfoInfo
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public HashSet<string> Assets { get; set; }
        public bool CanForceRelease { get; set; }
        public HashSet<string> Dependencies { get; set; }
    }

    public class BundleInfoMap
    {
        Dictionary<string, BundleInfoMapItem> _bundleMap = new Dictionary<string, BundleInfoMapItem>();
        public Dictionary<string, BundleInfoMapItem> BundleMap
        {
            get
            {
                return _bundleMap;
            }
        }
        public const string BUNDLE_INFO_PREFIX = "&BundleInfo%";

        public bool Read(string stream, bool Register = true, List<BundleInfoInfo> infoList = null)
        {
            _bundleMap.Clear();
            return AppendRead(stream, Register, infoList);
        }


        public bool AppendRead(string stream, bool Register = true, List<BundleInfoInfo> infoList = null)
        {
            StringReader sr = new StringReader(stream);
            string line = sr.ReadLine();
            while (null != line)
            {
                if (line.StartsWith(BUNDLE_INFO_PREFIX))
                {
                    byte[] buf = Convert.FromBase64String(line.Substring(BUNDLE_INFO_PREFIX.Length + 1));
                    MemoryStream ms = new MemoryStream(buf);
                    BinaryReader br = new BinaryReader(ms);

                    int bundleCnt = br.ReadInt32();
                    for (int i = 0; i < bundleCnt; i++)
                    {

                        string name = br.ReadString();
                        int size = br.ReadInt32();
                        bool canForceRelease = br.ReadBoolean();
                        int assetCnt = br.ReadInt32();
                        HashSet<string> assets = new HashSet<string>();
                        HashSet<string> DependsOn = new HashSet<string>();
                        string firstAsset = null;
                        for (int j = 0; j < assetCnt; j++)
                        {
                            string assetName = br.ReadString();

                            if (firstAsset == null)
                                firstAsset = assetName;
                            assets.Add(assetName);
                            if (Register)
                            {
                                ResourceMgr.Instance().RegisterBundleIdx(assetName, name, size);
                            }

                        }
                        if (Register)
                        {
                            BundleInfo bundle = ResourceMgr.Instance().GetBundle(name);
                            bundle.FirstAsset = firstAsset;
                            bundle.CanForceRelease = canForceRelease;
                            int dependCnt = br.ReadInt32();
                            for (int j = 0; j < dependCnt; j++)
                            {
                                string depName = br.ReadString();
                                bundle.DependsOn.Add(depName);
                            }

                        }
                        else
                        {
                            int dependCnt = br.ReadInt32();
                            for (int j = 0; j < dependCnt; j++)
                            {
                                string depName = br.ReadString();
                                DependsOn.Add(depName);
                            }
                        }
                        if (infoList != null)
                        {
                            BundleInfoInfo _bundleInfo = new BundleInfoInfo();
                            _bundleInfo.Name = name;
                            _bundleInfo.Size = size;
                            _bundleInfo.Assets = assets;
                            _bundleInfo.Dependencies = DependsOn;
                            infoList.Add(_bundleInfo);
                        }
                    }
                }
                else
                {
                    BundleInfoMapItem bmi = new BundleInfoMapItem();
                    bmi.FromString(line);
                    if (!_bundleMap.ContainsKey(bmi.Name))
                    {
                        _bundleMap.Add(bmi.Name, bmi);
                    }
                }
                line = sr.ReadLine();
            }
            return true;
        }

        public string ToString(List<BundleInfoInfo> lst)
        {
            StringWriter sw = new StringWriter();
            sw.Write(ToString());
            sw.Write(BUNDLE_INFO_PREFIX);
            sw.Write(':');
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(lst.Count);
            foreach (var i in lst)
            {
                bw.Write(i.Name);
                bw.Write(i.Size);
                bw.Write(i.CanForceRelease);
                bw.Write(i.Assets.Count);

                foreach (var j in i.Assets)
                {
                    bw.Write(j);
                }
                bw.Write(i.Dependencies.Count);
                foreach (var j in i.Dependencies)
                {
                    bw.Write(j);
                }
            }
            sw.WriteLine(Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length));
            return sw.ToString();
        }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            foreach (KeyValuePair<string, BundleInfoMapItem> kvp in _bundleMap)
            {
                BundleInfoMapItem bmi = kvp.Value;
                sw.WriteLine(bmi.ToString());
            }
            return sw.ToString();
        }
    }
}

