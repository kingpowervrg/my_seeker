

#if RES_PROJECT && UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EngineCore
{
    public class DoPahtPreviewer : MonoBehaviour
    {
        private static GameObject previewRootObject = null;
        private static FlyIconBezierPath uiTweeners = null;

        //private void PreviewTweenerOnShow()
        //{
        //    RebuildTweeners();
        //}

        //private void PreviewTweenerOnHide()
        //{
        //    RebuildTweeners();
        //}
        private void PreviewTweenerOnDo()
        {
            RebuildTweeners();

            if (null != uiTweeners)
            {
                uiTweeners.RefreshMoveInEditor();
            }
        }

        private void PreviewTweenerOnReset()
        {
            RebuildTweeners();

            if (null != uiTweeners)
            {
                uiTweeners.ResetMoveInEditor();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitEditorPreviewer()
        {
            Camera.main.gameObject.GetOrAddComponent<DoPahtPreviewer>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Comma))
                PreviewTweenerOnReset();

            if (Input.GetKeyDown(KeyCode.Period))
                PreviewTweenerOnDo();
        }

        private void RebuildTweeners()
        {
            if (previewRootObject != Selection.activeGameObject)
            {
                previewRootObject = Selection.activeGameObject;

                uiTweeners = previewRootObject.GetComponent<FlyIconBezierPath>();
            }
        }
    }
}
#endif