// using System.Collections.Generic;

// namespace EngineCore
// {
//     /// <summary>
//     /// Bundle信息
//     /// </summary>
//     public class BundleInfo
//     {
//         private int mSize = 0;
//         private HashSet<string> mSetFile = new HashSet<string>();
//         HashSet<string> dependOn = new HashSet<string>();
//         public string mName = string.Empty;

//         public string FirstAsset { get; set; }

//         public bool BundlePersistent { get; set; }

//         public bool AssetPersistent { get; set; }

//         public bool IsDependency { get; set; }

//         public bool CanForceRelease { get; set; }

//         public int Size
//         {
//             set { mSize = value; }
//             get { return mSize; }
//         }

//         internal void AddFile(string file)
//         {
//             mSetFile.Add(file);
//         }

//         public HashSet<string> Files
//         {
//             get { return mSetFile; }
//             set { this.mSetFile = value; }
//         }

//         public HashSet<string> DependsOn
//         {
//             get { return dependOn; }
//             set { this.dependOn = value; }
//         }

//     }
// }
