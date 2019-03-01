//#define PLATFORM_ID
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// You must obfuscate your secrets using Window > Unity IAP > Receipt Validation Obfuscator
// before receipt validation will compile in this sample.
#define RECEIPT_VALIDATION
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using EngineCore;

#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif
namespace SeekerGame
{
    public class IAPTools : MonoSingleton<IAPTools>, IStoreListener
    {
        public ENUM_BILLING_ERROR billing_error = ENUM_BILLING_ERROR.E_NONE;

        /// <summary>
        /// product id , receipt content
        /// </summary>
        public SafeAction<string, string> m_Usr_ReqVerifyTransaction;
        public SafeFunc<List<IAPProduct>> m_Usr_Get_Products;

        public SafeAction<string, string, string> m_Usr_Transction_Done_IOS;
        /// <summary>
        /// 反馈： 产品id，货币种类，价格
        /// </summary>
        public SafeAction<long, string, string> m_Usr_Transction_Done;




        private static IStoreController m_StoreController; // 存储商品信息;
        private static IExtensionProvider m_StoreExtensionProvider; // IAP扩展工具;
        private bool m_PurchaseInProgress = false; // 是否处于付费中;
        private IAppleExtensions m_AppleExtensions = null;
#pragma warning disable 0414
        private bool m_IsGooglePlayStoreSelected;

#if RECEIPT_VALIDATION
        private CrossPlatformValidator validator;
#endif

        private const string C_ITEM_0 = "com.xxx.xxx.productname"; // 注意这里统一小写(IOS和Google Paly 公用);

        public static ConfCharge GetGoodsByPlatformID(string platform_product_id_)
        {

            foreach (var good in ConfCharge.array)
            {

#if UNITY_ANDROID
                if (good.chargeSouceId == platform_product_id_ && (int)IAP_PLATFROM_TYPE.E_GOOGLE_PLAY == good.source)
                {
                    return good;
                }
#elif UNITY_IOS

                if (good.chargeSouceId == platform_product_id_ && (int)IAP_PLATFROM_TYPE.E_APPLE_STORE == good.source)
                {
                    return good;
                }
#else

                if (good.chargeSouceId == platform_product_id_ && (int)IAP_PLATFROM_TYPE.E_GOOGLE_PLAY == good.source)
                {
                    return good;
                }
#endif
            }

            return null;
        }

        public void InitIAP()
        {
            if (m_StoreController == null && m_StoreExtensionProvider == null)
            {
                Debug.Log("UNITY IAP   init iap !!!!!!!!!");
                billing_error = ENUM_BILLING_ERROR.E_NET_ERROR;
                InitUnityPurchase();
            }
        }

        public void RspVerifyTransaction(long charge_id_)
        {
#if UNITY_IOS && PLATFORM_ID
            ConfCharge charge = ConfCharge.Get(charge_id_);

            DoConfirmPendingPurchaseByIDIOS(charge.chargeSouceId);
#else
            DoConfirmPendingPurchaseByID(charge_id_);
#endif


        }


        // 根据ID给购买商品;
        public void BuyProductByID(long charge_id_)
        {
            if (IsInitialized())
            {
                if (m_PurchaseInProgress == true) return;

                Product product = m_StoreController.products.WithID(charge_id_.ToString());
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    m_StoreController.InitiatePurchase(product);
                    m_PurchaseInProgress = true;
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        public void BuyProductByIDIOS(string platform_unique_id_)
        {
            if (IsInitialized())
            {
                if (m_PurchaseInProgress == true) return;

                Product product = m_StoreController.products.WithID(platform_unique_id_);
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    m_StoreController.InitiatePurchase(product);
                    m_PurchaseInProgress = true;
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        public string GetPrice(long charge_id_)
        {
            if (null == m_StoreController)
                return "";
            // 这里可以获取您在AppStore和Google Play 上配置的商品;
            ProductCollection products = m_StoreController.products;

            if (null == products)
                return "";

            Product[] all = products.all;

            for (int i = 0; i < all.Length; i++)
            {
                //Debug.Log("UNITY IAP " + all[i].metadata.localizedTitle + "|" + all[i].metadata.localizedPriceString + "|" + all[i].metadata.localizedDescription + "|" + all[i].metadata.isoCurrencyCode);
                if (all[i].definition.id == charge_id_.ToString())
                {
                    return all[i].metadata.localizedPriceString;
                }
            }

            return "";
        }

        public string GetPriceIOS(string platform_unique_id_)
        {
            if (null == m_StoreController)
                return "";
            // 这里可以获取您在AppStore和Google Play 上配置的商品;
            ProductCollection products = m_StoreController.products;

            if (null == products)
                return "";

            Product[] all = products.all;

            for (int i = 0; i < all.Length; i++)
            {
                //Debug.Log("UNITY IAP " + all[i].metadata.localizedTitle + "|" + all[i].metadata.localizedPriceString + "|" + all[i].metadata.localizedDescription + "|" + all[i].metadata.isoCurrencyCode);
                if (all[i].definition.id == platform_unique_id_)
                {
                    return all[i].metadata.localizedPriceString;
                }
            }

            return "";
        }


        // 确认购买产品成功;
        private void DoConfirmPendingPurchaseByID(long chargeId)
        {
            Product product = m_StoreController.products.WithID(chargeId.ToString());
            if (product != null && product.availableToPurchase)
            {
                //product.metadata.localizedDescription,
                // product.metadata.isoCurrencyCode
                //product.metadata.localizedPrice.ToString(),
                //product.metadata.localizedPriceString,
                Debug.Log("UNITY IAP confirm ok !!!!!!!!!!!!!!!!");

                if (!m_Usr_Transction_Done.IsNull)
                {
                    m_Usr_Transction_Done.SafeInvoke(chargeId, product.metadata.isoCurrencyCode, product.metadata.localizedPrice.ToString());
                }

                if (m_PurchaseInProgress)
                {
                    m_StoreController.ConfirmPendingPurchase(product);
                    m_PurchaseInProgress = false;
                }
                else
                {
                    m_StoreController.ConfirmPendingPurchase(product);
                }
            }

        }


        private void DoConfirmPendingPurchaseByIDIOS(string platform_unique_Id)
        {
            Product product = m_StoreController.products.WithID(platform_unique_Id);
            if (product != null && product.availableToPurchase)
            {
                //product.metadata.localizedDescription,
                // product.metadata.isoCurrencyCode
                //product.metadata.localizedPrice.ToString(),
                //product.metadata.localizedPriceString,
                Debug.Log("UNITY IAP confirm ok !!!!!!!!!!!!!!!!");

                if (!m_Usr_Transction_Done_IOS.IsNull)
                {
                    m_Usr_Transction_Done_IOS.SafeInvoke(platform_unique_Id, product.metadata.isoCurrencyCode, product.metadata.localizedPrice.ToString());
                }

                if (m_PurchaseInProgress)
                {
                    m_StoreController.ConfirmPendingPurchase(product);
                    m_PurchaseInProgress = false;
                }
                else
                {
                    m_StoreController.ConfirmPendingPurchase(product);
                }
            }

        }

        public bool IsInitialized()
        {
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        // 初始化IAP;
        private void InitUnityPurchase()
        {
            if (IsInitialized()) return;

            Debug.Log("UNITY IAP start InitUnityPurchase");
            var module = StandardPurchasingModule.Instance();

            // The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and
            // developer ui (initialization, purchase, failure code setting). These correspond to
            // the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
            module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

            var builder = ConfigurationBuilder.Instance(module);

            Debug.Log("UNITY IAP after create builder");

            // Set this to true to enable the Microsoft IAP simulator for local testing.
            builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;

            m_IsGooglePlayStoreSelected =
                Application.platform == RuntimePlatform.Android && module.appStore == AppStore.GooglePlay;

            Debug.Log("UNITY IAP will get products1");

            if (m_Usr_Get_Products.IsNull)
                return;

            Debug.Log("UNITY IAP will get products2");

            var all_products = m_Usr_Get_Products.SafeInvoke();

            Debug.Log("UNITY IAP after get products");

            if (null == all_products)
                return;

            Debug.Log("UNITY IAP all products count = " + all_products.Count);

            if (0 == builder.products.Count)
            {
                foreach (var item in all_products)
                {
#if UNITY_IOS && PLATFORM_ID
                           builder.AddProduct(item.m_unique_platform_id, item.m_type);
#else
                    builder.AddProduct(item.m_charge_id.ToString(), item.m_type, item.m_cross_platform_ids);
#endif

                }
            }

            //var catalog = ProductCatalog.LoadDefaultCatalog();

            //foreach (var product in catalog.allValidProducts)
            //{
            //    if (product.allStoreIDs.Count > 0)
            //    {
            //        var ids = new IDs();
            //        foreach (var storeID in product.allStoreIDs)
            //        {
            //            ids.Add(storeID.id, storeID.store);
            //        }
            //        builder.AddProduct(product.id, product.type, ids);
            //    }
            //    else
            //    {
            //        builder.AddProduct(product.id, product.type);
            //    }
            //}

            //builder.AddProduct(C_ITEM_0, ProductType.Consumable);

#if RECEIPT_VALIDATION
            string appIdentifier;
#if UNITY_5_6_OR_NEWER
            appIdentifier = Application.identifier;
#else
        appIdentifier = Application.bundleIdentifier;
#endif
            validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(),
                UnityChannelTangle.Data(), appIdentifier);
#endif

            //初始化;
            UnityPurchasing.Initialize(this, builder);
        }

        #region Public Func




        // 恢复购买;
        public void RestorePurchases()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started ...");
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result) =>
                {
                    // 返回一个bool值，如果成功，则会多次调用支付回调，然后根据支付回调中的参数得到商品id，最后做处理(ProcessPurchase); 
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            else
            {
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }
        #endregion

        #region IStoreListener Callback
        // IAP初始化成功回掉函数;
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized Succ !");
            billing_error = ENUM_BILLING_ERROR.E_NONE;

            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
Application.platform == RuntimePlatform.OSXPlayer)
            {
                this.m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            }


            // 这里可以获取您在AppStore和Google Play 上配置的商品;
            ProductCollection products = m_StoreController.products;
            Product[] all = products.all;

            //for (int i = 0; i < all.Length; i++)
            //{
            //    Debug.Log(all[i].metadata.localizedTitle + "|" + all[i].metadata.localizedPriceString + "|" + all[i].metadata.localizedDescription + "|" + all[i].metadata.isoCurrencyCode);

            //}

#if UNITY_IOS
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
#endif
        }

        // IAP初始化失败回掉函数（没有网络的情况下并不会调起，而是一直等到有网络连接再尝试初始化）;
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                    Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                    break;
                case InitializationFailureReason.PurchasingUnavailable:
                    Debug.Log("Billing disabled! Ask the user if billing is disabled in device settings.");
                    billing_error = ENUM_BILLING_ERROR.E_DISABLE;
                    break;
                case InitializationFailureReason.NoProductsAvailable:
                    Debug.Log("No products available for purchase! Developer configuration error; check product metadata!");
                    break;
            }
        }

        // 支付成功处理函数;
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);

#if RECEIPT_VALIDATION // Local validation is available for GooglePlay, Apple, and UnityChannel stores
            if (m_IsGooglePlayStoreSelected ||
                //(m_IsUnityChannelSelected && m_FetchReceiptPayloadOnPurchase) ||
                Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.tvOS)
            {
                try
                {
                    var result = validator.Validate(e.purchasedProduct.receipt);
                    Debug.Log("Receipt is valid. Contents:");
                    foreach (IPurchaseReceipt productReceipt in result)
                    {
                        Debug.Log("UNITY IAP productID : " +  productReceipt.productID);
                        Debug.Log("UNITY IAP purchaseDate : " +productReceipt.purchaseDate);
                        Debug.Log("UNITY IAP transactionID : " +productReceipt.transactionID);

                        GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                        if (null != google)
                        {
                            Debug.Log(google.purchaseState);
                            Debug.Log(google.purchaseToken);
                        }

                        UnityChannelReceipt unityChannel = productReceipt as UnityChannelReceipt;
                        if (null != unityChannel)
                        {
                            Debug.Log(unityChannel.productID);
                            Debug.Log(unityChannel.purchaseDate);
                            Debug.Log(unityChannel.transactionID);
                        }

                        AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                        if (null != apple)
                        {
                            Debug.Log("UNITY IAP originalTransactionIdentifier : " +apple.originalTransactionIdentifier);
                            Debug.Log("UNITY IAP subscriptionExpirationDate : " +apple.subscriptionExpirationDate);
                            Debug.Log("UNITY IAP cancellationDate : " +apple.cancellationDate);
                            Debug.Log("UNITY IAP quantity : " +apple.quantity);
                        }

                        // For improved security, consider comparing the signed
                        // IPurchaseReceipt.productId, IPurchaseReceipt.transactionID, and other data
                        // embedded in the signed receipt objects to the data which the game is using
                        // to make this purchase.
                    }
                }
                catch (IAPSecurityException ex)
                {
                    Debug.Log("Invalid receipt, not unlocking content. " + ex);
                    return PurchaseProcessingResult.Complete;
                }
            }
#endif

            if (null != m_Usr_ReqVerifyTransaction)
                m_Usr_ReqVerifyTransaction.SafeInvoke(e.purchasedProduct.definition.id, e.purchasedProduct.receipt);
            else
                return PurchaseProcessingResult.Complete;

            // 我们自己后台完毕的话，通过代码设置成功(如果是不需要后台设置直接设置完毕，不要设置Pending);
            return PurchaseProcessingResult.Pending;
        }


        // 支付失败回掉函数;
        public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
        {
            Debug.LogError($"UNITY IAP FIALD id = {item.definition.id} , reason : {r.ToString()}");
            m_PurchaseInProgress = false;
        }

        // 恢复购买功能执行回掉函数;
        private void OnTransactionsRestored(bool success)
        {
            Debug.Log("Transactions restored.");
        }

        // 购买延迟提示(这个看自己项目情况是否处理);
        private void OnDeferred(Product item)
        {
            Debug.Log("Purchase deferred: " + item.definition.id);
        }


        #endregion
    }

}
