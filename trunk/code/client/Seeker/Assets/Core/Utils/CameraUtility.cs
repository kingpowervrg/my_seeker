/********************************************************************
	created:  2015-8-2  13:38:32
	filename: CameraUtility.cs
	author:	  songguangze@outlook.com
	
	purpose:  摄像机帮助类
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EngineCore.Utility
{
    public static class CameraUtility
    {
        /// <summary>
        /// GameObject是否在Camera内
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsGameObjectInCameraView(GameObject targetObj, Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;

            if (camera == null)
                return false;

            Vector3 targetObjViewportCoord = camera.WorldToViewportPoint(targetObj.transform.position);
            if (targetObjViewportCoord.x > 0 && targetObjViewportCoord.x < 1 && targetObjViewportCoord.y > 0f && targetObjViewportCoord.y < 1 && targetObjViewportCoord.z > camera.nearClipPlane && targetObjViewportCoord.z < camera.farClipPlane)
                return true;

            return false;
        }

        /// <summary>
        /// 指定AABB Bounds是否在Camera内
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsBoundsInCameraView(Bounds bounds, Camera camera = null)
        {
            if (bounds == default(Bounds))
            {
                return false;
            }
            if (camera == null)
                camera = Camera.main;

            Plane[] cameraFrustumPlanes = UnityEngine.GeometryUtility.CalculateFrustumPlanes(camera);
            bool isAABB =  UnityEngine.GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, bounds);
            
            return isAABB;
        }

        public static bool IsBoundsInCameraView(Bounds bound0,Bounds bound1,Camera camera = null)
        {
            if (bound1 == default(Bounds))
            {
                return IsBoundsInCameraView(bound0, camera);
            }
            else if (bound0 == default(Bounds))
            {
                return IsBoundsInCameraView(bound1, camera);
            }
            else
            {
                return IsBoundsInCameraView(bound0, camera) || IsBoundsInCameraView(bound1, camera);
            }
        }
        /// <summary>
        /// 世界坐标系转换到指定Canvas坐标系
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="worldCamera"></param>
        /// <param name="canvasRectTransform"></param>
        /// <param name="canvasCamera"></param>
        /// <returns></returns>
        public static Vector3 WorldPointInCanvasRectTransform(Vector3 worldPoint, Camera worldCamera, RectTransform canvasRectTransform, Camera canvasCamera = null)
        {
            if (canvasCamera == null)
                canvasCamera = worldCamera;
            
            Vector2 worldPointInScreenSpace = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPoint);

            Vector3 pointInCanvasSpace;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, worldPointInScreenSpace, canvasCamera, out pointInCanvasSpace);

            return pointInCanvasSpace;
        }

        public static Vector3 WorldPoint2CanvasRectTransform(Vector3 worldPoint, Camera world, RectTransform canas)
        {
            Vector3 screen_point = world.WorldToScreenPoint(worldPoint);
            Vector3 ret;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canas, new Vector2(screen_point.x, screen_point.y), FrameMgr.Instance.UICamera, out ret);

            return ret;
        }

        /// <summary>
        /// todo:方法有问题，慎用！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！稍后会改 guangze song 2018-6-29 15:14:58
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="uiGameObjectWithCanvas"></param>
        /// <returns></returns>
        public static Vector3 WorldPointInCanvasRectTransform(Vector3 worldPoint, GameObject uiGameObjectWithCanvas)
        {
            RectTransform canvasTransform = uiGameObjectWithCanvas.transform as RectTransform;
            Camera uiCamera = FrameMgr.Instance.UICamera;
            Camera worldCamera = Camera.main;

            Vector3 pointInCanvasSpace = WorldPointInCanvasRectTransform(worldPoint, worldCamera, canvasTransform, uiCamera);
            return pointInCanvasSpace;
        }

        public static Vector3 ScreenPointInCanvasRectTransform(Vector2 screenPoint, GameObject uiGameObject)
        {
            RectTransform canvasTransform = uiGameObject.transform as RectTransform;
            Camera uiCamera = FrameMgr.Instance.UICamera;

            Vector3 point;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasTransform, screenPoint, uiCamera, out point);

            return point;
        }

        public static Vector2 World2RectTrf( Vector3 world_pos_, Camera world_cam_, GameObject ui_canvas_, Camera ui_cam_)
        {
            Vector2 mouseDown = world_cam_.WorldToScreenPoint(world_pos_);
            Vector2 mouseUGUIPos = new Vector2();
            bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(ui_canvas_.transform as RectTransform, mouseDown, ui_cam_, out mouseUGUIPos);
            if (isRect)
            {
                return  mouseUGUIPos;
            }

            return Vector2.zero;
          
        }

        public static Vector3 UIPostionToWorldPos(Vector3 pos,Camera camera)
        {
            if (camera == null)
            {
                camera = Camera.main;
            }
            Camera uiCamera = FrameMgr.Instance.UICamera;
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera,pos);
            Debug.Log("screenPos : " + screenPos);
            screenPos.z = 25f;
            return camera.ScreenToWorldPoint(screenPos);
            //RectTransformUtility.sc
        }

        /// <summary>
        /// WorldPoint转换到指定Canvas本地坐标系(已经计算Canvas坐标系的缩放)
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="destCanvas"></param>
        /// <param name="canvasCamera"></param>
        /// <param name="worldCamera"></param>
        /// <returns></returns>
        public static Vector2 WorldPointToCanvasLocalPosition(Vector3 worldPoint, Canvas destCanvas, Camera canvasCamera = null, Camera worldCamera = null)
        {
            worldCamera = worldCamera ?? Camera.main;
            canvasCamera = canvasCamera ?? FrameMgr.Instance.UICamera;

            Vector2 pointInScreenSpace = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPoint);
            Vector2 pointInCanvasSpace;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(destCanvas.transform as RectTransform, pointInScreenSpace, canvasCamera, out pointInCanvasSpace);

            return pointInCanvasSpace;
        }

        public static Vector2 ScreenPointInLocalRectTransform(Vector2 screenPoint, GameObject uiGameObject, RectTransform parentRectTransform = null)
        {
            if (parentRectTransform == null)
                parentRectTransform = uiGameObject.transform.parent as RectTransform;

            Camera uiCamera = FrameMgr.Instance.UICamera;

            Vector2 pointInCanvasPosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPoint, uiCamera, out pointInCanvasPosition);

            return pointInCanvasPosition;
        }

        public static Vector2 ScreenPointInRootCanvasPoint(Vector2 screenPoint, GameObject uiGameObject)
        {
            Canvas rootCanvas = uiGameObject.GetComponentInParent<Canvas>();

            RectTransform canvasRectTransform = rootCanvas.transform as RectTransform;
            Camera uiCamera = FrameMgr.Instance.UICamera;

            Vector2 pointInCanvasPosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, uiCamera, out pointInCanvasPosition);

            return pointInCanvasPosition;
        }


        /// <summary>
        /// 对象是否在Camera内
        /// </summary>
        /// <param name="targetObj"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsInCameraView(GameObject targetObj, Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;
            if (camera == null) return false;

            Vector3 targetObjViewportCoord = camera.WorldToViewportPoint(targetObj.transform.position);
            if (targetObjViewportCoord.x > 0 && targetObjViewportCoord.x < 1 && targetObjViewportCoord.y > -0.5f && targetObjViewportCoord.y < 1 && targetObjViewportCoord.z > camera.nearClipPlane && targetObjViewportCoord.z < camera.farClipPlane)
                return true;

            return false;
        }
    }
}