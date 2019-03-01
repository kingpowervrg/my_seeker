using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WindowDialog : MonoBehaviour {

    [MenuItem("Tools/DialogTest")]
    public static void Dialog()
    {
        bool flag = UnityEditor.EditorUtility.DisplayDialog("Title", "Content", "是", "否");
    }
}
