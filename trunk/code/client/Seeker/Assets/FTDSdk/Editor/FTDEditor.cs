using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class FTDEditor : IPreprocessBuild
{
    private static bool isPostProcessingEnabled = true;

    public int callbackOrder
    {
        get { return 0; }
    }

    [MenuItem("Assets/FTDSdk/Check post processing status")]
    public static void CheckPostProcessingPermission()
    {
        EditorUtility.DisplayDialog("ftd", "The post processing for FTDSdk is " + (isPostProcessingEnabled ? "enabled." : "disabled."), "OK");
    }

    [MenuItem("Assets/FTDSdk/Change post processing status")]
    public static void ChangePostProcessingPermission()
    {
        isPostProcessingEnabled = !isPostProcessingEnabled;
        EditorUtility.DisplayDialog("ftd", "The post processing for FTDSdk is now " + (isPostProcessingEnabled ? "enabled." : "disabled."), "OK");
    }
    [PostProcessBuild(100)]
    public static void OnPostprocessBuild(BuildTarget target, string projectPath)
    {
        UnityEngine.Debug.Log("FTD: OnPostprocessBuild  projectPath = " + projectPath);
        if (target == BuildTarget.iOS)
        {
            RunPostProcessTasksiOS(projectPath);
        }

    }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.Android)
        {
            UnityEngine.Debug.Log("FTD: OnPreprocessBuild  projectPath = " + path);

            // Check what is user setting about allowing ftd SDK to perform post build tasks.
            // If user disabled it, oh well, we won't do a thing.
            if (!isPostProcessingEnabled)
            {
                UnityEngine.Debug.Log("FTD: You have forbidden the FTD SDK to perform post processing tasks.");
                UnityEngine.Debug.Log("FTD: Skipping post processing tasks.");
                return;
            }

            UnityEngine.Debug.Log("FTD: Starting to perform post build tasks for Android platform.");
            RunPostProcessTasksAndroid();
        }
    }

    private static void RunPostProcessTasksiOS(string projectPath)
    {
#if UNITY_IOS
        string jsonPath = Path.Combine(Application.dataPath, "FTDSdk/IOS/FAT_sta.json");
        string jsonoutPath = projectPath + "/Libraries/FTDSdk/IOS/FAT_sta.json";

        UnityEngine.Debug.Log("FTD: IOS SDK Application.dataPath ： " + Application.dataPath);
        UnityEngine.Debug.Log("FTD: IOS SDK json path ： " + jsonPath);
        UnityEngine.Debug.Log("FTD: IOS SDK json copy to path ： " + jsonoutPath);

        File.Copy(jsonPath, jsonoutPath, true);

        UnityEngine.Debug.Log("FTD: Starting to perform post build tasks for iOS platform.");

        string xcodeProjectPath = projectPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

        PBXProject xcodeProject = new PBXProject();
        xcodeProject.ReadFromFile(xcodeProjectPath);

        string xcodeTarget = xcodeProject.TargetGuidByName("Unity-iPhone");
        string jsonprojectpath = "/Libraries/FTDSdk/IOS/FAT_sta.json";
        string fileGuid = xcodeProject.AddFile(jsonoutPath, jsonprojectpath, PBXSourceTree.Source);
        xcodeProject.AddFileToBuild(xcodeTarget, fileGuid);

        // The ftd SDK needs two frameworks to be added to the project:
        // - AdSupport.framework
        // - iAd.framework
        UnityEngine.Debug.Log("FTD: Adding AdSupport.framework to Xcode project.");
        xcodeProject.AddFrameworkToProject(xcodeTarget, "AdSupport.framework", true);
        UnityEngine.Debug.Log("FTD: AdSupport.framework added successfully.");

        UnityEngine.Debug.Log("FTD: Adding iAd.framework to Xcode project.");
        xcodeProject.AddFrameworkToProject(xcodeTarget, "iAd.framework", true);
        UnityEngine.Debug.Log("FTD: iAd.framework added successfully.");

        UnityEngine.Debug.Log("FTD: Adding CoreTelephony.framework to Xcode project.");
        xcodeProject.AddFrameworkToProject(xcodeTarget, "CoreTelephony.framework", true);
        UnityEngine.Debug.Log("FTD: CoreTelephony.framework added successfully.");

        // The ftd SDK needs to have Obj-C exceptions enabled.
        // GCC_ENABLE_OBJC_EXCEPTIONS=YES

        UnityEngine.Debug.Log("FTD: Enabling Obj-C exceptions by setting GCC_ENABLE_OBJC_EXCEPTIONS value to YES.");
        xcodeProject.AddBuildProperty(xcodeTarget, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");

        UnityEngine.Debug.Log("FTD: Obj-C exceptions enabled successfully.");

        // The ftd SDK needs to have -ObjC flag set in other linker flags section because of it's categories.
        // OTHER_LDFLAGS -ObjC

        UnityEngine.Debug.Log("FTD: Adding -ObjC flag to other linker flags (OTHER_LDFLAGS).");
        xcodeProject.AddBuildProperty(xcodeTarget, "OTHER_LDFLAGS", "-ObjC");

        UnityEngine.Debug.Log("FTD: -ObjC successfully added to other linker flags.");

        // Save the changes to Xcode project file.
        xcodeProject.WriteToFile(xcodeProjectPath);

#endif
    }

    private static void RunPostProcessTasksAndroid()
    {
        bool isFTDManifestUsed = false;
        string androidPluginsPath = Path.Combine(Application.dataPath, "Plugins/Android");
        string ftdManifestPath = Path.Combine(Application.dataPath, "FTDSdk/Android/FTDSdkAndroidManifest.xml");

        string appManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");

        // Check if user has already created AndroidManifest.xml file in its location.
        // If not, use already predefined FTDAndroidManifest.xml as default one.
        if (!File.Exists(appManifestPath))
        {
            if (!Directory.Exists(androidPluginsPath))
            {
                Directory.CreateDirectory(androidPluginsPath);
            }

            isFTDManifestUsed = true;
            File.Copy(ftdManifestPath, appManifestPath);

            UnityEngine.Debug.Log("FTD: User defined AndroidManifest.xml file not found in Plugins/Android folder.");
            UnityEngine.Debug.Log("FTD: Creating default app's AndroidManifest.xml from FTDAndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FTD: User defined AndroidManifest.xml file located in Plugins/Android folder.");
        }

        // If ftd manifest is used, we have already set up everything in it so that 
        // our native Android SDK can be used properly.
        if (!isFTDManifestUsed)
        {
            // However, if you already had your own AndroidManifest.xml, we'll now run
            // some checks on it and tweak it a bit if needed to add some stuff which
            // our native Android SDK needs so that it can run properly.

            // Let's open the app's AndroidManifest.xml file.
            XmlDocument manifestFile = new XmlDocument();
            manifestFile.Load(appManifestPath);

            // Add needed permissions if they are missing.
            AddPermissions(manifestFile);

            // Add intent filter to main activity if it is missing.
            AddBroadcastReceiver(manifestFile);

            // Save the changes.
            manifestFile.Save(appManifestPath);

            // Clean the manifest file.
            CleanManifestFile(appManifestPath);

            UnityEngine.Debug.Log("FTD: App's AndroidManifest.xml file check and potential modification completed.");
            UnityEngine.Debug.Log("FTD: Please check if any error message was displayed during this process "
                + "and make sure to fix all issues in order to properly use the ftd SDK in your app.");
        }
    }

    private static void AddPermissions(XmlDocument manifest)
    {
        // The ftd SDK needs two permissions to be added to you app's manifest file:
        // <uses-permission android:name="android.permission.INTERNET" />
        // <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
        // <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
        // <uses-permission android:name="com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE" />

        UnityEngine.Debug.Log("FTD: Checking if all permissions needed for the ftd SDK are present in the app's AndroidManifest.xml file.");

        bool hasInternetPermission = false;
        bool hasAccessWifiStatePermission = false;
        bool hasAccessNetworkStatePermission = false;
        bool hasInstallReferrerServicePermission = false;

        XmlElement manifestRoot = manifest.DocumentElement;

        // Check if permissions are already there.
        foreach (XmlNode node in manifestRoot.ChildNodes)
        {
            if (node.Name == "uses-permission")
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.Value.Contains("android.permission.INTERNET"))
                    {
                        hasInternetPermission = true;
                    }
                    else if (attribute.Value.Contains("android.permission.ACCESS_WIFI_STATE"))
                    {
                        hasAccessWifiStatePermission = true;
                    }
                    else if (attribute.Value.Contains("android.permission.ACCESS_NETWORK_STATE"))
                    {
                        hasAccessNetworkStatePermission = true;
                    }
                    else if (attribute.Value.Contains("com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE"))
                    {
                        hasInstallReferrerServicePermission = true;
                    }
                }
            }
        }

        // If android.permission.INTERNET permission is missing, add it.
        if (!hasInternetPermission)
        {
            XmlElement element = manifest.CreateElement("uses-permission");
            element.SetAttribute("android__name", "android.permission.INTERNET");
            manifestRoot.AppendChild(element);
            UnityEngine.Debug.Log("FTD: android.permission.INTERNET permission successfully added to your app's AndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FTD: Your app's AndroidManifest.xml file already contains android.permission.INTERNET permission.");
        }

        // If android.permission.ACCESS_WIFI_STATE permission is missing, add it.
        if (!hasAccessWifiStatePermission)
        {
            XmlElement element = manifest.CreateElement("uses-permission");
            element.SetAttribute("android__name", "android.permission.ACCESS_WIFI_STATE");
            manifestRoot.AppendChild(element);
            UnityEngine.Debug.Log("FTD: android.permission.ACCESS_WIFI_STATE permission successfully added to your app's AndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FTD: Your app's AndroidManifest.xml file already contains android.permission.ACCESS_WIFI_STATE permission.");
        }

        // If android.permission.ACCESS_NETWORK_STATE permission is missing, add it.
        if (!hasAccessNetworkStatePermission)
        {
            XmlElement element = manifest.CreateElement("uses-permission");
            element.SetAttribute("android__name", "android.permission.ACCESS_NETWORK_STATE");
            manifestRoot.AppendChild(element);
            UnityEngine.Debug.Log("FTD: android.permission.ACCESS_NETWORK_STATE permission successfully added to your app's AndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FTD: Your app's AndroidManifest.xml file already contains android.permission.ACCESS_NETWORK_STATE permission.");
        }

        // If com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE permission is missing, add it.
        if (!hasInstallReferrerServicePermission)
        {
            XmlElement element = manifest.CreateElement("uses-permission");
            element.SetAttribute("android__name", "com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE");
            manifestRoot.AppendChild(element);
            UnityEngine.Debug.Log("FTD: com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE permission successfully added to your app's AndroidManifest.xml file.");
        }
        else
        {
            UnityEngine.Debug.Log("FTD: Your app's AndroidManifest.xml file already contains com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE permission.");
        }
    }

    private static void AddBroadcastReceiver(XmlDocument manifest)
    {
        // We're looking for existance of broadcast receiver in the AndroidManifest.xml
        // Check out the example below how that usually looks like:

        // <manifest
        //     <!-- ... -->>
        // 
        //     <supports-screens
        //         <!-- ... -->/>
        // 
        //     <application
        //         <!-- ... -->>
        //      <!--FTDReferrerReceiver 设置是为了使三方归因数据更为准确-->
        //      <receiver android:name="com.ftdsdk.www.receiver.FTDReferrerReceiver" android:exported="true">
        //              <intent-filter>
        //                  <action android:name="com.android.vending.INSTALL_REFERRER" />
        //              </intent-filter>
        //      </receiver>
        //         
        //         <activity android:name="com.unity3d.player.UnityPlayerActivity"
        //             <!-- ... -->
        //         </activity>
        //     </application>
        // 
        //     <!-- ... -->>
        //
        // </manifest>

        UnityEngine.Debug.Log("FTD: Checking if app's AndroidManifest.xml file contains receiver for INSTALL_REFERRER intent.");

        XmlElement manifestRoot = manifest.DocumentElement;
        XmlNode applicationNode = null;

        // Let's find the application node.
        foreach (XmlNode node in manifestRoot.ChildNodes)
        {
            if (node.Name == "application")
            {
                applicationNode = node;
                break;
            }
        }

        // If there's no applicatio node, something is really wrong with your AndroidManifest.xml.
        if (applicationNode == null)
        {
            UnityEngine.Debug.LogError("FTD: Your app's AndroidManifest.xml file does not contain \"<application>\" node.");
            UnityEngine.Debug.LogError("FTD: Unable to add the ftd broadcast receiver to AndroidManifest.xml.");
            return;
        }

        // Okay, there's an application node in the AndroidManifest.xml file.
        // Let's now check if user has already defined a receiver which is listening to INSTALL_REFERRER intent.
        // If that is already defined, don't force the ftd broadcast receiver to the manifest file.
        // If not, add the ftd broadcast receiver to the manifest file.
        bool isThereAnyCustomBroadcastReiver = false;

        foreach (XmlNode node in applicationNode.ChildNodes)
        {
            if (node.Name == "receiver")
            {
                foreach (XmlNode subnode in node.ChildNodes)
                {
                    if (subnode.Name == "intent-filter")
                    {
                        foreach (XmlNode subsubnode in subnode.ChildNodes)
                        {
                            if (subsubnode.Name == "action")
                            {
                                foreach (XmlAttribute attribute in subsubnode.Attributes)
                                {
                                    if (attribute.Value.Contains("INSTALL_REFERRER"))
                                    {
                                        isThereAnyCustomBroadcastReiver = true;
                                        break;
                                    }
                                }
                            }

                            if (isThereAnyCustomBroadcastReiver)
                            {
                                break;
                            }
                        }
                    }

                    if (isThereAnyCustomBroadcastReiver)
                    {
                        break;
                    }
                }
            }

            if (isThereAnyCustomBroadcastReiver)
            {
                break;
            }
        }

        // Let's see what we have found so far.
        if (isThereAnyCustomBroadcastReiver)
        {
            UnityEngine.Debug.Log("FTD: It seems like you are using your own broadcast receiver.");
            UnityEngine.Debug.Log("FTD: Please, add the calls to the ftd broadcast receiver like described in here: https://confluence.ftsview.com/pages/viewpage.action?pageId=2818100#BISDK-Unity%E6%8E%A5%E5%85%A5%E6%8C%87%E5%8D%97-%E5%A6%82%E4%BD%95%E6%8E%A5%E5%85%A5");
        }
        else
        {
            // Generate ftd broadcast receiver entry and add it to the application node.
            XmlElement receiverElement = manifest.CreateElement("receiver");
            receiverElement.SetAttribute("android__name", "com.ftdsdk.www.receiver.FTDReferrerReceiver");
            receiverElement.SetAttribute("android__permission", "android.permission.INSTALL_PACKAGES");
            receiverElement.SetAttribute("android__exported", "true");

            XmlElement intentFilterElement = manifest.CreateElement("intent-filter");
            XmlElement actionElement = manifest.CreateElement("action");
            actionElement.SetAttribute("android__name", "com.android.vending.INSTALL_REFERRER");

            intentFilterElement.AppendChild(actionElement);
            receiverElement.AppendChild(intentFilterElement);
            applicationNode.AppendChild(receiverElement);

            UnityEngine.Debug.Log("FTD: FTD broadcast receiver successfully added to your app's AndroidManifest.xml file.");
        }
    }

    private static void CleanManifestFile(String manifestPath)
    {
        // Due to XML writing issue with XmlElement methods which are unable
        // to write "android:[param]" string, we have wrote "android__[param]" string instead.
        // Now make the replacement: "android:[param]" -> "android__[param]"

        TextReader manifestReader = new StreamReader(manifestPath);
        string manifestContent = manifestReader.ReadToEnd();
        manifestReader.Close();

        Regex regex = new Regex("android__");
        manifestContent = regex.Replace(manifestContent, "android:");

        TextWriter manifestWriter = new StreamWriter(manifestPath);
        manifestWriter.Write(manifestContent);
        manifestWriter.Close();
    }

    private static int GetUnityIdeVersion()
    {
        int unityVersion;
        Int32.TryParse(Application.unityVersion[0].ToString(), out unityVersion);

        return unityVersion;
    }
}
