using PostFX;
using SeekerGame.Rendering;
using DG.Tweening;
namespace SeekerGame
{
    public class DarkerGame : GameSceneBase
    {
        private DarkSceneWithMask m_darkSceneBehaviour = null;

        public DarkerGame(int sceneId) : base(SceneMode.DARKER, sceneId)
        {

        }

        protected override void OnLoadedScene()
        {
            base.OnLoadedScene();

            CameraManager.Instance.InitCameraController<SpecialGameCamera>();

            m_darkSceneBehaviour = CameraManager.Instance.GetPostFX<DarkSceneWithMask>(EffectType.DarkSceneWithMask);
            m_darkSceneBehaviour.factor = 1f;
            m_darkSceneBehaviour.IsApply = true;

            ScreenDrawer.instance.enabled = false;
            ScreenDrawer.instance.paintBrushMode = ScreenDrawer.PaintBrushMode.SINGLE;
            ScreenDrawer.instance.enabled = true;
            m_darkSceneBehaviour.maskTex = ScreenDrawer.instance.DrawingTexture;

            GameEvents.MainGameEvents.EnableSceneSpecialEffect += EnableDarkModeAnimator;
        }

        public override void DestroyScene()
        {
            base.DestroyScene();
            if (tweener != null)
            {
                tweener.Kill();
            }
            GameEvents.MainGameEvents.EnableSceneSpecialEffect -= EnableDarkModeAnimator;
        }



        Tween tweener = null;
        private void EnableDarkModeAnimator(bool isEnable)
        {
            float start = isEnable ? 0 : 1f;
            float end = isEnable ? 1f : 0f;
            if (tweener != null)
            {
                tweener.Kill();
            }
            if (isEnable)
            {
                EnableDarkMode(isEnable);
            }
            tweener = DOTween.To(x => m_darkSceneBehaviour.factor = x, start, end, 0.4f).OnComplete(()=> {

                if (!isEnable)
                {
                    EnableDarkMode(isEnable);
                }
                
            });
        }
        private void EnableDarkMode(bool isEnable)
        {
            ScreenDrawer.instance.enabled = isEnable;
            if (isEnable)
                m_darkSceneBehaviour.maskTex = ScreenDrawer.instance.DrawingTexture;
        }

    }
}