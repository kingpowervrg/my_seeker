using EngineCore;
using GOGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_BAGUSE)]
    public class BagUseDialogUILogic : UILogicBase
    {
        private GameLabel m_title_lab;
        private GameImage m_icon_img;
        private GameLabel m_propName_lab;
        private GameLabel m_propNum_lab;
        private GameLabel m_propPrice_lab;
        private GameLabel m_priceNode_lab;

        private GameLabel m_useNum_lab;
        private GameButton m_addNum_btn;
        private GameButton m_reduceNum_btn;
        private GameButton m_maxNum_btn;

        private GameImage m_getNumNode_lab;
        private GameLabel m_getNum_lab;
        private GameButton m_yes_btn;

        private GameButton m_close_btn;

        private BagUseData m_bagData;

        private GameLabel m_currentNumLab = null;
        private GameLabel m_yesLab = null;
        //private UITweenerBase[] tweener = null;
        private int m_curNum = 1;
        protected override void OnInit()
        {
            base.OnInit();
            InitController();
        }

        private void InitController()
        {
            m_title_lab = Make<GameLabel>("Panel_animation:title");
            m_icon_img = Make<GameImage>("Panel_animation:Image_icon:icon");
            m_propName_lab = Make<GameLabel>("Panel_animation:Image_icon:title");
            m_propNum_lab = Make<GameLabel>("Panel_animation:Image_icon:sum");
            m_priceNode_lab = Make<GameLabel>("Panel_animation:Image_icon:sell");
            m_currentNumLab = Make<GameLabel>("Panel_animation:Image_icon:Text");
            m_propPrice_lab = m_priceNode_lab.Make<GameLabel>("number");

            m_useNum_lab = Make<GameLabel>("Panel_animation:Imagenumber:Text");
            m_addNum_btn = Make<GameButton>("Panel_animation:Imagenumber:btnAdd");
            m_reduceNum_btn = Make<GameButton>("Panel_animation:Imagenumber:btnReduce");
            m_maxNum_btn = Make<GameButton>("Panel_animation:Imagenumber:btnMax");

            m_getNumNode_lab = Make<GameImage>("Panel_animation:Button_continue:totalNum:moneyIcon");
            m_getNum_lab = Make<GameLabel>("Panel_animation:Button_continue:totalNum");
            m_yes_btn = Make<GameButton>("Panel_animation:Button_continue");
            m_yesLab = m_yes_btn.Make<GameLabel>("Text");

            m_close_btn = Make<GameButton>("Panel_animation:Button_close");
            //this.tweener = Transform.GetComponentsInChildren<UITweenerBase>(true);
        }

        private void InitEventListener()
        {
            m_addNum_btn.AddLongClickCallBack(OnAddNum);
            m_reduceNum_btn.AddLongClickCallBack(OnReduce);
            m_maxNum_btn.AddClickCallBack(OnMax);
            m_yes_btn.AddClickCallBack(OnSure);
            m_close_btn.AddClickCallBack(OnClose);
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);
            GameEvents.Skill_Event.OnSkillFinish += OnSkillFinish;
            MessageHandler.RegisterMessageHandler(MessageDefine.PlayerPropSellResposne, OnRes);
            MessageHandler.RegisterMessageHandler(MessageDefine.SCDropResp, OnRes);
            if (param is BagUseData)
            {
                m_bagData = param as BagUseData;
            }
            InitEventListener();
            InitDialog();
            CheckState();
            //for (int i = 0; i < this.tweener.Length; i++)
            //{
            //    this.tweener[i].ResetAndPlay();
            //}
        }

        private void InitDialog()
        {
            if (m_bagData == null)
            {
                return;
            }
            ConfProp confProp = m_bagData.prop.prop;
            m_propName_lab.Text = LocalizeModule.Instance.GetString(confProp.name);
            m_icon_img.Sprite = confProp.icon;
            m_propNum_lab.Text = string.Format("x{0}", m_bagData.prop.num);
            m_currentNumLab.Text = LocalizeModule.Instance.GetString("user_have_icon", m_bagData.prop.num);
            m_useNum_lab.Text = m_curNum.ToString();//string.Format("{0}/{1}",m_curNum, m_bagData.prop.num);
            if (m_bagData.infoType == PropInfoTypeEnum.Use)
            {
                m_title_lab.Text = "Use Prop";
                m_priceNode_lab.SetActive(false);
                m_getNumNode_lab.SetActive(false);
                m_getNum_lab.Visible = false;
                m_yesLab.Visible = true;
            }
            else if (m_bagData.infoType == PropInfoTypeEnum.Sale)
            {
                m_title_lab.Text = "Sale Prop";
                m_priceNode_lab.SetActive(true);
                m_getNumNode_lab.SetActive(true);
                m_getNum_lab.Visible = true;
                m_yesLab.Visible = false;
                m_propPrice_lab.Text = confProp.price.ToString();
                m_getNum_lab.Text = (confProp.price * m_curNum).ToString();
            }
        }

        private void setCostInfo()
        {
            m_useNum_lab.Text = m_curNum.ToString();
            if (m_bagData.infoType == PropInfoTypeEnum.Sale)
            {
                m_getNum_lab.Text = (m_bagData.prop.prop.price * m_curNum).ToString();
            }
        }

        private void OnAddNum(GameObject obj, float time)
        {
            if (m_bagData == null || m_curNum >= m_bagData.prop.num)
            {

                return;
            }
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.bag_soldadd.ToString());
            m_curNum++;
            CheckState();
            setCostInfo();
        }

        private void OnReduce(GameObject obj, float time)
        {
            if (m_bagData == null || m_curNum <= 1)
            {
                return;
            }
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.bag_soldadd.ToString());
            m_curNum--;
            CheckState();
            setCostInfo();
        }

        private void OnMax(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Button_Click_Common.ToString());
            if (m_bagData == null)
            {
                return;
            }
            m_curNum = m_bagData.prop.num;
            CheckState();
            setCostInfo();
        }

        private void OnSure(GameObject obj)
        {
            ConfProp confProp = m_bagData.prop.prop;
            if (m_bagData.infoType == PropInfoTypeEnum.Use)
            {
                if (confProp.type == 3)
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.gift_open.ToString());
                    CSDropReq req = new CSDropReq();
                    req.PropId = confProp.id;
                    req.Count = m_curNum;
                    GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
                }
                else
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.item_use.ToString());
                    if (confProp.skillId <= 0)
                    {
                        PopUpManager.OpenNormalOnePop("skill not exits");
                    }
                    GameSkillManager.Instance.OnStartSkill(confProp.id, m_curNum);
                }
            }
            else if (m_bagData.infoType == PropInfoTypeEnum.Sale)
            {
                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.item_sold.ToString());
                PlayerPropSellRequest req = new PlayerPropSellRequest();
                PlayerPropMsg playerProp = new PlayerPropMsg();
                playerProp.PropId = confProp.id;
                playerProp.Count = m_curNum;
                req.PlayerProps.Add(playerProp);
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
                System.Collections.Generic.Dictionary<UBSParamKeyName, object> _params = new System.Collections.Generic.Dictionary<UBSParamKeyName, object>()
                    {
                                { UBSParamKeyName.ContentID, confProp.id},
                                { UBSParamKeyName.ContentType, 1},
                                { UBSParamKeyName.Description, UBSDescription.PROPSELL }
                    };
                UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Pror_use, 1.0f, _params);
            }
            //GameEvents.UIEvents.UI_Bag_Event.OnPropCost.SafeInvoke(confProp.id);
            //EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_BAGUSE);
        }

        private void OnClose(GameObject obj)
        {
            EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, EngineCommonAudioKey.Close_Window.ToString());
            EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_BAGUSE);
        }

        private void OnSkillFinish(long propId)
        {
            if (propId == m_bagData.prop.prop.id)
            {
                if (m_bagData.prop.prop.type == 0)
                {
                    WaveTipHelper.LoadWaveContent("bag_energy_use", ConfSkill.Get(m_bagData.prop.prop.skillId).gain);
                }
                else
                {
                    WaveTipHelper.LoadWaveContent("bag_prop_use");
                }
                //PopUpManager.OpenNormalOnePop("afewfewfe");
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(propId, m_curNum);
                GameEvents.UIEvents.UI_Bag_Event.OnPropCost.SafeInvoke(m_bagData.prop.prop.id);
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_BAGUSE);
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.Skill_Event.OnSkillFinish -= OnSkillFinish;
            MessageHandler.UnRegisterMessageHandler(MessageDefine.PlayerPropSellResposne, OnRes);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCDropResp, OnRes);
            m_curNum = 1;
            m_bagData = null;
            m_addNum_btn.RemoveLongClickCallBack(OnAddNum);
            m_reduceNum_btn.RemoveLongClickCallBack(OnReduce);
            m_maxNum_btn.RemoveClickCallBack(OnMax);
            m_yes_btn.RemoveClickCallBack(OnSure);
            m_close_btn.RemoveClickCallBack(OnClose);
        }

        private void OnRes(object obj)
        {
            if (obj is PlayerPropSellResposne)
            {
                PlayerPropSellResposne res = (PlayerPropSellResposne)obj;
                WaveTipHelper.LoadWaveContent("Successful_sale");
                GlobalInfo.MY_PLAYER_INFO.ChangeCoin(m_bagData.prop.prop.price * m_curNum);
                GlobalInfo.MY_PLAYER_INFO.ReducePropForBag(m_bagData.prop.prop.id, m_curNum);
                GameEvents.UIEvents.UI_Bag_Event.OnPropCost.SafeInvoke(m_bagData.prop.prop.id);
                EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_BAGUSE);
            }
            else if (obj is SCDropResp)
            {
                SCDropResp res = (SCDropResp)obj;
                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
                param.Param = res;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
                OnSkillFinish(m_bagData.prop.prop.id);

                GameEvents.PlayerEvents.RequestLatestPlayerInfo.SafeInvoke();
            }
        }

        private void CheckState()
        {
            m_reduceNum_btn.SetGray(m_curNum <= 1);
            m_addNum_btn.SetGray(m_curNum >= m_bagData.prop.num);
            m_maxNum_btn.SetGray(m_curNum >= m_bagData.prop.num);
        }

        public override FrameDisplayMode UIFrameDisplayMode => FrameDisplayMode.WINDOWED;
    }
}

