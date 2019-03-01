/********************************************************************
	created:  2014-3-2 13:26:16
	filename: GameObjectUtil.cs
	author:	  songguangze@outlook.com
	
	purpose:  各种GameObject操作的帮助函数
*********************************************************************/
using EngineCore;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EngineCore
{
    /// <summary>
    /// GameObject Util
    /// </summary>
    public static class GameObjectUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectByName(GameObject obj, string name)
        {
            Transform root = obj.transform;

            return GetGameObjectByNameSub(root, name);
        }

        static GameObject GetGameObjectByNameSub(Transform t, string name)
        {
            if (t.name == name)
                return t.gameObject;
            int cnt = t.childCount;
            for (int i = 0; i < cnt; i++)
            {
                var res = GetGameObjectByNameSub(t.GetChild(i), name);
                if (res)
                    return res;
            }
            return null;
        }

        /// <summary>
        /// 设置Layer
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        /// <param name="enforceSet"></param>
        static public void SetLayer(GameObject go, int layer, bool enforceSet = false)
        {
            SetLayerSub(go.transform, layer, enforceSet);
        }

        static void SetLayerSub(Transform tran, int layer, bool enforceSet)
        {
            impGameObjectlayer(tran.gameObject, layer, enforceSet);
            int cnt = tran.childCount;
            for (int i = 0; i < cnt; i++)
            {
                SetLayerSub(tran.GetChild(i), layer, enforceSet);
            }
        }

        private static void impGameObjectlayer(GameObject obj, int layer, bool force)
        {
            if (!force)
            {
                //int oldLayer = obj.layer;
                //if (oldLayer == LayerDef.LIGHT_FACE)
                //    return;
                //if (oldLayer == LayerDef.Effect)
                //    return;
                //if (oldLayer == LayerDef.Shadow_Light_Face)
                //    return;
                //if (oldLayer == LayerDef.FakeShadow)
                //    return;
            }
            obj.layer = layer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform GetBindPoint(GameObject obj, string name)
        {
            if (name == "self")
            {
                return obj.transform;
            }

            var res = GetGameObjectByName(obj, name);
            if (res)
                return res.transform;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        static public Bounds GetGameObjectBounds(GameObject obj)
        {
            Bounds bd = new Bounds(Vector3.zero, Vector3.zero);
            foreach (Renderer r in obj.GetComponentsInChildren<Renderer>(true))
            {
                if (r == null)
                {
                    continue;
                }
                if (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer))
                {
                    continue;
                }

                Bounds rbd = r.bounds;
                if (MathUtil.IsBoundsInvalide(bd))
                {
                    bd = rbd;
                }
                else
                {
                    bd.Encapsulate(rbd);
                }
            }

            bd.center = bd.center - obj.transform.position;

            return bd;
        }

        /// <summary>
        /// 获取或添加Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : MonoBehaviour
        {
            T component = obj.GetComponent<T>();
            if (component == null)
                component = obj.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// 获取或添加GameObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="findName"></param>
        /// <returns></returns>
        public static GameObject FindOrAddGameobject(this GameObject obj, string findName)
        {
            GameObject targetObject = obj.transform.Find(findName).gameObject;
            if (targetObject == null)
            {
                targetObject = new GameObject(findName);
                targetObject.transform.SetParent(obj.transform);
                targetObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            return targetObject;
        }

        /// <summary>
        /// 设置场景激活状态
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="isActive"></param>
        public static void SetSceneActive(string sceneName, bool isActive)
        {
            int currentLoadedSceneCount = SceneManager.sceneCount;
            for (int i = 0; i < currentLoadedSceneCount; ++i)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name == sceneName)
                {
                    GameObject[] sceneRootObjects = loadedScene.GetRootGameObjects();
                    SetGameObjectsActive(sceneRootObjects, isActive);

                    break;
                }

            }
        }

        /// <summary>
        /// 设置GameObjects的激活状态
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="isActive"></param>
        public static void SetGameObjectsActive(GameObject[] gameObjects, bool isActive)
        {
            for (int j = 0; j < gameObjects.Length; ++j)
                gameObjects[j].SetActive(isActive);
        }
        /// <summary>
        /// 获取Transform完整路径
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static string GetTransformFullPath(this Transform transform)
        {
            string path = string.Empty;
            GetTransformFullPath(transform, ref path);

            if (path.EndsWithFast("/"))
                path = path.Remove(path.Length - 1, 1);

            return path;
        }

        /// <summary>
        /// 获取Transform完整路径
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="transformFullPath"></param>
        public static void GetTransformFullPath(Transform transform, ref string transformFullPath)
        {
            if (transform != null)
            {
                transformFullPath = $"{transform.name}/{transformFullPath}";
                GetTransformFullPath(transform.parent, ref transformFullPath);
            }
            else
            {
                if (transformFullPath.EndsWithFast("/"))
                    transformFullPath = transformFullPath.Remove(transformFullPath.Length - 1, 1);
            }
        }

    }
}
