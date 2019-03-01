using EngineCore;
using GOEngine;

namespace SeekerGame
{ 
    public class NormalTalkScene : SceneBase
    {
        public NormalTalkScene() : base(SceneMode.NORMALTALK)
        {

        }

        public void InitScene(string sceneName)
        {
            //ConfScene scenebase = ConfScene.Get(sceneId);
            //if (scenebase == null)
            //{
            //    DebugUtil.LogErrorFormat("sceneid is not exits : {0}",sceneId);
            //}
            LoadScene(sceneName);
        }

        /// <summary>
        /// 请求载入场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName)
        {
            EngineCore.EngineCoreEvents.ResourceEvent.LoadAdditiveScene.SafeInvoke(sceneName, OnLoadedScene);
        }

        /// <summary>
        /// 载入场景完成
        /// </summary>
        protected virtual void OnLoadedScene()
        {
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(1,true);
            //GameEvents.UIEvents.UI_Loading_Event.OnSceneAssetLoadOver.SafeInvoke();
            CameraManager.Instance.ResetMainCamera();
            GameEvents.SceneEvents.OnEnterScene.SafeInvoke();
        }
    }
}
