using System;
using System.Collections.Generic;
using EngineCore;

namespace SeekerGame.NewGuid
{
    public class GuidNewFuncSceneFingerTip : GuidNewFunctionBase
    {
        public override void OnExecute()
        {
            base.OnExecute();
            List<SceneItemEntity> sceneItem = GameEvents.MainGameEvents.GetSceneItemEntityList.SafeInvoke(1);
            UnityEngine.Debug.Log("sceneItem   " + sceneItem.Count);
            if (sceneItem.Count > 0 )
            {
                GameEvents.SceneEvents.OnSceneExhibitTipsGuide.SafeInvoke(sceneItem[0], true);
            }
            GameEvents.MainGameEvents.OnPickedSceneObject += OnPickedSceneItem;
        }

        private void OnPickedSceneItem(SceneItemEntity sceneItemEntity)
        {
            GameEvents.MainGameEvents.OnPickedSceneObject -= OnPickedSceneItem;
            OnDestory();
        }

        public override void OnDestory(FuncState funcState = FuncState.Complete)
        {
            base.OnDestory(funcState);
        }

        public override void ResetFunc(bool isRetainFunc = true)
        {
            base.ResetFunc(isRetainFunc);
            GameEvents.MainGameEvents.OnPickedSceneObject -= OnPickedSceneItem;
        }
    }
}
