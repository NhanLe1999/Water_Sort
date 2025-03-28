using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class UIButtonExpand : MonoBehaviour
    {
        private int numberItemReward = 1;

        [SerializeField] private RectTransform arrow;
        [SerializeField] private Image icon;
        [SerializeField] private Image imageObjAds;
        [SerializeField] private Text txtCountItem;
        [SerializeField] private GameObject objAds;
        private Button button;
        private bool isExpandEd;
        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnBtnUseItem_Clicked);
            LevelManager.Instance.OnNotificationMove += OnNotificationMove;
            LevelManager.Instance.OnChangeHolderState += OnChangeHolderState;
            LevelManager.Instance.OnResetExpandState += Instance_OnRestart;
            LevelManager.Instance.UpdateNumberItem += UpdateUIItem;
        }

        private void Instance_OnRestart()
        {
            isExpandEd = false;
            button.interactable = !isExpandEd;
            SetIconColor(!isExpandEd);
        }

        void OnEnable()
        {
            UpdateUIItem();
        }
        void UpdateUIItem()
        {
            int count = GameStatics.ITEM_EXPAND;
            objAds.SetActive(count <= 0);
            txtCountItem.gameObject.SetActive(count > 0);
            txtCountItem.text = count.ToString();
        }
        private void OnBtnUseItem_Clicked()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (LevelManager.Instance.Level.no <= 2 || LevelManager.Instance.CurrentState != LevelManager.State.Playing)
                return;
            if (GameStatics.ITEM_EXPAND > 0)
            {
                UseItem();
            }
            else if (WaterSort.GameManager.IsTestMode)
            {
                GameStatics.ITEM_EXPAND += 1;
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
                        GameStatics.ITEM_EXPAND += numberItemReward;
                        UseItem();
                    }
                });
            }
        }
        private void UseItem()
        {
            isExpandEd = true;
            // arrow.gameObject.SetActive(false);
            DOTween.Kill(transform);
            transform.eulerAngles = Vector3.zero;
            button.interactable = !isExpandEd;
            SetIconColor(!isExpandEd);
            LevelManager.Instance.Help_Expand();
            GameStatics.ITEM_EXPAND--;
            UpdateUIItem();
        }

        private void OnNotificationMove(bool isMove)
        {
            if (isExpandEd)
                return;
            if (isMove)
            {
                DOTween.Kill(transform);
                transform.eulerAngles = Vector3.zero;
                //  arrow.gameObject.SetActive(false);              
            }
            else
            {
                transform.DOShakeRotation(5, 30).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                //arrow.gameObject.SetActive(true);
                //arrow.DOAnchorPosY(-150, 0.5f).SetLoops(-1, LoopType.Yoyo);               
            }
        }

        private void OnChangeHolderState()
        {
            if (isExpandEd)
                return;
            SetIconColor(!LevelManager.Instance.IsHavePouring);
            button.interactable = !LevelManager.Instance.IsHavePouring;
        }


        private void SetIconColor(bool isEnable)
        {
            var tempColor = icon.color;
            tempColor.a = isEnable ? 1.0f : 0.5f;
            icon.color = tempColor;
            imageObjAds.color = tempColor;
            txtCountItem.color = tempColor;
        }

        private void OnDestroy()
        {
            LevelManager.Instance.OnNotificationMove -= OnNotificationMove;
            LevelManager.Instance.OnChangeHolderState -= OnChangeHolderState;
            LevelManager.Instance.OnResetExpandState -= Instance_OnRestart;
            LevelManager.Instance.UpdateNumberItem -= UpdateUIItem;
            button.onClick.RemoveListener(OnBtnUseItem_Clicked);
        }
    }
}