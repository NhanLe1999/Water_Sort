using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace WaterSort
{
    public class PopupGetSkin : MonoBehaviour
    {
        [SerializeField] private Button btnClose;
      
        [SerializeField] private Button btnGet;
       

        [Header("Image Item")]
        [SerializeField] Image[] arrayImageItem;
        [SerializeField] Image[] arrayImageMaskItem;

        

        [SerializeField] RectTransform rectTranformFireWork;
        

        [SerializeField] GameObject panelParent;
        private int idBotlle;

        [Header("Anchor")]
        [SerializeField] RectTransform anchorTop;
        void Start()
        {
            btnClose.onClick.AddListener(OnClick_BtnClose);
            btnGet.onClick.AddListener(OnClick_BtnGet);          
           
        }

        private void OnDestroy()
        {
            btnClose.onClick.RemoveListener(OnClick_BtnClose);
            btnGet.onClick.RemoveListener(OnClick_BtnGet);
        }

        private void OnClick_BtnClose()
        {
            Hide();
            LevelManager.Instance.ShopPanel_OnUpdateSkin();
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }

        public void Show(int id)
        {
            idBotlle = id;
            Sprite sprite = ResourceManager.LoadBottle(idBotlle);
            for (int i = 0; i < arrayImageItem.Length; i++)
                arrayImageItem[i].sprite = sprite;
            Sprite maskSprite = ResourceManager.LoadMask(idBotlle);
            for (int i = 0; i < arrayImageMaskItem.Length; i++)
                arrayImageMaskItem[i].sprite = maskSprite;

            rectTranformFireWork.anchoredPosition = new Vector2(0, -650);
            panelParent.SetActive(true);
            gameObject.SetActive(true);
           

        }

        private float timeDelay = 3;
        private float timeCount = 3;
        private float xPosition = 401;
        private int nevative = -1;
        private void Update()
        {
            if (timeCount < timeDelay)
            {
                timeCount += Time.deltaTime;
            }
            else
            {
                rectTranformFireWork.gameObject.SetActive(false);
                timeCount = 0;
                if (xPosition >= 400 || xPosition <= -400)
                    nevative *= -1;
                xPosition += nevative * Random.Range(50, 300);
                if (xPosition > 400)
                    xPosition = 400;
                if (xPosition <- 400)
                    xPosition = -400;

                float y = Random.Range(anchorTop.anchoredPosition.y - 200, anchorTop.anchoredPosition.y);
                rectTranformFireWork.anchoredPosition = new Vector2(xPosition, y);
                rectTranformFireWork.gameObject.SetActive(true);
            }
        }

        private void OnClick_BtnGet()
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
            //UIManager.Instance.ShowClaim(ItemShop.Type.Bottle, idBotlle);
            GameConfig.Unlock(ItemShop.Type.Bottle, idBotlle);
            GameConfig.ID_BOTTLE = idBotlle;
            LevelManager.Instance.ShopPanel_OnUpdateSkin();
            Hide();
        }

        private void Hide()
        {
          

            panelParent.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
