using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GOGUI
{
    public class UIUpdatePositionFromWorld : MonoBehaviour
    {
        public bool ManualUpdate = false;

        private Vector3 mWorldPosition = Vector3.zero;
        private RectTransform mCachedTrans;
        Transform parent;
        Transform rootTransform;
        Canvas canvas;
        private Camera mUICamera;
        float lastTime;

        public Vector3 WorldPosition
        {
            get { return mWorldPosition; }
            set { mWorldPosition = value; }
        }
        void Start()
        {            
            mCachedTrans = GetComponent<RectTransform>();
            rootTransform = mCachedTrans.parent.parent;
            canvas = mCachedTrans.parent.gameObject.GetComponent<Canvas>();
        }


        void Update()
        {
            if (!mCachedTrans.parent)
                return;
            if (!ManualUpdate)
            {
                rootTransform.position = Vector3.zero;
                rootTransform.rotation = Quaternion.identity;
                parent = rootTransform.parent;
                if (!parent)
                    return;
                mWorldPosition = parent.position;
            }

            if (mUICamera == null)
            {
                mUICamera = GOGUITools.UICamera;
                canvas.worldCamera = mUICamera;
                //GOEngine.GameObjectUtil.SetLayer(rootTransform.gameObject, LayerDef.UI /*GOEngine.RenderLayerDef.RL_UI*/,true);
            }

            if ((mUICamera != null) && (Camera.main != null))
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(mWorldPosition);
                //Vector3 uiWorldPos = mUICamera.ScreenToWorldPoint(screenPos);
                Vector3 pos = new Vector3(screenPos.x, screenPos.y, 0) / canvas.scaleFactor;
                mCachedTrans.anchoredPosition3D = pos;
            }
        }
    }
}