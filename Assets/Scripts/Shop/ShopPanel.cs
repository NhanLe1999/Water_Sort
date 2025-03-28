using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class ShopPanel : MonoBehaviour
    {


        [Header("Button Close")]
        [SerializeField] private Button _closeBtn;

        [Header("Button Bottle")]
        [SerializeField] private Button _bottleBtn;
        [SerializeField] private Image _bottleBase;

        [Header("Button BackGround")]
        [SerializeField] private Button _backGroundBtn;
        [SerializeField] private Image _backGroundBase;

        [Header("Button NoAds")]
        [SerializeField] private Button _noAdsBtn;
        [SerializeField] private Image _noAdsBase;

        [Header("Array Sprite")]
        [SerializeField] private Sprite[] arrayConnerSprite;
        [SerializeField] private Sprite[] arrayMidleSprite;

        [Header("Button Unlock")]
        [SerializeField] private Button _unlockBtn;

        [Header("Button Buy NoAds")]
        [SerializeField] private Button _buyNoAdsBtn;
        [SerializeField] private Text noAdsCost;

        [Header("Object")]
        [SerializeField] private GameObject scrollView;
        [SerializeField] private GameObject noAds;

        [Header("Prefabs")]
        [SerializeField] ItemShop itemPrefab;
        [SerializeField] Transform itemContentTransform;


        private int totalItem;
        private List<ItemShop> listItems = new List<ItemShop>();
        private ItemShop.Type type;
        public static event Action OnUpdateSkin;
        public static event Action OnBuyRemoveAds;
        private void Awake()
        {
            _closeBtn.onClick.AddListener(OnClickCloseButton);
            _bottleBtn.onClick.AddListener(OnClickBottleButton);
            _backGroundBtn.onClick.AddListener(OnClickBackGroundButton);
            _noAdsBtn.onClick.AddListener(OnClickNoAdsButton);
            _unlockBtn.onClick.AddListener(OnUnLock);
            _buyNoAdsBtn.onClick.AddListener(OnBuyNoAds);
        }

        private void OnEnable()
        {
            ShowBottleTab();
            ItemShop.OnSelectItem += ItemShop_OnSelectItem;
        }

        private void ItemShop_OnSelectItem(int idSelected)
        {
            if (type == ItemShop.Type.Bottle)
            {
                GameConfig.ID_BOTTLE = idSelected;
            }
            else
            {
                GameConfig.ID_BACKGROUND = idSelected;
            }
        }

        public void Show()
        {

            gameObject.SetActive(true);

        }
        private void OnClickCloseButton()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            gameObject.SetActive(false);
        }

        private void OnClickBottleButton()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            ShowBottleTab();
        }

        private void ShowBottleTab()
        {
            ResetBaseButton();
            totalItem = GameConfig.TOTAL_BOTTLE;
            type = ItemShop.Type.Bottle;
            UpdateContent();
            listItems[GameConfig.ID_BOTTLE].IsSelected = true;
            _bottleBase.sprite = arrayConnerSprite[0];
            ActiveObject(false);
            UpdateStateUnLock();
        }
        private void OnClickBackGroundButton()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            ResetBaseButton();
            totalItem = GameConfig.TOTAL_BACKGROUND;
            type = ItemShop.Type.BackGround;
            UpdateContent();
            listItems[GameConfig.ID_BACKGROUND].IsSelected = true;
            _backGroundBase.sprite = arrayMidleSprite[0];
            ActiveObject(false);
            UpdateStateUnLock();
        }

        private void UpdateContent()
        {

            for (int count = 0; count < totalItem; count++)
            {
                if (listItems.Count <= count)
                {
                    var item = Instantiate(itemPrefab, itemContentTransform);
                    listItems.Add(item);
                }
                bool isUnLock = GameConfig.IsUnlock(type, count);
                listItems[count].SetData(count, type, !isUnLock);
            }

            for (int count = totalItem; count < listItems.Count; count++)
            {
                listItems[count].SetDisable();
            }
        }

        private void UpdateStateUnLock()
        {
            _unlockBtn.gameObject.SetActive(false);
            for (int count = 0; count < totalItem; count++)
            {
                bool isUnLock = GameConfig.IsUnlock(type, count);
                if (!isUnLock)
                {
                    _unlockBtn.gameObject.SetActive(true);
                    break;
                }
            }
        }

        private void OnClickNoAdsButton()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            ResetBaseButton();
            _noAdsBase.sprite = arrayConnerSprite[0];
            ActiveObject(true);
        }

        private void OnBuyNoAds()
        {
            IAPManager.Instance.BuyProductID(IAPManager.REMOVE_ADS, () =>
            {
                ResourceManager.EnableAds = false;
                D2S.Ads.AdsController.Instance.DestroyBanner();
                _buyNoAdsBtn.gameObject.SetActive(ResourceManager.EnableAds);
                OnBuyRemoveAds?.Invoke();
            });
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }

        private void OnUnLock()
        {
            if (GameManager.IsTestMode)
            {
                GetReward();
            }
            else if (!D2S.Ads.AdsController.Instance.IsLoadedVideoReward())
            {
                Toast.ShowShortText("Rewarded video is not ready!");
            }
            else
            {
                D2S.Ads.AdsController.Instance.ShowVideoReward((result) =>
                {
                    if (result)
                        GetReward();
                    else
                        Toast.ShowShortText("Rewarded video is not finished yet!");
                });
            }
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }

        private void GetReward()
        {
            for (int i = 0; i < totalItem; i++)
            {
                if (listItems[i].IsLock)
                {
                    //UIManager.Instance.ShowClaim(type, i);

                    GameConfig.Unlock(type, listItems[i].ID);
                    listItems[i].IsLock = false;
                    listItems[i].OnPointerClick(null);
                    ItemShop_OnSelectItem(i);
                    UpdateStateUnLock();

                    break;
                }
            }
        }

        private void ResetBaseButton()
        {
            _bottleBase.sprite = arrayConnerSprite[1];
            _backGroundBase.sprite = arrayMidleSprite[1];
            _noAdsBase.sprite = arrayConnerSprite[1];
        }

        private void ActiveObject(bool isNoAdsTab)
        {
            scrollView.SetActive(!isNoAdsTab);
            _unlockBtn.gameObject.SetActive(!isNoAdsTab);
            noAds.SetActive(isNoAdsTab);
            _buyNoAdsBtn.gameObject.SetActive(ResourceManager.EnableAds);
            noAdsCost.text = IAPManager.Instance.PriceProduct(IAPManager.REMOVE_ADS);
        }

        private void OnDisable()
        {
            ItemShop.OnSelectItem -= ItemShop_OnSelectItem;
            OnUpdateSkin?.Invoke();
        }

        private void OnDestroy()
        {
            _closeBtn.onClick.RemoveListener(OnClickCloseButton);
            _bottleBtn.onClick.RemoveListener(OnClickBottleButton);
            _backGroundBtn.onClick.RemoveListener(OnClickBackGroundButton);
            _noAdsBtn.onClick.RemoveListener(OnClickNoAdsButton);
            _buyNoAdsBtn.onClick.AddListener(OnBuyNoAds);
        }
    }
}
