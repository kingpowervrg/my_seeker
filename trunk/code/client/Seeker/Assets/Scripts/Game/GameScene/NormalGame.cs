/********************************************************************
	created:  2018-4-10 17:44:44
	filename: NormalGame.cs
	author:	  songguangze@outlook.com
	
	purpose:  标准寻物玩法
*********************************************************************/


namespace SeekerGame
{
    public class NormalGame : GameSceneBase
    {
        public NormalGame(int sceneId) : base(SceneMode.NORMALGAME, sceneId)
        {

        }

        protected override void OnLoadedScene()
        {
            base.OnLoadedScene();

            CameraManager.Instance.InitCameraController<GameCameraNew>();
        }
    }
}