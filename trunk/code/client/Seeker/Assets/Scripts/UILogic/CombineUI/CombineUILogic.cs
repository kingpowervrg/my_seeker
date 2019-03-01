using EngineCore;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeekerGame
{
    [UILogicHandler(UIDefine.UI_COMBINE)]
    public class CombineUILogic : BaseViewComponetLogic
    {
        protected CombineView m_view;
        CombineGiftView m_gift_view;
        protected CombineData m_data;

        CombinedObjCache m_3d_obj_cache;
        long m_fixed_prop_id = 0;
        public long Fixed_prop_id
        {
            get { return m_fixed_prop_id; }
        }
        public override void OnPackageRequest(IMessage imsg, params object[] msg_params)
        {
            base.OnPackageRequest(imsg, msg_params);

            if (imsg is CSCombineRequest)
            {
                CSCombineRequest req = imsg as CSCombineRequest;
                req.CombineId = (long)(msg_params[0]);
            }

        }

        public override void OnScResponse(object s)
        {
            base.OnScResponse(s);

            if (s is SCCombineResponse)
            {
                var rsp = s as SCCombineResponse;

                if (MsgStatusCodeUtil.OnError(rsp.Result))
                    return;

                if (false == rsp.Success)
                {
                    PopUpManager.OpenNormalOnePop(LocalizeModule.Instance.GetString("slots_luckTxt"));
                }
                else
                {
                    var raw_req = EngineCoreEvents.SystemEvents.GetRspPairReq.SafeInvoke();
                    CSCombineRequest req = raw_req as CSCombineRequest;

                    m_view.RefreshCombinedPropCount(req.CombineId);
                }
            }
        }

        public override void OnShow(object param)
        {
            base.OnShow(param);

            if (null != param)
            {
                //合成任务指定的合成
                m_fixed_prop_id = (long)param;

            }
            else
                m_fixed_prop_id = 0;

            CombineDataManager.Instance.RegisterMessageHandler(); //确保data manager先执行回调
            MessageHandler.RegisterMessageHandler(MessageDefine.SCCombineResponse, OnScResponse);


        }


        private void AddProp(SCPlayerUpLevel msg)
        {
            //foreach (var reward in msg.Rewards)
            //{
            //    ConfProp prop = ConfProp.Get(reward.PropId);

            //    if ((int)PROP_TYPE.E_FUNC == prop.type || (int)PROP_TYPE.E_CHIP == prop.type || (int)PROP_TYPE.E_NROMAL == prop.type || (int)PROP_TYPE.E_ENERGE == prop.type)
            //    {
            //        GlobalInfo.MY_PLAYER_INFO.AddSingleBagInfo(reward.PropId, reward.Num);
            //    }
            //}
        }

        public override void OnHide()
        {
            base.OnHide();
            GameEvents.UIEvents.UI_GameEntry_Event.OnMaskBGVisible.SafeInvoke(false);
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCCombineResponse, OnScResponse);
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_3d_obj_cache = new CombinedObjCache();
            m_view = this.Make<CombineView>("Panel_down");
            m_gift_view = Make<CombineGiftView>("Panel_giftdetail");
            this.SetCloseBtnID("Panel_down:Button_close");
        }


        public override void Dispose()
        {
            base.Dispose();

            foreach (Asset3DObj element in m_3d_obj_cache.AllCachedObjs())
            {
                EngineCoreEvents.ResourceEvent.ReleaseAssetEvent.SafeInvoke(element.m_asset_name, element.m_asset);
            }

            m_3d_obj_cache.Clear();
        }

        public void GetObjFromCache(string obj_name_, Action<string, GameObject> OnLoaded_)
        {
            GameObject ret = m_3d_obj_cache.GetFromCache(obj_name_);

            if (null != ret)
            {
                OnLoaded_?.Invoke(obj_name_, ret);
                return;
            }

            EngineCoreEvents.ResourceEvent.GetAssetEvent.SafeInvoke(obj_name_, (assetName, assetObject) =>
            {
                m_3d_obj_cache.AddToCache(obj_name_, assetName, (GameObject)assetObject);
                OnLoaded_?.Invoke(obj_name_, (GameObject)assetObject);

            }, LoadPriority.HighPrior);
        }

        public void ReqCombine(long id_)
        {
            CSCombineRequest req = new CSCombineRequest();

            this.OnScAsyncRequest(req, id_);

        }

        public void ShowGift(long combine_id_)
        {
            long gift_id = ConfCombineFormula.Get(combine_id_).dropGroupId;
            long drop_id = ConfProp.Get(gift_id).dropout;

            List<DropOutJsonData> datas = CommonHelper.GetFixedDropOuts(drop_id);

            var props = from data in datas
                        select new CombineGiftData() { m_id = data.value, m_num = data.count };

            m_gift_view.Visible = true;
            m_gift_view.Refresh(props);
        }

        public override FrameDisplayMode UIFrameDisplayMode
        {
            get
            {
                return FrameDisplayMode.WINDOWED;
            }
        }
    }
}
