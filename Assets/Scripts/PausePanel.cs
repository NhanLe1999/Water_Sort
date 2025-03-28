using WaterSort;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WaterSort
{
    public class PausePanel : MonoBehaviour
    {
        [Header("Shop")]
        [SerializeField] private Button _shopBtn;

        [Header("Sound")]
        [SerializeField] private Button _soundBtn;
        [SerializeField] private Image imageButtonSound;

        [Header("Music")]
        [SerializeField] private Button _musicBtn;
        [SerializeField] private Image imageButtonMusic;



        [Header("Other")]
        [SerializeField] private Button _removeAdsBtn;
        [SerializeField] private Button _rateBtn;
        [SerializeField] private Button _shareBtn;

       


        [Header("Close")]
        [SerializeField] private Button _closeBtn;
       

        [Header("Image BackGround Active")]
        [SerializeField] private Sprite[] arraySpriteMusic;
        [Header("Image BackGround Active")]
        [SerializeField] private Sprite[] arraySpriteSound;

        [Header("txtLevel")]
        [SerializeField] Text txtLevel;
        private void Awake()
        {
            _shopBtn.onClick.AddListener(OnClickShop);

            _soundBtn.onClick.AddListener(OnClickSound);
            _musicBtn.onClick.AddListener(OnClickMusic);

            _removeAdsBtn.onClick.AddListener(OnClickRemoveAds);
            _rateBtn.onClick.AddListener(OnClickRate);
            _shareBtn.onClick.AddListener(OnClickShare);

           

            _closeBtn.onClick.AddListener(OnClickCloseButton);

            ShopPanel.OnBuyRemoveAds += ShopPanel_OnBuyRemoveAds;
        }

        private void ShopPanel_OnBuyRemoveAds()
        {
            _removeAdsBtn.gameObject.SetActive(ResourceManager.EnableAds);
            _shareBtn.gameObject.SetActive(!ResourceManager.EnableAds);
        }

        private void OnEnable()
        {

            _removeAdsBtn.gameObject.SetActive(ResourceManager.EnableAds);            
            _shareBtn.gameObject.SetActive(!ResourceManager.EnableAds);

            AudioManagerOnSoundStateChanged(AudioManager.IsSoundEnable);
            AudioManager.SoundStateChanged += AudioManagerOnSoundStateChanged;

            AudioManagerOnMusicStateChanged(AudioManager.IsMusicEnable);
            AudioManager.MusicStateChanged += AudioManagerOnMusicStateChanged;

            VibrationManager_VibrationStateChanged(VibrationManager.IsVibrationEnable);
            VibrationManager.VibrationStateChanged += VibrationManager_VibrationStateChanged;
        }

        public void Show()
        {

            if (LevelManager.Instance.GameMode == GameMode.Undefined)         
                txtLevel.text = "Level " + LevelManager.Instance.Level.no.ToString();
            else
                txtLevel.text = "Level " + $"{DailyChallenge.Instance.LevelInOrder} / 5";

            gameObject.SetActive(true);
           

        }

        private void OnClickShop()
        {
            UIManager.Instance.ShowShop();
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }

        private void OnClickSound()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);

            AudioManager.IsSoundEnable = !AudioManager.IsSoundEnable;
                      
        }

        private void OnClickMusic()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            AudioManager.IsMusicEnable = !AudioManager.IsMusicEnable;
        }
        private void AudioManagerOnSoundStateChanged(bool active)
        {
            imageButtonSound.sprite = active ? arraySpriteSound[1] : arraySpriteSound[0];
            imageButtonSound.SetNativeSize();
        }

        private void AudioManagerOnMusicStateChanged(bool active)
        {
            imageButtonMusic.sprite = active ? arraySpriteMusic[1] : arraySpriteMusic[0];
            imageButtonMusic.SetNativeSize();
        }

        private void OnClickRemoveAds()
        {
            IAPManager.Instance.BuyProductID(IAPManager.REMOVE_ADS, () =>
            {
                ResourceManager.EnableAds = false;
                D2S.Ads.AdsController.Instance.DestroyBanner();
                _removeAdsBtn.gameObject.SetActive(ResourceManager.EnableAds);
                _shareBtn.gameObject.SetActive( !ResourceManager.EnableAds);
            });

            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }
        private void OnClickRate()
        {
            D2S.Ads.AdsController.Instance.LeftApplicationWithoutShowAdsWhenComeBack();          
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            GameConfig.Rated = true;
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
           // StartCoroutine(IARManager.Instance.RateImmediatel());
        }
        private void OnClickShare()
        {
            D2S.Ads.AdsController.Instance.LeftApplicationWithoutShowAdsWhenComeBack();
            string txtShare = "Download and play with me!\n" + $"market://details?id={Application.identifier}";
#if UNITY_IOS
        txtShare = "Download and play with me!\n" + $"itms - apps://itunes.apple.com/app/{GameConfig.APPLE_ID}";
#endif
            UnityNative.Sharing.Example.UnityNativeSharingHelper.ShareText(txtShare);
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }

        private void OnClickVibration()
        {
            VibrationManager.IsVibrationEnable = !VibrationManager.IsVibrationEnable;
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }
        private void VibrationManager_VibrationStateChanged(bool active)
        {
            
        }

        private void OnClickCloseButton()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);           
            gameObject.SetActive(false);                  
        }

        private void OnDisable()
        {
            AudioManager.SoundStateChanged -= AudioManagerOnSoundStateChanged;
            VibrationManager.VibrationStateChanged -= VibrationManager_VibrationStateChanged;

        }
        private void OnDestroy()
        {
            _shopBtn.onClick.RemoveListener(OnClickShop);

            _soundBtn.onClick.RemoveListener(OnClickSound);
            _musicBtn.onClick.RemoveListener(OnClickMusic);

            _removeAdsBtn.onClick.RemoveListener(OnClickRemoveAds);
            _rateBtn.onClick.RemoveListener(OnClickRate);
            _shareBtn.onClick.RemoveListener(OnClickShare);

           

            _closeBtn.onClick.RemoveListener(OnClickCloseButton);

            ShopPanel.OnBuyRemoveAds -= ShopPanel_OnBuyRemoveAds;
        }
    }
}






