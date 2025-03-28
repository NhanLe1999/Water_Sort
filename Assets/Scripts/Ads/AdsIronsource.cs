using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace D2S.Ads
{
    public class AdsIronsource : AdsController
    {
        public string idAds;
        protected override void RealInitAds()
        {
#if ADS_IRONSOURCE
            IronSource.Agent.init(idAds);
            IronSource.Agent.validateIntegration();

            #region Banner events
            IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
            #endregion

            #region Inter events
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
            #endregion


            #region Reward events
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            #endregion
#endif
        }
#if ADS_IRONSOURCE
        void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }
#endif

#if ADS_IRONSOURCE
        #region Banner events
        void BannerAdLoadedEvent()
        {
            base.BannerLoadedEvent();
        }
        void BannerAdLoadFailedEvent(IronSourceError error)
        {
            base.BannerLoadFailedEvent();
        }
        #endregion
#endif

        protected override void RealLoadBanner()
        {
#if ADS_IRONSOURCE
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
#endif
        }
        protected override void RealShowBanner()
        {
            base.RealShowBanner();
#if ADS_IRONSOURCE
            IronSource.Agent.displayBanner();
#endif
        }
        protected override void RealHideBanner()
        {
            base.RealHideBanner();
#if ADS_IRONSOURCE
            IronSource.Agent.hideBanner();
#endif
        }
        protected override void RealDestroyBanner()
        {
#if ADS_IRONSOURCE
            IronSource.Agent.destroyBanner();
#endif
        }


#if ADS_IRONSOURCE
        #region Inter events
        void InterstitialAdReadyEvent()
        {
            base.InterstitialLoadedEvent();
        }
        void InterstitialAdLoadFailedEvent(IronSourceError error)
        {
            base.InterstitialLoadFailedEvent();
        }
        void InterstitialAdShowFailedEvent(IronSourceError error)
        {
            base.InterstitialShowFailedEvent();
        }
        void InterstitialAdClosedEvent()
        {
            base.InterstitialClosedEvent();
        }
        void InterstitialAdShowSucceededEvent()
        {
            base.InterstitialShowSucceededEvent();
        }
        #endregion
#endif

        protected override void RealLoadInterstitial()
        {
            Debug.Log("Ads Real show");
#if ADS_IRONSOURCE
            IronSource.Agent.loadInterstitial();
#endif
        }

        protected override void RealShowInterstitial()
        {
            base.RealShowInterstitial();
#if ADS_IRONSOURCE
            IronSource.Agent.showInterstitial();
#endif
        }

#if ADS_IRONSOURCE
        #region Reward events
        void RewardedVideoAdClosedEvent()
        {
            base.RewardedVideoClosedEvent();
        }
        void RewardedVideoAvailabilityChangedEvent(bool available)
        {
            base.RewardedVideoRewarededEvent();
        }
        void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
        {
            base.RewardedVideoRewarededEvent();
        }
        void RewardedVideoAdShowFailedEvent(IronSourceError error)
        {
            base.RewardedVideoShowFailedEvent();
        }
        #endregion
#endif

        protected override void RealLoadVideoReward()
        {

        }


        public override bool IsLoadedVideoReward()
        {
#if UNITY_EDITOR
            return true;
#endif
#if ADS_IRONSOURCE
            return IronSource.Agent.isRewardedVideoAvailable();
#endif
            return false;
        }

        protected override void RealShowVideoReward()
        {
            base.RealShowVideoReward();
#if ADS_IRONSOURCE
            IronSource.Agent.showRewardedVideo();
#endif
        }
    }
}