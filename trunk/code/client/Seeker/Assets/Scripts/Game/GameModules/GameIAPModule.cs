//#define PLATFROM_ID
using EngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

namespace SeekerGame
{
    public class GameIAPModule : AbstractModule
    {


        private static GameIAPModule m_instance;



        public static GameIAPModule Instance
        {
            get { return m_instance; }
        }

        public ENUM_BILLING_ERROR GetBillingError()
        {
            return IAPTools.instance.billing_error;
        }

        private void BuyProduct(long charge_id_)
        {
            IAPTools.instance.BuyProductByID(charge_id_);
        }

        private void BuyProductIOS(string platform_unique_id_)
        {
            IAPTools.instance.BuyProductByIDIOS(platform_unique_id_);
        }

        private void DoReqVerifyTransaction(string charge_id_, string receipt_)
        {
#if UNITY_ANDROID
            CSGooglePayChargeRequest req = new CSGooglePayChargeRequest();
            req.JsonData = receipt_;
            req.ProductId = charge_id_;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#elif UNITY_IOS
            CSIOSPayChargeRequest req = new CSIOSPayChargeRequest();
            req.JsonData = receipt_;
            req.ProductId = charge_id_;
            GameEvents.NetWorkEvents.SendMsg.SafeInvoke(req);
#endif
        }

        private void DoRspVerifyTransaction(object msg_)
        {
            if (msg_ is SCGooglePayChargeResponse)
            {
                var rsp = msg_ as SCGooglePayChargeResponse;

                if (!MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
                {
                    Debugger.Log("UNITY IAP purchase rsp product id = " + rsp.ProductId);

                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.buy_cash_success.ToString());

                    ConfCharge good = (!string.IsNullOrEmpty(rsp.ProductId) && !string.IsNullOrWhiteSpace(rsp.ProductId)) ? IAPTools.GetGoodsByPlatformID(rsp.ProductId) : null;

                    long charge_id = null != good ? good.id : 0L;

                    if (0 == charge_id)
                    {
                        TransactionTips(LocalizeModule.Instance.GetString("submit_ok"), false);
                        return;
                    }

                    Debugger.Log("UNITY IAP purchase rsp charge id = " + charge_id);
                    IAPTools.instance.RspVerifyTransaction(charge_id);

                    string good_desc = LocalizeModule.Instance.GetString(good.desc);

                    Debugger.Log("UNITY IAP in transction done charge_id = " + charge_id);

                    GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(new CSPlayerInfoRequest());

                    TransactionTips(good_desc, true);

                    GameEvents.IAPEvents.OnTransactionDone.SafeInvoke(charge_id);

                    long giftID = CommonHelper.GetGiftID(charge_id);
                    if (giftID > 0)
                    {
                        //CommonHelper.OpenGift(giftID, 1);
                    }
                }
                else
                {
                    Debugger.LogError("UNITY IAP error transction done product id = " + rsp.ProductId);

                    ConfCharge good = (!string.IsNullOrEmpty(rsp.ProductId) && !string.IsNullOrWhiteSpace(rsp.ProductId)) ? IAPTools.GetGoodsByPlatformID(rsp.ProductId) : null;

                    if (null != good)
                    {
                        string good_desc = LocalizeModule.Instance.GetString(good.desc);

                        TransactionTips(good_desc, false);

                        IAPTools.instance.RspVerifyTransaction(good.id);
                    }
                    else
                    {
                        var pair_req = EngineCoreEvents.SystemEvents.GetRspPairReq.SafeInvoke();

                        if (pair_req is CSGooglePayChargeRequest)
                        {
                            CSGooglePayChargeRequest req_wrapper = pair_req as CSGooglePayChargeRequest;
                            string charge_id = req_wrapper.ProductId;

                            good = ConfCharge.Get(long.Parse(charge_id));

                            if (null != good)
                            {
                                string good_desc = LocalizeModule.Instance.GetString(good.desc);

                                TransactionTips(good_desc, false);

                                IAPTools.instance.RspVerifyTransaction(good.id);
                            }
                        }


                    }
                }

            }
            else if (msg_ is SCIOSPayChargeResponse)
            {
                var rsp = msg_ as SCIOSPayChargeResponse;

                Debugger.Log("UNITY IAP purchase rsp platform id = " + rsp.ProductId);


                if (!MsgStatusCodeUtil.OnError(rsp.ResponseStatus))
                {
                    EngineCoreEvents.AudioEvents.PlayAudio.SafeInvoke(Audio.AudioType.UISound, GameCustomAudioKey.buy_cash_success.ToString());

                    ConfCharge good = (!string.IsNullOrEmpty(rsp.ProductId) && !string.IsNullOrWhiteSpace(rsp.ProductId)) ? IAPTools.GetGoodsByPlatformID(rsp.ProductId) : null;

                    long charge_id = null != good ? good.id : 0L;

                    if (0 == charge_id)
                    {
                        TransactionTips(LocalizeModule.Instance.GetString("submit_ok"), false);
                        return;
                    }

                    Debugger.Log("UNITY IAP purchase rsp charge id = " + charge_id);
                    IAPTools.instance.RspVerifyTransaction(charge_id);

                    string good_desc = LocalizeModule.Instance.GetString(good.desc);


                    Debugger.Log("in transction done charge_id = " + charge_id);

                    GameEvents.NetWorkEvents.SendAsyncMsg.SafeInvoke(new CSPlayerInfoRequest());

                    TransactionTips(good_desc, true);

                    GameEvents.IAPEvents.OnTransactionDone.SafeInvoke(charge_id);

                    //long giftID = CommonHelper.GetGiftID(charge_id);
                    //if (giftID > 0)
                    //{
                    //    var gifts = PushGiftManager.Instance.GetPushInfosByTurnOnType(ENUM_PUSH_GIFT_BLOCK_TYPE.E_LOGIN);

                    //    if( null != gifts && gifts.Count > 0)
                    //    {
                    //        foreach( var gift in gifts)
                    //        {
                    //            if( giftID ==  gift.PushId )
                    //            {
                    //                gift.Buyed = true;
                    //            }

                    //        }
                    //    }
                    //    //CommonHelper.OpenGift(giftID, 1);
                    //}
                }
                else
                {
                    Debugger.LogError("UNITY IAP error transction done product id = " + rsp.ProductId);


                    ConfCharge good = (!string.IsNullOrEmpty(rsp.ProductId) && !string.IsNullOrWhiteSpace(rsp.ProductId)) ? IAPTools.GetGoodsByPlatformID(rsp.ProductId) : null;

                    if (null != good)
                    {
                        string good_desc = LocalizeModule.Instance.GetString(good.desc);

                        TransactionTips(good_desc, false);

                        IAPTools.instance.RspVerifyTransaction(good.id);
                    }
                    else
                    {
                        var pair_req = EngineCoreEvents.SystemEvents.GetRspPairReq.SafeInvoke();

                        if (pair_req is CSIOSPayChargeRequest)
                        {
                            CSIOSPayChargeRequest req_wrapper = pair_req as CSIOSPayChargeRequest;
#if UNITY_IOS && PLATFROM_ID
                            string platform_unique_id = req_wrapper.ProductId;
                            Debug.Log("失败交易商品 platform unique id " + platform_unique_id);
                            good = IAPTools.GetGoodsByPlatformID(platform_unique_id);
#else
                            string charge_id = req_wrapper.ProductId;
                            Debug.Log("失败交易商品 charge id " + charge_id);
                            good = ConfCharge.Get(long.Parse(charge_id));
#endif
                            if (null != good)
                            {
                                string good_desc = LocalizeModule.Instance.GetString(good.desc);

                                Debug.Log("失败交易商品 " + good_desc);

                                TransactionTips(good_desc, false);

                                IAPTools.instance.RspVerifyTransaction(good.id);
                            }
                        }


                    }
                }


            }
        }

        private void DoTransactionDone(long charge_id_, string currency_, string price_)
        {
            //Confinapppurchase good = this.GetGoodsByProductID(game_product_id_);

            ConfCharge good = this.GetGoodsByChargeID(charge_id_);

            System.Collections.Generic.Dictionary<UBSParamKeyName, object> _params = new System.Collections.Generic.Dictionary<UBSParamKeyName, object>()
                    {
                                { UBSParamKeyName.ContentID, charge_id_},
                                //{ UBSParamKeyName.ContentType, good.type},
                                { UBSParamKeyName.ContentType, 1},
                                { UBSParamKeyName.NumItems, 1},
                                { UBSParamKeyName.Currency, currency_},
                                { UBSParamKeyName.Description, LocalizeModule.Instance.GetString( good.desc) }
                    };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Purchased, float.Parse(price_), _params);
        }


        private void DoTransactionDoneIOS(string platform_unique_id_, string currency_, string price_)
        {
            ConfCharge good = IAPTools.GetGoodsByPlatformID(platform_unique_id_);
            long charge_id_ = good.id;

            System.Collections.Generic.Dictionary<UBSParamKeyName, object> _params = new System.Collections.Generic.Dictionary<UBSParamKeyName, object>()
                    {
                                { UBSParamKeyName.ContentID, charge_id_},
                                //{ UBSParamKeyName.ContentType, good.type},
                                { UBSParamKeyName.ContentType, 1},
                                { UBSParamKeyName.NumItems, 1},
                                { UBSParamKeyName.Currency, currency_},
                                { UBSParamKeyName.Description, LocalizeModule.Instance.GetString( good.desc) }
                    };
            UserBehaviorStatisticsModules.Instance.LogEvent(UBSEventKeyName.Purchased, float.Parse(price_), _params);
        }

        //private Confinapppurchase GetGoodsByProductID(string product_id_)
        //{
        //    foreach (var good in Confinapppurchase.array)
        //    {
        //        if (good.gameid == product_id_)
        //        {
        //            return good;
        //        }
        //    }

        //    return null;
        //}

        private ConfCharge GetGoodsByChargeID(long charge_id_)
        {

            foreach (var item in ConfCharge.array)
            {
#if UNITY_ANDROID
                if ((int)IAP_PLATFROM_TYPE.E_GOOGLE_PLAY != item.source)
                    continue;
#elif UNITY_IOS
                if ((int)IAP_PLATFROM_TYPE.E_APPLE_STORE != item.source)
                    continue;
#else
                if ((int)IAP_PLATFROM_TYPE.E_GOOGLE_PLAY != item.source)
                    continue;
#endif

                if (item.id == charge_id_)
                {
                    return item;
                }
            }

            return null;
        }

        //        private Confinapppurchase GetGoodsByPlatformID(string platform_product_id_)
        //        {

        //            foreach (var good in Confinapppurchase.array)
        //            {

        //#if UNITY_ANDROID
        //                if (good.googleid == platform_product_id_)
        //                {
        //                    return good;
        //                }
        //#elif UNITY_IOS

        //                 if (good.appleid == platform_product_id_)
        //                {
        //                    return good;
        //                }
        //#else

        //                if (good.gameid == platform_product_id_)
        //                {
        //                    return good;
        //                }
        //#endif
        //            }

        //            return null;
        //        }




        private List<IAPProduct> DoGetProducts()
        {
            List<IAPProduct> ret = new List<IAPProduct>();

            Dictionary<long, Dictionary<IAP_PLATFROM_TYPE, string>> goods = new Dictionary<long, Dictionary<IAP_PLATFROM_TYPE, string>>();

            Debug.Log("UNITY IAP charge count = " + ConfCharge.array.Count);

            foreach (var item in ConfCharge.array)
            {



#if UNITY_ANDROID
                if ((int)IAP_PLATFROM_TYPE.E_GOOGLE_PLAY != item.source)
                    continue;
#elif UNITY_IOS
                if ((int)IAP_PLATFROM_TYPE.E_APPLE_STORE != item.source)
                    continue;
#endif

                long charge_id = item.id;
                if (!goods.ContainsKey(charge_id))
                {
                    goods[charge_id] = new Dictionary<IAP_PLATFROM_TYPE, string>();
                }

                goods[charge_id][(IAP_PLATFROM_TYPE)item.source] = item.chargeSouceId;
            }

            foreach (var item in goods)
            {
                IAPProduct product = new IAPProduct()
                {
                    m_charge_id = item.Key,
                    //m_unique_platform_id = ConfCharge.Get(item.Key).chargeSouceId,
                    m_type = ConvertProductType(1),
                    m_cross_platform_ids = new IDs(),
                };

                if (item.Value.ContainsKey(IAP_PLATFROM_TYPE.E_GOOGLE_PLAY))
                {
                    product.m_cross_platform_ids.Add(item.Value[IAP_PLATFROM_TYPE.E_GOOGLE_PLAY], UnityEngine.Purchasing.GooglePlay.Name);
                }

                if (item.Value.ContainsKey(IAP_PLATFROM_TYPE.E_APPLE_STORE))
                {
                    product.m_cross_platform_ids.Add(item.Value[IAP_PLATFROM_TYPE.E_APPLE_STORE], UnityEngine.Purchasing.AppleAppStore.Name);
                }

                ret.Add(product);
            }

            return ret;
        }


        private List<IAPProduct> DoGetProductsIOS()
        {
            List<IAPProduct> ret = new List<IAPProduct>();


            foreach (var item in ConfCharge.array)
            {



#if UNITY_ANDROID
                if ((int)IAP_PLATFROM_TYPE.E_GOOGLE_PLAY != item.source)
                    continue;
#elif UNITY_IOS
                if ((int)IAP_PLATFROM_TYPE.E_APPLE_STORE != item.source)
                    continue;
#endif


                IAPProduct product = new IAPProduct()
                {
                    m_charge_id = item.id,
                    m_unique_platform_id = ConfCharge.Get(item.id).chargeSouceId,
                    m_type = ConvertProductType(1),
                };

                ret.Add(product);
            }

            return ret;
        }

        //private List<IAPProduct> DoGetProducts()
        //{
        //    List<IAPProduct> ret = new List<IAPProduct>();

        //    foreach (var item in Confinapppurchase.array)
        //    {
        //        IAPProduct product = new IAPProduct()
        //        {
        //            m_product_id = item.gameid,
        //            m_type = ConvertProductType(item.type),
        //            m_cross_platform_ids = new IDs
        //                    {
        //                        {item.googleid, UnityEngine.Purchasing.GooglePlay.Name},
        //                        {item.appleid, UnityEngine.Purchasing.AppleAppStore.Name},

        //                     }
        //        };
        //        ret.Add(product);
        //    }

        //    return ret;
        //}

        //private List<IAPProduct> DoGetProducts()
        //{
        //    List<IAPProduct> ret = new List<IAPProduct>();

        //    IAPProduct product = new IAPProduct()
        //    {
        //        m_product_id = "100个金币",
        //        m_type = ProductType.Consumable,
        //        m_cross_platform_ids = new IDs
        //                    {
        //                        {"seeker.coin.100", UnityEngine.Purchasing.GooglePlay.Name},
        //                        {"seeker.coin.100", UnityEngine.Purchasing.AppleAppStore.Name},

        //                     }
        //    };
        //    ret.Add(product);

        //    product = new IAPProduct()
        //    {
        //        m_product_id = "200个金币",
        //        m_type = ProductType.Consumable,
        //        m_cross_platform_ids = new IDs
        //                    {
        //                        {"seeker.cash.100", UnityEngine.Purchasing.GooglePlay.Name},
        //                        {"seeker.cash.100", UnityEngine.Purchasing.AppleAppStore.Name},

        //                     }
        //    };
        //    ret.Add(product);

        //    return ret;
        //}


        private ProductType ConvertProductType(int game_type)
        {
            switch (game_type)
            {
                case 1:
                    return ProductType.Consumable;
                case 2:
                    return ProductType.NonConsumable;
                default:
                    return ProductType.Consumable;
            }
        }

        public override void Start()
        {
            base.Start();

            m_instance = this;
            AutoStart = true;


            IAPTools.instance.m_Usr_ReqVerifyTransaction += DoReqVerifyTransaction;

#if UNITY_IOS && PLATFROM_ID
            IAPTools.instance.m_Usr_Get_Products += DoGetProductsIOS;
#else
            IAPTools.instance.m_Usr_Get_Products += DoGetProducts;
#endif


            IAPTools.instance.m_Usr_Transction_Done_IOS += DoTransactionDoneIOS;
            IAPTools.instance.m_Usr_Transction_Done += DoTransactionDone;


            GameEvents.IAPEvents.Sys_BuyProductEvent += BuyProduct;
            GameEvents.IAPEvents.Sys_BuyProductIOSEvent += BuyProductIOS;
            GameEvents.IAPEvents.Sys_GetPriceEvent += GetPrice;
            GameEvents.IAPEvents.Sys_GetPriceIOSEvent += GetPriceIOS;
            GameEvents.IAPEvents.Sys_GetUSDPriceEvent += GetUSDPrice;

#if UNITY_ANDROID
            MessageHandler.RegisterMessageHandler(MessageDefine.SCGooglePayChargeResponse, DoRspVerifyTransaction);
#elif UNITY_IOS
            MessageHandler.RegisterMessageHandler(MessageDefine.SCIOSPayChargeResponse, DoRspVerifyTransaction);
#else
            MessageHandler.RegisterMessageHandler(MessageDefine.SCGooglePayChargeResponse, DoRspVerifyTransaction);
#endif

        }

        public void InitIAP()
        {
            Debug.Log("UNITY IAP game iap module init iap");
            IAPTools.instance.InitIAP();
        }


        public override void Dispose()
        {
            base.Dispose();
            IAPTools.instance.m_Usr_ReqVerifyTransaction -= DoReqVerifyTransaction;
#if UNITY_IOS && PLATFROM_ID
            IAPTools.instance.m_Usr_Get_Products -= DoGetProductsIOS;
#else
            IAPTools.instance.m_Usr_Get_Products -= DoGetProducts;
#endif
            IAPTools.instance.m_Usr_Transction_Done_IOS -= DoTransactionDoneIOS;
            IAPTools.instance.m_Usr_Transction_Done -= DoTransactionDone;

#if UNITY_ANDROID
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCGooglePayChargeResponse, DoRspVerifyTransaction);
#elif UNITY_IOS
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCIOSPayChargeResponse, DoRspVerifyTransaction);
#else
            MessageHandler.UnRegisterMessageHandler(MessageDefine.SCGooglePayChargeResponse, DoRspVerifyTransaction);
#endif
        }

        private string GetPrice(long charge_id_)
        {
            return IAPTools.instance.GetPrice(charge_id_);
        }

        private string GetPriceIOS(string platform_unique_id_)
        {
            return IAPTools.instance.GetPriceIOS(platform_unique_id_);
        }

        private float GetUSDPrice(long charge_id)
        {
            ConfCharge charge = this.GetGoodsByChargeID(charge_id);

            if (null != charge)
            {
                return float.Parse(charge.cashCount.ToString());
            }

            return 0.0f;

        }


        private void TransactionTips(string desc_, bool ok_)
        {
            PopUpData pd = new PopUpData();
            pd.title = "recharge_title";
            if (ok_)
                pd.content = "recharge_ok";
            else
                pd.content = "recharge_fail";
            pd.content_param0 = desc_;
            pd.isOneBtn = true;

            PopUpManager.OpenPopUp(pd);
        }
    }
}
