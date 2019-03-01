/********************************************************************
	created:  2018-6-1 14:53:8
	filename: Builder.cs
	author:	  songguangze@fotoable.com
	
	purpose:  打包可执行文件
*********************************************************************/
using EngineCore.Editor;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018
using UnityEditor.Build.Reporting;
#endif
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;


public class Builder :
#if UNITY_2017
    IPreprocessBuild
#elif UNITY_2018
    IPreprocessBuildWithReport
#endif

{
    public static string TRUNK_ROOT = "../../../../";
    public static string WINDOWS_PATH = "release/client/TheChief.exe";
    public static string[] BUILD_SCENES = new string[] { "Assets/GameClient.unity" };

    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }

    /// <summary>
    /// 打包成Windows
    /// </summary>
    public static void BuildWindowsplayer()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        string buildFullPath = GetOutputPath(BuildTarget.StandaloneWindows64);
        string buildPath = Path.GetDirectoryName(buildFullPath);
        EngineCore.EngineFileUtil.DeleteDirectory(buildPath);
        Directory.CreateDirectory(buildPath);
#if UNITY_DEBUG
        BuildGame(BUILD_SCENES, GetOutputPath(BuildTarget.StandaloneWindows64), BuildTarget.StandaloneWindows64, BuildOptions.AllowDebugging | BuildOptions.Development);
#else
        BuildGame(BUILD_SCENES, GetOutputPath(BuildTarget.StandaloneWindows64), BuildTarget.StandaloneWindows64, BuildOptions.None);
#endif
    }



    public static void BuildAndroidPlayer()
    {
        //PackBuildinShaders.PackAlwaysBuildinShaders();

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        PlayerSettings.applicationIdentifier = "com.fotoable.thechief";
        string buildPath = GetOutputPath(BuildTarget.Android);
        PlayerSettings.Android.useAPKExpansionFiles = false;
        PlayerSettings.Android.keyaliasName = null;
        PlayerSettings.Android.keystorePass = null;
        PlayerSettings.Android.keystoreName = null;
        PlayerSettings.Android.keyaliasPass = null;

#if UNITY_2017
        BuildGame(BUILD_SCENES, buildPath, BuildTarget.Android, BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.Il2CPP);
#elif UNITY_2018
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        BuildGame(BUILD_SCENES, buildPath, BuildTarget.Android, BuildOptions.AllowDebugging | BuildOptions.Development);
#endif

        //干掉之前的
        FileInfo fileInfo = new FileInfo(buildPath);
        FileInfo[] directoryFiles = fileInfo.Directory.GetFiles();
        for (int i = 0; i < directoryFiles.Length; ++i)
        {
            if (directoryFiles[i].Name != fileInfo.Name)
                directoryFiles[i].Delete();
        }
    }

    public static void BuildIOSPlayer()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
        PlayerSettings.applicationIdentifier = "com.fotoable.thechief";
        PlayerSettings.use32BitDisplayBuffer = false;
        string buildPath = GetOutputPath(BuildTarget.iOS);

        //干掉之前的
        FileInfo fileInfo = new FileInfo(buildPath);
        FileInfo[] directoryFiles = fileInfo.Directory.GetFiles();
        for (int i = 0; i < directoryFiles.Length; ++i)
        {
            if (directoryFiles[i].Name != fileInfo.Name)
                directoryFiles[i].Delete();
        }
#if UNITY_2017
        BuildGame(BUILD_SCENES, buildPath, BuildTarget.iOS, BuildOptions.AllowDebugging | BuildOptions.Development | BuildOptions.Il2CPP);
#elif UNITY_2018
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        BuildGame(BUILD_SCENES, buildPath, BuildTarget.iOS, BuildOptions.AllowDebugging | BuildOptions.Development);
#endif
    }

    private static void BuildGame(string[] scenes, string outputPath, BuildTarget buildTarget, BuildOptions buildOptions)
    {
#if UNITY_2017
        string buildResult = BuildPipeline.BuildPlayer(scenes, outputPath, buildTarget, buildOptions);
        if (!string.IsNullOrEmpty(buildResult))
            throw new Exception($"Build Failed ,error message :" + buildResult);
#elif UNITY_2018
        BuildReport buildResult = BuildPipeline.BuildPlayer(scenes, outputPath, buildTarget, buildOptions);
        if (buildResult.summary.result == BuildResult.Failed)
            throw new Exception($"Build Failed ,error message :" + buildResult.summary.ToString());
#endif
    }


    /// <summary>
    /// 获取输出路径
    /// </summary>
    /// <param name="buildTarget"></param>
    private static string GetOutputPath(BuildTarget buildTarget)
    {
        string projectRootPath = Path.GetDirectoryName(Path.Combine(Application.dataPath, TRUNK_ROOT));
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows64:
                return Path.Combine(projectRootPath, WINDOWS_PATH);
            case BuildTarget.Android:           //android 格式带有时间戳
                string timeStamp = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
                string fileName = $"TheChief-EyeOfCity-{ timeStamp}.apk";
                return Path.Combine(projectRootPath, "release/android", fileName);
            case BuildTarget.iOS:
                string iosProjectPath = "TheChief";
                return Path.Combine(projectRootPath, "release/ios", iosProjectPath);
        }

        return string.Empty;
    }

#if UNITY_2017
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        //PackBuildinShaders.PackAlwaysBuildinShaders();
    }
#elif UNITY_2018
    public void OnPreprocessBuild(BuildReport report)
    {

    }
#endif


    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projPath = UnityEditor.iOS.Xcode.Custom.PBXProject.GetPBXProjectPath(path);
            UnityEditor.iOS.Xcode.Custom.PBXProject pbxProject = new UnityEditor.iOS.Xcode.Custom.PBXProject();
            pbxProject.ReadFromString(File.ReadAllText(projPath));

            string target = pbxProject.TargetGuidByName("Unity-iPhone");

            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "false");
            pbxProject.AddBuildProperty(target, "OTHER_LDFLAGS", " -lxml2");

            File.WriteAllText(projPath, pbxProject.WriteToString());
        }

    }
}