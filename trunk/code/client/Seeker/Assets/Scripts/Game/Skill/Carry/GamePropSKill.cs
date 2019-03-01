using EngineCore;

namespace SeekerGame
{
    public class GamePropSKill : GameSkillCarryBase
    {

        public GamePropSKill(long propId) : base(propId)
        {

        }

        protected override void InitCarryBase()
        {
            ConfProp confProp = ConfProp.Get(m_carryID);
            if (confProp != null)
            {
                m_skillID = confProp.skillId;
                MessageHandler.RegisterMessageHandler(MessageDefine.SCSkillEmitResponse, OnRes);
                GameEvents.MainGameEvents.OnGameStatusChange += OnGameStatusChange;
                base.InitCarryBase();
            }
        }

        private void OnGameStatusChange(SceneBase.GameStatus status)
        {
            if (m_skillStatus == SkillStatus.Release)
            {
                if (status == SceneBase.GameStatus.PAUSE)
                {
                    m_skillBase.OnPause();
                }
                else if (status == SceneBase.GameStatus.GAMING)
                {
                    m_skillBase.OnResume();
                }
            }

        }

        protected override bool OnRequestSkill(int count)
        {
            if (!base.OnRequestSkill(count))
            {
                return false;
            }
            CSSkillEmitRequest req = new CSSkillEmitRequest();
            req.PropId = m_carryID;
            req.Count = count;
            req.InOrOutScene = (InOrOutScene)((SceneModule.Instance.CurrentScene is GameSceneBase) ? 1 : 0);
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
            m_skillBase.SetReleaseCount(count);
            return true;
        }

        protected override void OnRes(object obj)
        {
            base.OnRes(obj);
            if (obj is SCSkillEmitResponse)
            {
                SCSkillEmitResponse res = (SCSkillEmitResponse)obj;

                if (res.PropId == m_carryID)
                {
                    if (0 == res.SkillId)
                    {
                        UnityEngine.Debug.LogError("skill release error : " + m_carryID);
                        GameEvents.Skill_Event.OnSkillError.SafeInvoke(m_carryID);
                        GameEvents.Skill_Event.OnSkillReset.SafeInvoke(m_carryID);
                        return;
                    }

                    if (res.SkillId != m_skillID)
                        return;

                    if (res.Result == 0)
                    {
                        UnityEngine.Debug.LogError("skill release error : " + m_carryID);
                        GameEvents.Skill_Event.OnSkillError.SafeInvoke(m_carryID);
                        GameEvents.Skill_Event.OnSkillReset.SafeInvoke(m_carryID);
                        return;
                    }
                    m_skillStatus = SkillStatus.Release;
                    System.Collections.Generic.Dictionary<UBSParamKeyName, object> _params = new System.Collections.Generic.Dictionary<UBSParamKeyName, object>()
                    {
                                { UBSParamKeyName.ContentID, res.PropId},
                                { UBSParamKeyName.ContentType, 0},
                                { UBSParamKeyName.Description, UBSDescription.PROPUSE}
                    };
                    UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Pror_use, 1.0f, _params);
                    //释放成功
                    if (1 == res.PropId)
                    {
                        //使用寻物镜的音效
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.item_xunwujing.ToString());
                    }
                    else if (2 == res.PropId)
                    {
                        //使用探测仪的音效
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.item_tanceyi.ToString());
                    }
                    else if (3 == res.PropId)
                    {
                        //使用炸弹的音效
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.item_boom.ToString());
                    }
                    else if (4 == res.PropId)
                    {
                        //使用加时器的音效
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.item_jiashiqi.ToString());
                    }
                    else if (5 == res.PropId)
                    {
                        //使用拭纸巾的音效
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.item_shizhijin.ToString());
                    }
                    else if (6 == res.PropId)
                    {
                        //使用应急灯的音效
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.item_yingjideng.ToString());
                    }
                    else
                    {
                        EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
                    }
                    GameEvents.Skill_Event.OnSkillFinish.SafeInvoke(m_carryID);
                    m_skillBase.OnStart();

                }
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

        }

        public override void OnDestory()
        {
            base.OnDestory();
            if (m_skillID >= 0)
            {
                MessageHandler.UnRegisterMessageHandler(MessageDefine.SCSkillEmitResponse, OnRes);
            }
        }
    }
}
