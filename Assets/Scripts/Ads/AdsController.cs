using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D2S.Ads
{
    public class AdsController : MonoBehaviour, AdsManager
    {
        public static AdsController Instance;
        private bool isInited = false;

        protected bool isShowingAdOpenApp;
        protected float intervalTimeReload = 3;

        protected bool isLoadingBanner;
        protected bool isLoadedBanner;
        protected bool isNeedShowBanner;
        protected bool isShowingBanner = false;
        protected int countLoadBanner = 0;

        private const int TIME_INTERVAL_INTERSTITIAL = 60;
        private float nextTimeInterval = 0;
        protected bool isLoadingInter;
        protected bool isLoadedInter;
        protected int countLoadInter = 0;

        protected bool isGetReward;
        protected event Action<bool> callbackReward;


        protected bool isLoadingRewarded;
        protected bool isLoadedRewarded;
        protected int countLoadRewarded = 0;
        protected bool isAutoReloadAdsWhenLoadFailed = true;


        protected bool isLeftApplicationByAds;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
            InitAds();
        }
        public void InitAds()
        {
            if (isInited) return;
            isInited = true;
            RealInitAds();
            LoadVideoReward();
            if (GetRemoveAds()) return;
            LoadOpenApp();
            LoadBanner();
            LoadInterstitial();
        }

        private void OnApplicationPause(bool pause)
        {
            if (GetRemoveAds()) return;
            if (!pause)
            {             
                if (!isLeftApplicationByAds)
                {                  
                    if (IsLoadedOpenAppAd())
                        ShowOpenApp();
                    else //if (isShowingBanner)
                    {                      
                        RealShowBanner();
                    }
                }
                else
                {
                    isLeftApplicationByAds = false;
                    RealShowBanner();
                }
            }
            else
            {
                Debug.Log("home: " + isShowingBanner);
                if (isShowingBanner)
                {
                    RealHideBanner();
                }
            }

        }
        protected virtual void RealInitAds() { }
        private bool GetRemoveAds()
        {
            //return PlayerPrefs.GetInt("noads") == 1;
            return !WaterSort.ResourceManager.EnableAds;
        }

        protected void OnCloseOpenAppAds()
        {
            return;
            if (isShowingBanner)
            {
                RealShowBanner();
            }
            TurnOnOffIngameMusic(true);
        }
        public void LoadOpenApp()
        {
            return;
            if (GetRemoveAds())
                return;
            RealLoadOpenApp();
            
        }
        protected virtual void RealLoadOpenApp()
        {
        }
        protected virtual bool IsLoadedOpenAppAd()
        {
            return false;
        }
        public void ShowOpenApp()
        {
            return;
            if (GetRemoveAds()) return;
            RealShowOpenApp();
        }
        protected virtual void RealShowOpenApp()
        {
            TurnOnOffIngameMusic(false);
        }

        protected void BannerLoadedEvent()
        {
            isLoadingBanner = false;
            isLoadedBanner = true;
            countLoadBanner = 0;

            if (isNeedShowBanner)
            {
                isShowingBanner = true;
                RealShowBanner();
            }
            else
            {
                RealHideBanner();
            }
        }
        protected void BannerLoadFailedEvent()
        {
            isLoadingBanner = false;
            isLoadedBanner = false;
            countLoadBanner++;

            if (isAutoReloadAdsWhenLoadFailed)
                DOVirtual.DelayedCall(intervalTimeReload * countLoadBanner, () =>
                {
                    LoadBanner();
                });
        }
        protected void BannerClickedEvent()
        {
            this.isLeftApplicationByAds = true;
        }
        public void LoadBanner()
        {
            if (GetRemoveAds()) return;
            if (isLoadingBanner || isLoadedBanner)
                return;
            isLoadingBanner = true;
            isLoadedBanner = false;
            RealLoadBanner();
        }
        protected virtual void RealLoadBanner()
        {

        }
        public void ShowBanner()
        {
            if (GetRemoveAds()) return;
            if (isLoadedBanner)
            {
                isShowingBanner = true;
                RealShowBanner();
            }
            else
            {
                isNeedShowBanner = true;
                if (!isLoadingBanner)
                    LoadBanner();
            }
        }
        protected virtual void RealShowBanner()
        {
            Debug.Log("[Ads] Show banner");
        }
        public void HideBanner()
        {
            isShowingBanner = false;
            isNeedShowBanner = false;
            RealHideBanner();
        }
        protected virtual void RealHideBanner()
        {
            Debug.Log("[Ads] Hide banner");
        }
        public void DestroyBanner()
        {
            isShowingBanner = false;
            isNeedShowBanner = false;
            RealDestroyBanner();
        }
        protected virtual void RealDestroyBanner() { }

        protected void InterstitialLoadedEvent()
        {
            isLoadingInter = false;
            isLoadedInter = true;
            countLoadInter = 0;
        }
        protected void InterstitialLoadFailedEvent()
        {
            isLoadingInter = false;
            isLoadedInter = false;
            countLoadInter++;

            if (isAutoReloadAdsWhenLoadFailed)
                DOVirtual.DelayedCall(intervalTimeReload * countLoadInter, () =>
            {
                LoadInterstitial();
            });
        }
        protected void InterstitialShowFailedEvent()
        {
            LoadInterstitial();
            TurnOnOffIngameMusic(true);
        }
        protected void InterstitialClosedEvent()
        {
            TurnOnOffIngameMusic(true);
            if (isShowingBanner)
            {
                RealShowBanner();
            }
            LoadInterstitial();
        }
        protected void InterstitialShowSucceededEvent()
        {
            LoadInterstitial();
        }
        public void LoadInterstitial()
        {
            if (GetRemoveAds()) return;
            if (isLoadingInter || isLoadedInter) return;
            isLoadingInter = true;
            isLoadedInter = false;
            RealLoadInterstitial();
        }
        protected virtual void RealLoadInterstitial() { }
        public bool IsLoadedInterstitial()
        {
#if UNITY_EDITOR
            return true;
#endif
            return isLoadedInter;
        }
        public void ShowInterstitial()
        {
            Debug.Log("Show interstitial ads");
            if (GetRemoveAds())
                return;
            if (IsLoadedInterstitial())
            {
                if (Time.realtimeSinceStartup >= nextTimeInterval)
                {
                    nextTimeInterval = Time.realtimeSinceStartup + TIME_INTERVAL_INTERSTITIAL;
                    isLoadedInter = false;
                    RealShowInterstitial();
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("Not enough time interval: " + (nextTimeInterval - Time.realtimeSinceStartup));
#endif
                }
            }
            else
            {
                if (!isLoadingInter)
                    LoadInterstitial();
            }
        }
        protected virtual void RealShowInterstitial()
        {
            isLeftApplicationByAds = true;
            TurnOnOffIngameMusic(false);
        }

        protected void RewardedLoadedEvent()
        {
            isLoadingRewarded = false;
            isLoadedRewarded = true;
            countLoadRewarded = 0;
        }
        protected void RewardedLoadFailedEvent()
        {
            isLoadingRewarded = false;
            isLoadedRewarded = false;
            countLoadRewarded++;

            if (isAutoReloadAdsWhenLoadFailed)
                DOVirtual.DelayedCall(intervalTimeReload * countLoadRewarded, () =>
            {
                LoadVideoReward();
            });
        }
        protected void RewardedVideoClosedEvent()
        {
            TurnOnOffIngameMusic(true);
            if (isShowingBanner)
            {
                RealShowBanner();
            }
            LoadVideoReward();
            this.callbackReward?.Invoke(isGetReward);           
        }
        protected void RewardedVideoRewarededEvent()
        {
            this.isGetReward = true;
        }
        protected void RewardedVideoShowFailedEvent()
        {
            this.callbackReward?.Invoke(isGetReward);
            TurnOnOffIngameMusic(true);
        }
        public void LoadVideoReward()
        {
            //if (isLoadingRewarded || isLoadedRewarded) return;
            isLoadingRewarded = true;
            isLoadedRewarded = false;
            RealLoadVideoReward();
        }
        protected virtual void RealLoadVideoReward() { }
        public virtual bool IsLoadedVideoReward()
        {
            return true;
        }
        public void ShowVideoReward(Action<bool> callback)
        {
            //#if UNITY_EDITOR
            //            Debug.Log("Fake view reward");
            //            callback?.Invoke(true);
            //            return;
            //#endif
            if (IsLoadedVideoReward())
            {
                isGetReward = false;
                this.callbackReward = callback;
                RealShowVideoReward();
            }
            else
            {
                if (!isLoadingRewarded)
                {
                    LoadVideoReward();
                }
                callback?.Invoke(false);
            }
        }
        protected virtual void RealShowVideoReward()
        {
            isLeftApplicationByAds = true;
            TurnOnOffIngameMusic(false);
        }

        private void TurnOnOffIngameMusic(bool value)
        {
            //Debug.Log("turn on/off music: " + value);
            if (SoundController.Instance != null) SoundController.Instance.MuteMusic(!value);
        }
        public void LeftApplicationWithoutShowAdsWhenComeBack()
        {
            isLeftApplicationByAds = true;
        }
        public void ReloadAllAds()
        {
            if (!IsLoadedVideoReward())
                LoadVideoReward();
            if (GetRemoveAds()) return;
            LoadBanner();
            LoadOpenApp();
            LoadInterstitial();
        }
    }
}