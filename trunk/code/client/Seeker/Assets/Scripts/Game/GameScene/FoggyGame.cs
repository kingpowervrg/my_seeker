/********************************************************************
	created:  2018-4-19 11:56:51
	filename: FoggyGame.cs
	author:	  songguangze@outlook.com
	
	purpose:  迷雾模式
*********************************************************************/

using SeekerGame.Rendering;
using DG.Tweening;
namespace SeekerGame
{
    public class FoggyGame : GameSceneBase
    {
        private SceneBlur m_sceneBlurComponent = null;

        public FoggyGame(int sceneId) : base(SceneMode.FOGGY, sceneId)
        {

        }

        protected override void OnLoadedScene()
        {
            base.OnLoadedScene();
            CameraManager.Instance.InitCameraController<SpecialGameCamera>();
            this.m_sceneBlurComponent = CameraManager.Instance.ScreenBlurComponent;
            ScreenDrawer.instance.enabled = false;
            ScreenDrawer.instance.paintBrushMode = ScreenDrawer.PaintBrushMode.MULTIPLE;
            ScreenDrawer.instance.enabled = true;

            this.m_sceneBlurComponent.ReplacementMaskTexture = ScreenDrawer.instance.DrawingTexture;
            this.m_sceneBlurComponent.HandleBlurScene(true);

            GameEvents.MainGameEvents.EnableSceneSpecialEffect += EnableFoggyModeAnimator;
        }

        private void EnableFoggyMode(bool isEnable)
        {
            ScreenDrawer.instance.enabled = isEnable;
            if (isEnable)
                this.m_sceneBlurComponent.ReplacementMaskTexture = ScreenDrawer.instance.DrawingTexture;

            this.m_sceneBlurComponent.HandleBlurScene(isEnable);
        }

        public override void DestroyScene()
        {
            base.DestroyScene();

            GameEvents.MainGameEvents.EnableSceneSpecialEffect -= EnableFoggyModeAnimator;
        }

        private Tween tweener = null;
        private void EnableFoggyModeAnimator(bool isEnable)
        {
            float start = isEnable ? 0 : 1f;
            float end = isEnable ? 1f : 0f;
            if (tweener != null)
            {
                tweener.Kill();
            }
            if (isEnable)
            {
                EnableFoggyMode(isEnable);
            }
            tweener = DOTween.To(x => this.m_sceneBlurComponent.Factor = x, start, end, 0.4f).OnComplete(() =>
            {

                if (!isEnable)
                {
                    EnableFoggyMode(isEnable);
                }

            });
        }


    }
}