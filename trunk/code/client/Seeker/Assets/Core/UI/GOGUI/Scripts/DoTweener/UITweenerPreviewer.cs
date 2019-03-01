#if RES_PROJECT && UNITY_EDITOR
/********************************************************************
	created:  2019-1-14 19:17:11
	filename: UITweenerPreviewer.cs
	author:	  songguangze@outlook.com
	
	purpose:  基于DoTween的UI动画组件，预览类，提供自定义的TriggerType的预览 
*********************************************************************/
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EngineCore
{
    public class UITweenerPreviewer : MonoBehaviour
    {
        private static GameObject previewRootObject = null;
        private static UITweenerBase[] uiTweeners = null;

        private void PreviewTweenerOnShow()
        {
            RebuildTweeners();
            if (uiTweeners != null)
                uiTweeners.Where(uiTweener => uiTweener.m_triggerType == UITweenerBase.TweenTriggerType.OnShow && uiTweener.enabled).ForEach(uiTweener => uiTweener.ResetAndPlay(true));
        }

        private void PreviewTweenerOnHide()
        {
            RebuildTweeners();
            if (uiTweeners != null)
                uiTweeners.Where(uiTweener => uiTweener.m_triggerType == UITweenerBase.TweenTriggerType.OnHide && uiTweener.enabled).ForEach(uiTweener => uiTweener.ResetAndPlay(true));
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitEditorPreviewer()
        {
            Camera.main.gameObject.GetOrAddComponent<UITweenerPreviewer>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Comma))
                PreviewTweenerOnShow();

            if (Input.GetKeyUp(KeyCode.Period))
                PreviewTweenerOnHide();
        }

        private void RebuildTweeners()
        {
            if (previewRootObject != Selection.activeGameObject)
            {
                previewRootObject = Selection.activeGameObject;
                uiTweeners = previewRootObject.GetComponentsInChildren<UITweenerBase>();
            }
        }
    }
}
#endif