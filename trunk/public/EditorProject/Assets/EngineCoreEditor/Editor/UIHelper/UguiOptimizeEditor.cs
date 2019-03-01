using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UguiOptimizeEditor : MonoBehaviour
{
    [MenuItem("GameObject/UI/Image")]
    public static void CreatImage()
    {
        GameObject imageObj = new GameObject("Image", typeof(Image));
        imageObj.GetComponent<Image>().raycastTarget = false;
        setParent(imageObj);
    }

    [MenuItem("GameObject/UI/Text")]
    public static void CreatText()
    {
        GameObject textObj = new GameObject("Text", typeof(Text));
        Text text = textObj.GetComponent<Text>();
        text.raycastTarget = false;
        text.supportRichText = false;
        text.text = "New Text";
        text.color = Color.black;
        setParent(textObj);
    }

    private static void setParent(GameObject obj)
    {
        Transform objTransform = obj.transform;
        objTransform.SetParent(Selection.activeTransform);
        objTransform.localPosition = Vector2.zero;
        objTransform.localScale = Vector2.one;
    }

   

}