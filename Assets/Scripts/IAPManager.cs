using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace WaterSort
{
    public class IAPManager : Singleton<IAPManager>
    {
        /*
        public const string PACK_1 = "com.watersort.coinpack1";
        public const string PACK_2 = "com.watersort.coinpack2";
        public const string PACK_3 = "com.watersort.coinpack3";
        public const string PACK_4 = "com.watersort.coinpack4";
        */
        public const string REMOVE_ADS = "com.watersort.removeads";


        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        void Start()
        {
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            /*
            builder.AddProduct(PACK_1, ProductType.Consumable);
            builder.AddProduct(PACK_2, ProductType.Consumable);
            builder.AddProduct(PACK_3, ProductType.Consumable);
            builder.AddProduct(PACK_4, ProductType.Consumable);
            */
            builder.AddProduct(REMOVE_ADS, ProductType.NonConsumable);

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
         //   UnityPurchasing.Initialize(this, builder);
        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }



        public void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        public string PriceProduct(string productID)
        {
            if (IsInitialized())
                return m_StoreController.products.WithID(productID).metadata.localizedPriceString;
            else
                return "1.99";
        }
        public bool CanRestorePurchase()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void RestorePurchases()
        {
            if (!IsInitialized())
            {
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                Debug.Log("RestorePurchases started ...");
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result) =>
                {
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                    if (result)
                    {
                        this.OnRestoreCallback?.Invoke();
                    }
                });
            }
            else
            {
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized: PASS");
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            if (OnBuySuccessCallback != null)
            {
                OnBuySuccessCallback();
                OnBuySuccessCallback = null;
            }
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        private Action OnBuySuccessCallback;
        private Action OnRestoreCallback;
        public void BuyProductID(string iapId, Action OnSuccess)
        {
            this.OnBuySuccessCallback = OnSuccess;
            D2S.Ads.AdsController.Instance.LeftApplicationWithoutShowAdsWhenComeBack();
            BuyProductID(iapId);
        }

        public void Restore(Action OnRestoreCallback)
        {
            this.OnRestoreCallback = OnRestoreCallback;
            RestorePurchases();
        }
    }
}