using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(UnityEngine.UI.CanvasScaler))]
public class UIFitScaler : MonoBehaviour
{

    static float curAspect = -1;
    void Awake()
    {

        if (!Application.isPlaying)
            return;

        if (curAspect == -1)
        {
//#if UNITY_EDITOR
            Camera camera = Camera.main;
            if (camera == null)
            {
                camera = GameObject.FindObjectOfType<Camera>();
            }
            //Vector2 screenSize = BoxBlurSprite.Editor_GetScreenPixelDimensions(camera);
            //            curAspect = screenSize.x / screenSize.y;
            //#else
            //            curAspect = (float)Screen.width / Screen.height;
            //#endif
            curAspect = (float)Screen.width / Screen.height;
            UnityEngine.UI.CanvasScaler scaler = this.GetComponent<UnityEngine.UI.CanvasScaler>();
            Vector2 referenceResolution = scaler.referenceResolution;
            float referenceAspect = referenceResolution.x / referenceResolution.y;
            if (curAspect > referenceAspect)
            {
                scaler.matchWidthOrHeight = 1.0f;
            }
            else
            {
                scaler.matchWidthOrHeight = 0.0f;
            }
        }
    }
#if UNITY_EDITOR
    void Update()
    {
        return;
        //Camera camera = Camera.main;
        //if (camera == null)
        //{
        //    camera = GameObject.FindObjectOfType<Camera>();
        //}
        //Vector2 screenSize = BoxBlurSprite.Editor_GetScreenPixelDimensions(camera);
        //curAspect = screenSize.x / screenSize.y;


        //UnityEngine.UI.CanvasScaler scaler = this.GetComponent<UnityEngine.UI.CanvasScaler>();
        //Vector2 referenceResolution = scaler.referenceResolution;
        //float referenceAspect = referenceResolution.x / referenceResolution.y;
        //if (curAspect > referenceAspect)
        //{
        //    scaler.matchWidthOrHeight = 1.0f;
        //}
        //else
        //{
        //    scaler.matchWidthOrHeight = 0.0f;
        //}

    }
#endif
}
