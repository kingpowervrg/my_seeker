using EngineCore;
using GOEngine;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace SeekerGame
{
    public class ExhititionHallScene : SceneBase
    {
        bool m_is_entity_collection_created;
        Dictionary<long, Vector3> m_entity_positions = new Dictionary<long, Vector3>();
        Dictionary<long, Quaternion> m_entity_rotates = new Dictionary<long, Quaternion>();
        public ExhititionHallScene() : base(SceneMode.EXHITITIONHALL)
        {

        }

        public void InitScene()
        {
            LoadScene("ShouCangShi_01");
            LoadSceneObjects();
        }

        public override void DestroyScene()
        {
            base.DestroyScene();


            EngineCoreEvents.InputEvent.OnOneFingerTouchup -= OnTouchScreen;
            EngineCoreEvents.InputEvent.OnPinIn -= ZoomIn;
            EngineCoreEvents.InputEvent.OnPinOut -= ZoomOut;
            EngineCoreEvents.InputEvent.OnSwipe -= Swipe;
            //GameEvents.SceneEvents.Listen_GetExhibitPos -= GetEntityPos;
            //GameEvents.SceneEvents.Listen_GetExhibitRotate -= GetEntityRotate;
        }

        /// <summary>
        /// 请求载入场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName)
        {
            EngineCore.EngineCoreEvents.ResourceEvent.LoadAdditiveScene.SafeInvoke(sceneName, OnLoadedScene);
        }


        public Vector3 GetEntityPos(long prop_id_)
        {
            return m_entity_positions.ContainsKey(prop_id_) ? m_entity_positions[prop_id_] : Vector3.zero;
        }

        public Quaternion GetEntityRotate(long prop_id_)
        {
            return m_entity_rotates.ContainsKey(prop_id_) ? m_entity_rotates[prop_id_] : Quaternion.identity;
        }

        /// <summary>
        /// 载入场景完成
        /// </summary>
        protected virtual void OnLoadedScene()
        {
            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(1, true);
            CameraManager.Instance.InitCameraController<GameCameraNew>();
            CameraManager.Instance.ResetMainCamera();
            GameEvents.SceneEvents.OnEnterScene.SafeInvoke();

            InputModule.Instance.Enable = true;
            EasyTouch.SetEnableAutoSelect(true);
            EasyTouch.instance.touchCameras[0].camera = Camera.main;


            EngineCoreEvents.InputEvent.OnOneFingerTouchup += OnTouchScreen;
            EngineCoreEvents.InputEvent.OnPinIn += ZoomIn;
            EngineCoreEvents.InputEvent.OnPinOut += ZoomOut;
            EngineCoreEvents.InputEvent.OnSwipe += Swipe;
            //GameEvents.SceneEvents.Listen_GetExhibitPos += GetEntityPos;
            //GameEvents.SceneEvents.Listen_GetExhibitRotate += GetEntityRotate;

            FindingEntityCoords();
        }

        void FindingEntityCoords()
        {
            GameObject exhibit_root = GameObject.Find("ExhibitRoot");

            if (null == exhibit_root)
            {
                Debug.LogError("没有设置展厅，陈列物根节点");
                return;
            }

            foreach (Transform child in exhibit_root.transform)
            {
                string child_name = child.gameObject.name;
                long prop_id = long.Parse(child_name);
                Vector3 exhibit_world_pos = child.position;
                Quaternion exhibit_world_rotate = child.rotation;
                m_entity_positions.Add(prop_id, exhibit_world_pos);
                m_entity_rotates.Add(prop_id, exhibit_world_rotate);
            }

            ActiveSceneEntities();

        }

        protected virtual void ActiveSceneEntities()
        {
            if (!m_is_entity_collection_created)
            {
                TimeModule.Instance.SetTimeout(ActiveSceneEntities, Time.deltaTime);
                return;
            }

            bool allDone = true;
            foreach (EntityBase entity in AllSceneEntities)
            {
                if (entity.AssetLoadStatus == ResStatus.NONE)
                {
                    entity.Load();
                    allDone = false;
                }
                else if (entity.AssetLoadStatus == ResStatus.WAIT)
                    allDone = false;
            }

            if (!allDone)
            {
                TimeModule.Instance.SetTimeout(ActiveSceneEntities, Time.deltaTime);
                return;
            }


            InitEntityPos();
        }

        void InitEntityPos()
        {
            foreach (EntityBase entity in AllSceneEntities)
            {
                if (entity is SceneExhibitEntity)
                {
                    var ex_entity = entity as SceneExhibitEntity;
                    long prop_id = ex_entity.EntityData;

                    Vector3 ex_pos = GetEntityPos(prop_id);
                    ex_entity.EntityPosition = ex_pos;
                    Quaternion ex_rotate = GetEntityRotate(prop_id);
                    ex_entity.EntityEulerRotation = GetEntityRotate(prop_id).eulerAngles;
                    ex_entity.EntityScale = Vector3.one;
                }
            }

            GameEvents.UIEvents.UI_Loading_Event.OnLoadingState.SafeInvoke(2, true);
        }

        protected virtual void LoadSceneObjects()
        {
            m_is_entity_collection_created = false;

            var exhibitions = GameEvents.UIEvents.UI_Bag_Event.Listen_GetAllExhibitions.SafeInvoke();

            foreach (PropData ex in exhibitions)
            {
                EntityBase sceneItemEntity = EntityManager.Instance.AllocEntity(EntityType.Scene_Exhibit);

                sceneItemEntity.SetAssetName(ex.prop.exhibit);
                sceneItemEntity.SetEntityData(ex.prop.id);
                //sceneItemEntity.SetLightTexture(m_sceneExtraLightTex);
                sceneItemEntity.PreloadAsset();

                AddSceneEntity(sceneItemEntity);
            }

            m_is_entity_collection_created = true;

        }


        protected virtual void OnTouchScreen(Gesture gesture)
        {
            //if (this.m_gameStatus == GameStatus.GAMING || NewGuid.GuidNewNodeManager.Instance.GetNodeStatus(NewGuid.GuidNewNodeManager.SceneTips) == NewGuid.NodeStatus.None)
            //{
            //    GameEvents.MainGameEvents.OnSceneClick.SafeInvoke(gesture.position);
            //    TotalTouchCount++;
            //    if (gesture.pickedObject != null && gesture.pickedObject.layer == LayerDefine.SceneTargetObjectLayer)
            //    {
            //        if (!gesture.pickedObject.CompareTag("cameraPoint"))
            //            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.game_exhibit.ToString());
            //        OnPickedSceneObject(gesture.pickedObject);
            //    }
            //    else
            //    {
            //        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.game_error.ToString());
            //        GameEvents.MainGameEvents.OnErrorTouch.SafeInvoke(TimeModule.GameRealTime.RealTime, gesture.position);
            //    }
            //}
        }

        protected virtual void ZoomIn(Gesture gesture)
        {
        }

        protected virtual void ZoomOut(Gesture gesture)
        {
        }

        protected virtual void Swipe(Gesture gesture)
        {
        }
    }
}