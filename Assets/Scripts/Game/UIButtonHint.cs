using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class UIButtonHint : MonoBehaviour
    {
        private int numberItemReward = 5;
        [SerializeField] private GameObject objCount;
        [SerializeField] private Text txtCountItem;
        [SerializeField] private GameObject objAds;
        [SerializeField] private Image icon;
        private Button button;
        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnBtnUseItem_Clicked);
            LevelManager.Instance.OnChangeHolderState += OnChangeHolderState;
            UpdateUIItem();
        }
        void UpdateUIItem()
        {
            int count = GameStatics.ITEM_HINT;
            objAds.SetActive(count <= 0);
            objCount.SetActive(count > 0);
            txtCountItem.text = count.ToString();
        }
        private void OnBtnUseItem_Clicked()
        {
            if (LevelManager.Instance.Level.no <= 2)
                return;
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (GameStatics.ITEM_HINT > 0)
            {
                UseItem();
            }
            else if (WaterSort.GameManager.IsTestMode)
            {
                GameStatics.ITEM_HINT += 1;
                UseItem();
            }
            else
            {
                if (!D2S.Ads.AdsController.Instance.IsLoadedVideoReward())
                {
                    Toast.ShowShortText("Sorry no video ads available.Check your internet connection!");
                    return;
                }
                else D2S.Ads.AdsController.Instance.ShowVideoReward((result) =>
                {
                    if (result)
                    {
                        GameStatics.ITEM_HINT += numberItemReward;
                        UseItem();
                    }
                });
            }
        }
        private void UseItem()
        {
            if (LevelManager.Instance.Help_Hint())
            {
                GameStatics.ITEM_HINT--;
                UpdateUIItem();
            }
        }

        private void OnChangeHolderState()
        {
            
            SetIconColor(!LevelManager.Instance.IsHavePouring);
            button.interactable = !LevelManager.Instance.IsHavePouring;
        }


        private void SetIconColor(bool isEnable)
        {
            var tempColor = icon.color;
            tempColor.a = isEnable ? 1.0f : 0.5f;
            icon.color = tempColor;

        }

        private void OnDestroy()
        {
            LevelManager.Instance.OnChangeHolderState -= OnChangeHolderState;
            button.onClick.RemoveListener(OnBtnUseItem_Clicked);
        }
    }
}