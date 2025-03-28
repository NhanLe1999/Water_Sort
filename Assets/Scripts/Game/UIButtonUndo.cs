using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class UIButtonUndo : MonoBehaviour
    {
        private int numberItemReward = 5;
        [SerializeField] private Text txtCountItem;
        [SerializeField] private GameObject objAds;
        [SerializeField] private Image icon;
        private Button undoButton;

        void Awake()
        {
            undoButton = GetComponent<Button>();
            undoButton.onClick.AddListener(OnBtnUseItem_Clicked);

        }

        private void Start()
        {
            CheckUndoEnable();
            LevelManager.Instance.OnChangeHolderState += OnChangeHolderState;
            LevelManager.Instance.UpdateNumberItem += UpdateUIItem;
            UpdateUIItem();
        }

       
        void UpdateUIItem()
        {
            int count = GameStatics.ITEM_UNDO;
            objAds.SetActive(count <= 0);
            txtCountItem.text = count.ToString();
        }
        private void OnChangeHolderState()
        {
            CheckUndoEnable();
        }

        private void OnBtnUseItem_Clicked()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (LevelManager.Instance.Level.no <= 2 || LevelManager.Instance.CurrentState != LevelManager.State.Playing)
                return;
            if (!LevelManager.Instance.HaveUndo)
                return;

            if (GameStatics.ITEM_UNDO > 0)
            {               
                UseItem();
            }
            else if (WaterSort.GameManager.IsTestMode)
            {
                GameStatics.ITEM_UNDO += 1;
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
                        GameStatics.ITEM_UNDO += numberItemReward;
                        UseItem();
                    }
                });
            }
        }
        private void UseItem()
        {
            LevelManager.Instance.Help_Undo();
            CheckUndoEnable();
            GameStatics.ITEM_UNDO--;
            UpdateUIItem();
        }

        private void CheckUndoEnable()
        {
            SetIconColor(LevelManager.Instance.HaveUndo);
            if (LevelManager.Instance.HaveUndo)
            {
                undoButton.interactable = true;

            }
            else
            {
                undoButton.interactable = false;
            }
        }

        private void SetIconColor(bool isEnable)
        {
            var tempColor = icon.color;
            tempColor.a = isEnable ? 1.0f : 0.5f;
            icon.color = tempColor;
            txtCountItem.color = tempColor;
        }
        private void OnDestroy()
        {
            LevelManager.Instance.OnChangeHolderState -= OnChangeHolderState;
            LevelManager.Instance.UpdateNumberItem -= UpdateUIItem;
            undoButton.onClick.RemoveListener(OnBtnUseItem_Clicked);
        }
    }
}