using System;
namespace D2S.Ads
{
    public interface AdsManager
    {
        void InitAds();

        void LoadBanner();
        void ShowBanner();
        void HideBanner();
        void DestroyBanner();

        void LoadInterstitial();
        bool IsLoadedInterstitial();
        void ShowInterstitial();

        void LoadVideoReward();
        bool IsLoadedVideoReward();
        void ShowVideoReward(Action<bool> success);

    }
}