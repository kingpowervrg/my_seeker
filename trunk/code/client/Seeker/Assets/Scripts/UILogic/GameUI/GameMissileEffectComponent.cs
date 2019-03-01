using System;
using System.Collections.Generic;
using EngineCore;
using UnityEngine;
using DG.Tweening;
namespace SeekerGame
{
    public class GameMissileComponent : GameUIComponent
    {
        private GameUIContainer m_container = null;
        private GameMissileEffectComponent[] m_missileEffect = null;
        //private GameMissileEffect3D[] m_missileEffect3D = null;
        protected override void OnInit()
        {
            base.OnInit();
            this.m_container = Make<GameUIContainer>(gameObject);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.MainGameEvents.OnMissileFly += OnMissileLaunch;
            #region 二维炸弹
            m_missileEffect = new GameMissileEffectComponent[3];
            this.m_container.EnsureSize<GameMissileEffectComponent>(3);
            for (int i = 0; i < 3; i++)
            {
                m_missileEffect[i] = this.m_container.GetChild<GameMissileEffectComponent>(i);
                m_missileEffect[i].Visible = true;
            }
            #endregion
            #region 3D炸弹
            //if (m_missileEffect3D == null)
            //{
            //    m_missileEffect3D = new GameMissileEffect3D[3];
            //    for (int i = 0; i < m_missileEffect3D.Length; i++)
            //    {
            //        GameMissileEffect3D missile = new GameMissileEffect3D();
            //        missile.Load();
            //        m_missileEffect3D[i] = missile;
            //    }

            //}
            #endregion

        }

        public void OnMissileLaunch(Vector3 startPos,Vector3 endPos,Action cb)
        {
            for (int i = 0; i < 3; i++)
            {
                if (m_missileEffect[i].GetStatus() == MissileStatus.UnUse)
                {
                    Debug.Log("missileLaunch : " + i);
                    m_missileEffect[i].MissileLaunch(startPos,endPos,cb);
                    return;
                }
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.MainGameEvents.OnMissileFly -= OnMissileLaunch;
            //m_missileEffect3D = null;
        }



        public class GameMissileEffectComponent : GameUIComponent
        {
            private GameUIEffect m_flyEffect = null;
            private GameUIEffect m_boomEffect = null;
            private MissileStatus m_status = MissileStatus.UnUse;
            protected override void OnInit()
            {
                base.OnInit();
                this.m_flyEffect = Make<GameUIEffect>("Effect_Fly");
                this.m_boomEffect = Make<GameUIEffect>("Effect_Boom");
            }

            public override void OnShow(object param)
            {
                base.OnShow(param);
                this.m_flyEffect.EffectPrefabName = "UI_zhadan_tuowei_1.prefab";
                this.m_flyEffect.Visible = false;
                this.m_boomEffect.EffectPrefabName = "UI_zhadan.prefab";
                this.m_boomEffect.Visible = false;
                this.m_status = MissileStatus.UnUse;
            }

            public override void OnHide()
            {
                base.OnHide();
            }

            public void SetStatus(MissileStatus status)
            {
                m_status = status;
            }

            public MissileStatus GetStatus()
            {
                return m_status;
            }

            public void MissileLaunch(Vector3 startPos,Vector3 endPos,Action cb)
            {
                SetStatus(MissileStatus.Fly);
                this.m_flyEffect.Position = startPos;
                this.m_flyEffect.Visible = true;
                this.m_flyEffect.ReplayEffect();
                Vector3[] m_wayPoint = new Vector3[3];
                Vector3 upPoint = new Vector3(startPos.x,startPos.y + 80f,startPos.z);
                m_wayPoint[0] = startPos;
                m_wayPoint[1] = upPoint;
                m_wayPoint[2] = endPos;
                this.m_flyEffect.Widget.DOPath(m_wayPoint, 1f, PathType.CatmullRom, PathMode.Sidescroller2D).SetLookAt(0).SetEase(Ease.InFlash).OnComplete(() =>
                {
                    this.m_flyEffect.Visible = false;
                    this.m_boomEffect.Position = endPos;
                    this.m_boomEffect.Visible = true;
                    if (cb != null)
                    {
                        cb();
                    }
                    TimeModule.Instance.SetTimeout(()=> {
                        this.m_status = MissileStatus.UnUse;
                        this.m_boomEffect.Visible = false;
                        
                    },0.5f);
                    //this.m_boomEffect.ReplayEffect();
                    //this.Visible = false;
                    // OnFlyEffectPlayFinished?.Invoke();
                });
            }
        }

        public class GameMissileEffect3D
        {
            private GameObject m_missileObj = null;
            private GameObject m_bloomObj = null;
            private MissileStatus m_missileStatus = MissileStatus.UnUse;

            public MissileStatus GetStatus()
            {
                return this.m_missileStatus;
            }


            public void Load()
            {
                GOGUI.GOGUITools.GetAssetAction.SafeInvoke("UI_zhadan_tuowei.prefab", (prefabName, obj) => {
                    this.m_missileObj = (GameObject)obj;
                    this.m_missileObj.SetActive(false);
                }, LoadPriority.Default);
                GOGUI.GOGUITools.GetAssetAction.SafeInvoke("UI_zhadan.prefab", (prefabName, obj) =>
                {
                    this.m_bloomObj = (GameObject)obj;
                    this.m_bloomObj.SetActive(false);
                }, LoadPriority.Default);
            }

            

            public void MissileLaunch(Vector3 startPos, Vector3 endPos, Action cb)
            {
                if (this.m_missileObj == null)
                {
                    return;
                }
                m_missileStatus = MissileStatus.Fly;
                this.m_missileObj.transform.position = startPos;
                this.m_missileObj.SetActive(true);
                Vector3[] m_wayPoint = new Vector3[3];
                Vector3 upPoint = new Vector3(startPos.x, startPos.y + 40f, startPos.z);
                m_wayPoint[0] = startPos;
                m_wayPoint[1] = upPoint;
                m_wayPoint[2] = endPos;
                this.m_missileObj.transform.DOPath(m_wayPoint, 3f, PathType.Linear, PathMode.Full3D).SetLookAt(0).SetEase(Ease.InFlash).OnComplete(() =>
                {
                    this.m_missileObj.SetActive(false);
                    this.m_bloomObj.transform.position = endPos;
                    this.m_bloomObj.SetActive(true);
                    if (cb != null)
                    {
                        cb();
                    }
                    TimeModule.Instance.SetTimeout(() =>
                    {
                        this.m_missileStatus = MissileStatus.UnUse;
                        this.m_bloomObj.SetActive(false);
                    }, 0.5f);
                });

                //this.m_missileObj.transform.
            }
        }

        public enum MissileStatus
        {
            UnUse,
            Fly,
        }
    }


    
}
