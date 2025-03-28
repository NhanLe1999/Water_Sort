using UnityEngine;
#if ADS_ADMOB
using GoogleMobileAds.Api;
#endif
using System;


namespace D2S.Ads
{
    public class AdsAdmob : AdsController
    {
        [Serializable]
        public class AdmobId
        {
            public string _appId;
            public string _openAppId;
            public string _bannerId;
            public string _interstitialId;
            public string _rewardedId;
        }

#if ADS_ADMOB
        private AppOpenAd appOpenAd;
        private BannerView bannerView;
        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;
        private bool isBannerShowed;

#if UNITY_ANDROID
        public AdmobId androidId;
#elif UNITY_IOS
        public AdmobId iosId;
#endif
        private AdmobId currentId;

        private DateTime loadOpenAdTime;
#endif

        protected override void RealInitAds()
        {
#if ADS_ADMOB
#if UNITY_ANDROID
            currentId = androidId;
#elif UNITY_IOS
            currentId = iosId;
#else
            Debug.LogError("Not detect platform!!!!");
#endif
            if (Debug.isDebugBuild)
            {
                currentId = new AdmobId(); // test Ads
#if UNITY_ANDROID
                currentId._bannerId = "ca-app-pub-3940256099942544/6300978111";
                currentId._openAppId = "ca-app-pub-3940256099942544/3419835294";
                currentId._interstitialId = "ca-app-pub-3940256099942544/1033173712";
                currentId._rewardedId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
                currentId._bannerId = "ca-app-pub-3940256099942544/2934735716";
                currentId._openAppId = "ca-app-pub-3940256099942544/5662855259";
                currentId._interstitialId = "ca-app-pub-3940256099942544/4411468910";
                currentId._rewardedId = "ca-app-pub-3940256099942544/1712485313";
#endif
            }
#endif


#if ADS_ADMOB
            MobileAds.Initialize(initStatus =>
            {
                Debug.Log("Admob is ready!");
            });
#endif
            isAutoReloadAdsWhenLoadFailed = false;
            //Application.runInBackground = false;
        }

#if ADS_ADMOB
        #region OpenApp events
        private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
        {
            OnCloseOpenAppAds();
            Debug.Log("Closed ad open app");
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            if (this.appOpenAd != null)
            {
                this.appOpenAd.Destroy();
                this.appOpenAd = null;
            }
            isShowingAdOpenApp = false;
            LoadOpenApp();
        }

        private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
        {
            Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            if (this.appOpenAd != null)
            {
                this.appOpenAd.Destroy();
                this.appOpenAd = null;
            }
            LoadOpenApp();
        }

        private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
        {
            Debug.Log("Displayed app open ad");
            isShowingAdOpenApp = true;
        }

        private void HandleAdDidRecordImpression(object sender, EventArgs args)
        {
            Debug.Log("Recorded ad impression");
        }

        private void HandlePaidEvent(object sender, AdValueEventArgs args)
        {
            Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                    args.AdValue.CurrencyCode, args.AdValue.Value);
        }
        #endregion
        protected override bool IsLoadedOpenAppAd()
        {           
            return appOpenAd != null && (DateTime.UtcNow - loadOpenAdTime).TotalHours < 4;
        }

#endif
        protected override void RealLoadOpenApp()
        {
#if ADS_ADMOB
            if (IsLoadedOpenAppAd()) return;
            AdRequest request = new AdRequest.Builder().Build();
            // Load an app open ad for portrait orientation
            AppOpenAd.LoadAd(currentId._openAppId, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
            {
                if (error != null)
                {
                    // Handle the error.
                    Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                    return;
                }

                // App open ad is loaded.
                this.appOpenAd = appOpenAd;
                loadOpenAdTime = DateTime.UtcNow;
            }));
#endif
        }

        protected override void RealShowOpenApp()
        {
           
#if ADS_ADMOB
            base.RealShowOpenApp();
            if (!IsLoadedOpenAppAd() || isShowingAdOpenApp)
            {
                return;
            }
            appOpenAd.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
            appOpenAd.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
            appOpenAd.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
            appOpenAd.OnAdDidRecordImpression += HandleAdDidRecordImpression;
            appOpenAd.OnPaidEvent += HandlePaidEvent;

            appOpenAd.Show();
#endif
        }

#if ADS_ADMOB
        #region Banner events
        private void HandleOnAdLoaded(object sender, EventArgs args)
        {
            base.BannerLoadedEvent();
            RealShowBanner();
        }
        private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            base.BannerLoadFailedEvent();
        }
        private void BannerView_OnAdOpening(object sender, EventArgs e)
        {
            this.BannerClickedEvent();
        }
        #endregion
#endif

        protected override void RealLoadBanner()
        {
#if ADS_ADMOB
            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            bannerView = new BannerView(currentId._bannerId, adaptiveSize, AdPosition.Bottom);
            this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
            this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
            this.bannerView.OnAdOpening += BannerView_OnAdOpening;

            AdRequest request = new AdRequest.Builder().Build();
            this.bannerView.LoadAd(request);
#endif
        }


        protected override void RealShowBanner()
        {
           // if (isBannerShowed) return;

            base.RealShowBanner();
#if ADS_ADMOB
            isBannerShowed = true;
            bannerView.Show();
#endif
        }
        protected override void RealHideBanner()
        {
            base.RealHideBanner();
#if ADS_ADMOB
            isBannerShowed = false;
            bannerView.Hide();
#endif
        }
        protected override void RealDestroyBanner()
        {
#if ADS_ADMOB
            isBannerShowed = false;
            bannerView.Destroy();
#endif
        }


#if ADS_ADMOB
        #region Inter events
        private void Interstitial_HandleOnAdLoaded(object sender, EventArgs args)
        {
            base.InterstitialLoadedEvent();
        }
        private void Interstitial_HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            base.InterstitialLoadFailedEvent();
        }
        private void Interstitial_HandleOnAdClosed(object sender, EventArgs args)
        {
            base.InterstitialClosedEvent();
        }
        #endregion
#endif
        protected override void RealLoadInterstitial()
        {
#if ADS_ADMOB
            this.interstitialAd = new InterstitialAd(currentId._interstitialId);
            this.interstitialAd.OnAdLoaded += Interstitial_HandleOnAdLoaded;
            this.interstitialAd.OnAdFailedToLoad += Interstitial_HandleOnAdFailedToLoad;
            this.interstitialAd.OnAdClosed += Interstitial_HandleOnAdClosed;
            AdRequest request = new AdRequest.Builder().Build();
            this.interstitialAd.LoadAd(request);
#if UNITY_EDITOR
            Interstitial_HandleOnAdLoaded(null, null);
#endif
#endif
        }
        protected override void RealShowInterstitial()
        {
            base.RealShowInterstitial();
#if ADS_ADMOB
            this.interstitialAd.Show();
#endif
        }

#if ADS_ADMOB
        #region Reward events
        private void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            base.RewardedLoadedEvent();
        }
        private void RewardedAd_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            base.RewardedLoadFailedEvent();
        }
        private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
        {
            base.RewardedVideoShowFailedEvent();
        }

        private void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            base.RewardedVideoClosedEvent();
        }

        private void HandleUserEarnedReward(object sender, Reward args)
        {
            base.RewardedVideoRewarededEvent();
        }
        #endregion
#endif
        protected override void RealLoadVideoReward()
        {
#if ADS_ADMOB
            this.rewardedAd = new RewardedAd(currentId._rewardedId);
            this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            this.rewardedAd.OnAdFailedToLoad += RewardedAd_OnAdFailedToLoad;
            this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            AdRequest request = new AdRequest.Builder().Build();
            this.rewardedAd.LoadAd(request);
#endif
        }


        public override bool IsLoadedVideoReward()
        {
#if ADS_ADMOB
#if UNITY_EDITOR
            return true;
#else
            return rewardedAd.IsLoaded();
#endif
#else
            return false;
#endif
        }
        protected override void RealShowVideoReward()
        {
            base.RealShowVideoReward();
#if ADS_ADMOB
            rewardedAd.Show();
#endif
        }

    }
}