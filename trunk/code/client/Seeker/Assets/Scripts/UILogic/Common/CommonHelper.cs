using EngineCore;
using EngineCore.Utility;
using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utf8Json;

namespace SeekerGame
{
    public static class CommonHelper

    {
        public const string C_FB_URL = "https://www.facebook.com/groups/2129013450693076/";
        public static string GetStringFromLocalize(string key_)
        {
            return LocalizeModule.Instance.GetString(key_);
        }

        public static void OpenEnterGameScanUI(long scan_id_, ChapterInfo sceneChapterInfo = null, long taskID = -1)
        {

            CSFindEnterReq req = new CSFindEnterReq()
            {
                FindId = scan_id_,
            };

            GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_SCAN_GAME) { Param = new List<long> { scan_id_, taskID } });
            return;
        }

        public static bool NeedOpenAcceptTaskUI(long scene_id_)
        {
            if (CommonData.C_CARTOON_SCENE_START_ID == scene_id_ / CommonData.C_SCENE_TYPE_ID)
            {
                StartCartoonManager.Instance.OpenStartCartoonForID(scene_id_);
                return false;
            }
            return true;
        }

        public static bool NeedOpenTalkUI(NormalTask taskInfo)
        {
            if (taskInfo.TaskData.dialogBegin > 0)
            {
                return true;
            }
            return false;
        }

        public static void OpenEnterGameSceneUI(long scene_id_, ChapterInfo sceneChapterInfo = null, long taskID = -1)
        {
            if (scene_id_ < 10)
            {
                CSFindEnterReq req = new CSFindEnterReq()
                {
                    FindId = scene_id_,
                };

                GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(req);

                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_SCAN_GAME) { Param = new List<long> { scene_id_, taskID } });
                return;
            }

            if (CommonData.C_JIGSAW_SCENE_START_ID == scene_id_ / CommonData.C_SCENE_TYPE_ID)
            {
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_ENTER_JIGSAW) { Param = new List<long> { scene_id_, taskID } });
            }
            else if (CommonData.C_CARTOON_SCENE_START_ID == scene_id_ / CommonData.C_SCENE_TYPE_ID)
            {
                StartCartoonManager.Instance.OpenStartCartoonForID(scene_id_);
            }
            else
            {
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(new FrameMgr.OpenUIParams(UIDefine.UI_FIND_OBJ_ENTER_UI)
                {
                    Param = new EnterSceneData()
                    {
                        SceneID = scene_id_,
                        SceneBelongChapterInfo = sceneChapterInfo,
                        taskConfID = taskID
                    }
                });
            }
        }

        public static void ShowOutput(List<OutPutItemView> outputs_, int exp_, int coin_, int cash_, int vit_)
        {
            outputs_.ForEach((item) => { item.Visible = false; });

            int output_idx = 0;

            if (0 != exp_)
            {
                outputs_[output_idx].Visible = true;
                outputs_[output_idx].m_icon.Sprite = "icon_mainpanel_exp_2.png";
                outputs_[output_idx].m_num.Text = exp_.ToString();
                ++output_idx;
            }
            if (0 != coin_)
            {
                outputs_[output_idx].Visible = true;
                outputs_[output_idx].m_icon.Sprite = "icon_mainpanel_coin_2.png";
                outputs_[output_idx].m_num.Text = coin_.ToString();
                ++output_idx;
            }
            if (0 != cash_)
            {
                outputs_[output_idx].Visible = true;
                outputs_[output_idx].m_icon.Sprite = "icon_mainpanel_cash_2.png";
                outputs_[output_idx].m_num.Text = cash_.ToString();
                ++output_idx;
            }
            if (0 != vit_)
            {
                outputs_[output_idx].Visible = true;
                outputs_[output_idx].m_icon.Sprite = "icon_mainpanel_energy_2.png";
                outputs_[output_idx].m_num.Text = vit_.ToString();
                ++output_idx;
            }
        }



        public static void ShowOutput(GameUIContainer outputs_, int exp_, int coin_, int cash_, int vit_)
        {
            outputs_.EnsureSize<OutPutItemView>(4);


            if (0 != exp_)
            {
                outputs_.GetChild<OutPutItemView>(0).Visible = true;
                outputs_.GetChild<OutPutItemView>(0).m_icon.Sprite = "icon_mainpanel_exp_2.png";
                outputs_.GetChild<OutPutItemView>(0).m_num.Text = exp_.ToString();
            }
            if (0 != coin_)
            {
                outputs_.GetChild<OutPutItemView>(0).Visible = true;
                outputs_.GetChild<OutPutItemView>(0).m_icon.Sprite = "icon_mainpanel_coin_2.png";
                outputs_.GetChild<OutPutItemView>(0).m_num.Text = coin_.ToString();
            }
            if (0 != cash_)
            {
                outputs_.GetChild<OutPutItemView>(0).Visible = true;
                outputs_.GetChild<OutPutItemView>(0).m_icon.Sprite = "icon_mainpanel_cash_2.png";
                outputs_.GetChild<OutPutItemView>(0).m_num.Text = cash_.ToString();
            }
            if (0 != vit_)
            {
                outputs_.GetChild<OutPutItemView>(0).Visible = true;
                outputs_.GetChild<OutPutItemView>(0).m_icon.Sprite = "icon_mainpanel_energy_2.png";
                outputs_.GetChild<OutPutItemView>(0).m_num.Text = vit_.ToString();
            }
        }

        public static void ShowPropTips(long prop_id, int prop_num_, Vector3 ui_world_pos, Vector2 m_offset)
        {
            Vector2 screenPoint2 = RectTransformUtility.WorldToScreenPoint(CameraManager.Instance.UICamera, ui_world_pos);

            int address_count = ConfProp.Get(prop_id).address.Length;

            ToolTipsData data = new ToolTipsData()
            {
                ItemID = prop_id,
                CurCount = prop_num_,
                MaxCount = 0,

                ScreenPos = screenPoint2 - new Vector2(ToolTipsView.C_WIDTH * 0.5f + m_offset.x/* - this.Widget.sizeDelta.x * 0.5f*/, -10.0f * address_count + m_offset.y),
            };


            FrameMgr.OpenUIParams ui_data = new FrameMgr.OpenUIParams(UIDefine.UI_TOOL_TIPS)
            {
                Param = data,
            };

            EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(ui_data);
        }

        public static List<DropOutJsonData> ParseDropOut(string json)
        {
            return JsonSerializer.Deserialize<List<DropOutJsonData>>(json);

            //json = json.Replace("count:", "");
            //json = json.Replace("rate:", "");
            //json = json.Replace("value:", "");
            //json = json.Replace("},{", "|");
            //json = json.Replace("{", "");
            //json = json.Replace("}", "");
            //json = json.Replace("[", "");
            //json = json.Replace("]", "");
            //string[] arrays = json.Split('|');
            //List<DropOutJsonData> rankList = new List<DropOutJsonData>();
            //for (int i = 0; i < arrays.Length; i++)
            //{
            //    string[] valueArrays = arrays[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            //    if (valueArrays.Length == 3)
            //    {
            //        DropOutJsonData data = new DropOutJsonData();
            //        int count;
            //        int rate;
            //        long value;
            //        int.TryParse(valueArrays[0], out count);
            //        int.TryParse(valueArrays[1], out rate);
            //        long.TryParse(valueArrays[2], out value);
            //        data.Count = count;
            //        data.Rate = rate;
            //        data.Value = value;
            //        rankList.Add(data);
            //    }
            //}
            //return rankList;
        }

        public static List<long> GetDropOuts(long drop_id_)
        {


            ConfDropOut2 drop_all = ConfDropOut2.Get(drop_id_);

            if (null == drop_all)
                return new List<long>();

            string rdm_drop = drop_all.merge;
            List<DropOutJsonData> rdm_datas = new List<DropOutJsonData>();
            if (IsStringValid(rdm_drop))
                rdm_datas = CommonHelper.ParseDropOut(rdm_drop);

            HashSet<DropOutJsonData> rdm_set = new HashSet<DropOutJsonData>(rdm_datas);


            string fix_drop = drop_all.fixed2;
            List<DropOutJsonData> fix_datas = new List<DropOutJsonData>();
            if (IsStringValid(fix_drop))
                fix_datas = CommonHelper.ParseDropOut(fix_drop);

            HashSet<DropOutJsonData> fixed_set = new HashSet<DropOutJsonData>(fix_datas);

            rdm_set.ExceptWith(fixed_set);

            List<DropOutJsonData> all_datas = new List<DropOutJsonData>();

            all_datas.AddRange(rdm_set);

            List<long> ids = new List<long>();

            int counter = 0;
            int[] idx = CommonUtils.GetRandomList(all_datas.Count);

            while (counter < all_datas.Count && counter < 6)
            {
                ids.Add(all_datas[idx[counter]].value);
                ++counter;

            }

            return ids;
        }




        public static List<DropOutJsonData> GetFixedDropOuts(long drop_id_)
        {


            ConfDropOut2 drop_all = ConfDropOut2.Get(drop_id_);

            string fix_drop = drop_all.fixed2;
            List<DropOutJsonData> fix_datas = new List<DropOutJsonData>();
            if (IsStringValid(fix_drop))
                fix_datas = CommonHelper.ParseDropOut(fix_drop);

            return fix_datas;


        }

        public static void UpdateEffectPosByProgressbar(GameProgressBar bar_, GameUIEffect effect_, float interval_, float duration_)
        {

            TimeModule.Instance.SetTimeInterval(() => EffectProgressbarValueSync(bar_, effect_), interval_, duration_).OnComplete(() => effect_.Visible = false);

            TimeModule.Instance.SetTimeout(() => { effect_.Visible = false; effect_.Visible = true; }, 0.01f);
        }

        public static void EffectProgressbarValueSync(GameProgressBar bar_, GameUIEffect effect_)
        {
            Vector3 topRightConner = bar_.FillRectangleWorldConners[2];
            Vector3 bottomRightConner = bar_.FillRectangleWorldConners[3];

            Vector3 centerPos = new Vector3(topRightConner.x, (topRightConner.y + bottomRightConner.y) / 2, topRightConner.z);
            effect_.Position = centerPos;
        }


        public static string GetOutputIconName(EUNM_BASE_REWARD output_type)
        {
            string icon_name = "";

            switch (output_type)
            {
                case (EUNM_BASE_REWARD.E_COIN):
                    {
                        icon_name = "icon_mainpanel_coin_2.png";
                    }
                    break;
                case (EUNM_BASE_REWARD.E_CASH):
                    {

                        icon_name = "icon_mainpanel_cash_2.png";

                    }
                    break;
                case (EUNM_BASE_REWARD.E_EXP):
                    {

                        icon_name = "icon_mainpanel_exp_2.png";

                    }
                    break;
                case (EUNM_BASE_REWARD.E_VIT):
                    {

                        icon_name = "icon_mainpanel_energy_2.png";

                    }
                    break;
            }

            return icon_name;
        }

        public static string GetOutputIconName(RewardItemType output_type)
        {
            string icon_name = "";

            switch (output_type)
            {
                case (RewardItemType.COIN):
                    {
                        icon_name = "icon_mainpanel_coin_2.png";
                    }
                    break;
                case (RewardItemType.CASH):
                    {

                        icon_name = "icon_mainpanel_cash_2.png";

                    }
                    break;
                case (RewardItemType.EXP):
                    {

                        icon_name = "icon_mainpanel_exp_2.png";

                    }
                    break;
                case (RewardItemType.VIT):
                    {

                        icon_name = "icon_mainpanel_energy_2.png";

                    }
                    break;
            }

            return icon_name;
        }

        public static string GetOutputColorNum(EUNM_BASE_REWARD output_type, int num)
        {
            string color_num = "";

            switch (output_type)
            {
                case (EUNM_BASE_REWARD.E_COIN):
                    {
                        color_num = "<color=#ffcb7e>{0}</color>";
                    }
                    break;
                case (EUNM_BASE_REWARD.E_CASH):
                    {

                        color_num = "<color=#92ffb2>{0}</color>";

                    }
                    break;
                case (EUNM_BASE_REWARD.E_EXP):
                    {

                        color_num = "<color=#7cd4ff>{0}</color>";

                    }
                    break;
                case (EUNM_BASE_REWARD.E_VIT):
                    {

                        color_num = "<color=#ffefab>{0}</color>";

                    }
                    break;
            }

            return string.Format(color_num, num);
        }


        public static int GetPropExp(long prop_id_, int num_)
        {
            ConfProp prop = ConfProp.Get(prop_id_);
            if ((int)PROP_TYPE.E_EXP == prop.type)
            {
                return prop.value * num_;
            }

            return 0;
        }

        static SafeAction<string> OnShareLinkResult;

        public static void FBShareLink(string url, Action<string> OnResult_)
        {
            OnShareLinkResult = OnResult_;
            FB.ShareLink(new Uri(url), callback: HandleResult);
        }

        public static void FBLogout()
        {
            if (FB.IsLoggedIn)
            {
                FB.LogOut();
            }
        }

        static void HandleResult(IResult result)
        {
            string LastResponse = "";
            if (result == null)
            {
                LastResponse = "FACEBOOK: invite Null Response\n";
                Debug.LogError(LastResponse);
                OnShareLinkResult.SafeInvoke(LastResponse);
                return;
            }


            string Status = "";

            // Some platforms return the empty string instead of null.
            if (!string.IsNullOrEmpty(result.Error))
            {
                Status = "FACEBOOK: inviteError - Check log for details";
                LastResponse = "FACEBOOK: inviteError Response:\n" + result.Error;
                OnShareLinkResult.SafeInvoke(LastResponse);
            }
            else if (result.Cancelled)
            {
                Status = "FACEBOOK: inviteCancelled - Check log for details";
                LastResponse = "FACEBOOK: inviteCancelled Response:\n" + result.RawResult;
                OnShareLinkResult.SafeInvoke(LastResponse);
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                Status = "FACEBOOK: inviteSuccess - Check log for details";
                LastResponse = "FACEBOOK: inviteSuccess Response:\n" + result.RawResult;
            }
            else
            {
                LastResponse = "FACEBOOK: inviteEmpty Response\n";
                OnShareLinkResult.SafeInvoke(LastResponse);

            }

            Debug.Log(string.Format("FACEBOOK: Status = {0},  Responce = {1}", Status, LastResponse));
        }

        public static void UItween(TweenScale tween_)
        {


            if (ENUM_UI_TWEEN_DIR.E_LEFT == GameEntryHelper.S_TWEEN_DIR)
            {
                tween_.From.x = 1564.0f;
            }
            else
            {
                tween_.From.x = -1571.0f;
            }

            tween_.ResetAndPlay();
            tween_.PlayForward();
        }

        public static void ShowLoading(bool val_)
        {
            if (val_)
            {
                FrameMgr.OpenUIParams ui_param = new FrameMgr.OpenUIParams(UIDefine.UI_SYNC_LOADING)
                {
                    Param = false,
                };
                EngineCore.EngineCoreEvents.UIEvent.ShowUIAndGetFrameWithParam.SafeInvoke(ui_param);
                GameEvents.System_Events.SetLoadingTips.SafeInvoke("Syncing...");
            }
            else
            {
                EngineCore.EngineCoreEvents.UIEvent.HideUIEvent.SafeInvoke(UIDefine.UI_SYNC_LOADING);
            }
        }


        public static void TestNetwork()
        {
            FrameMgr.Instance.StartCoroutine(HttpTest());
        }

        public static void ResetTween(GameObject obj)
        {
            UITweenerBase[] tweens = obj.GetComponents<UITweenerBase>();

            foreach (var item in tweens)
            {
                item.ResetAndPlay();
            }
        }

        public static long GetGiftID(long charge_id_)
        {
            ConfCharge charge = ConfCharge.Get(charge_id_);

            if (null == charge)
                return -1L;

            ConfPush push_gift = ConfPush.array.Find((item) => item.chargeid == charge.id);

            if (null == push_gift)
                return -1L;

            return push_gift.giftid;
        }


        static IEnumerator HttpTest()
        {
            WWW www = new WWW("http://game.fotoable-conan.com/api-web/confirm/api");
            yield return www;

            if (www.error != null)
            {
                GameEvents.NetworkWatchEvents.NetError.SafeInvoke();
            }
            else
            {
                GameEvents.NetworkWatchEvents.NetPass.SafeInvoke();
            }
        }

        public static void OpenGift(long propID, int num)
        {
            ConfProp confProp = ConfProp.Get(propID);
            if (confProp == null)
            {
                return;
            }
            if (confProp.type == 3 && confProp.bindType == 1) //
            {
                MessageHandler.RegisterMessageHandler(MessageDefine.SCDropResp, OnOpenGiftCallback);
                //MessageHandler.RegisterMessageHandler(MessageDefine.SCAutoOpenGiftDropResp, OnOpenGiftCallback);

                EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.gift_open.ToString());
                CSDropReq req = new CSDropReq();
                req.PropId = confProp.id;
                req.Count = num;
                GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
            }
        }

        public static void OnOpenAutoGiftCallback(object obj)
        {
            if (obj is SCAutoOpenGiftDropResp)
            {
                SCAutoOpenGiftDropResp res = (SCAutoOpenGiftDropResp)obj;

                foreach (var item in res.DropInfos)
                {
                    GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(item.PropId, item.Count);
                }

                GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
                param.Param = res;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            }
        }
        private static void OnOpenGiftCallback(object obj)
        {
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCDropResp, OnOpenGiftCallback);
            //MessageHandler.UnRegisterMessageHandler(MessageDefine.SCAutoOpenGiftDropResp, OnOpenGiftCallback);
            if (obj is SCDropResp)
            {
                SCDropResp res = (SCDropResp)obj;

                foreach (var item in res.DropInfos)
                {
                    GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(item.PropId, item.Count);
                }

                GameEvents.UIEvents.UI_GameEntry_Event.Listen_OnCombinePropCollected.SafeInvoke();

                FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
                param.Param = res;
                EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            }
            //else if (obj is SCAutoOpenGiftDropResp)
            //{
            //    SCAutoOpenGiftDropResp res = (SCAutoOpenGiftDropResp)obj;
            //    FrameMgr.OpenUIParams param = new FrameMgr.OpenUIParams(UIDefine.UI_GIFTRESULT);
            //    param.Param = res;
            //    EngineCoreEvents.UIEvent.ShowUIEventWithParam.SafeInvoke(param);
            //}
        }



        public static bool IsStringValid(string info_)
        {
            return (!string.IsNullOrEmpty(info_)) && (!string.IsNullOrWhiteSpace(info_)) && (!"null".Equals(info_));
        }


        private static void GetPlayMode(int scene_play_mode_, out int wan, out int qian, out int ban)
        {
            wan = scene_play_mode_ / 10000;  //0普通(不组合)，1迷雾，2黑天
            qian = scene_play_mode_ / 1000 % 10;  //1普通，2反词，3剪影
            ban = scene_play_mode_ / 100 % 10;  //0普通， 1多物
        }

        public static string GetModeIconName(long scene_id)
        {

            string prefix = "icon_playmode";
            if (scene_id < 20000000)
            {
                int play_mode = ConfScene.Get(scene_id).playMode;

                int wan, qian, bai;

                GetPlayMode(play_mode, out wan, out qian, out bai);


                string wan_str = string.Empty;

                if (0 == wan)
                {
                    wan_str = "pt";
                }
                else if (1 == wan)
                {
                    wan_str = "miwu";
                }
                else if (2 == wan)
                {
                    wan_str = "hei";
                }


                string qian_str = string.Empty;

                if (1 == qian)
                {
                    qian_str = "pt";
                }
                if (2 == qian)
                {
                    qian_str = "fan";
                }
                else if (3 == qian)
                {
                    qian_str = "jian";
                }

                string bai_str = string.Empty;

                if (0 == bai)
                {
                    bai_str = "pt";
                }
                if (1 == bai)
                {
                    bai_str = "duo";
                }


                return string.Format("{0}_{1}_{2}_{3}.png", prefix, wan_str, qian_str, bai_str);

            }
            else
            {
                prefix = "icon_playmode_jigsaw";
                return LocalizeModule.Instance.GetString("icon_playmode_jigsaw");
            }



        }



        public static string GetModeName(long scene_id)
        {

            string suffix = LocalizeModule.Instance.GetString("scene_mode");
            if (scene_id < 20000000)
            {
                int play_mode = ConfScene.Get(scene_id).playMode;

                int wan, qian, bai;

                GetPlayMode(play_mode, out wan, out qian, out bai);


                string wan_str = string.Empty;

                if (0 == wan)
                {
                    wan_str = LocalizeModule.Instance.GetString("scene_mode_1000");// "pt";
                }
                else if (1 == wan)
                {
                    wan_str = LocalizeModule.Instance.GetString("scene_mode_11000");//"miwu";
                }
                else if (2 == wan)
                {
                    wan_str = LocalizeModule.Instance.GetString("scene_mode_21000");//"hei";
                }


                string qian_str = string.Empty;
                if (0 != wan)
                {
                    if (1 == qian)
                    {
                        qian_str = string.Empty; //LocalizeModule.Instance.GetString("scene_mode_1000");//"pt";
                    }
                    if (2 == qian)
                    {
                        qian_str = LocalizeModule.Instance.GetString("scene_mode_2000");//"fan";
                    }
                    else if (3 == qian)
                    {
                        qian_str = LocalizeModule.Instance.GetString("scene_mode_3000");//"jian";
                    }
                }
                string bai_str = string.Empty;

                if (0 != wan)
                {
                    if (0 == bai)
                    {
                        bai_str = string.Empty; //LocalizeModule.Instance.GetString("scene_mode_1000");//"pt";
                    }
                    if (1 == bai)
                    {
                        bai_str = LocalizeModule.Instance.GetString("scene_mode_100");//"duo";
                    }
                }

                return string.Format("{0}{1}{2}{3}", wan_str, qian_str, bai_str, suffix);

            }
            else
            {
                return LocalizeModule.Instance.GetString("UI.jigsaw");
            }



        }



    }

    public class OutPutItemView : GameUIComponent
    {
        public GameImage m_icon;
        public GameLabel m_num;

        protected override void OnInit()
        {
            m_icon = Make<GameImage>("Image");
            m_num = Make<GameLabel>("Text");
        }

    }

    public class EnterSceneData
    {
        public long SceneID;
        public ChapterInfo SceneBelongChapterInfo;
        public long taskConfID;
    }



}
