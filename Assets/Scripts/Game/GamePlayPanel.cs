using WaterSort;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using Spine.Unity;
using System;

namespace WaterSort
{
    public class GamePlayPanel : MonoBehaviour
    {
        [SerializeField] private Text _lvlTxt;
        [SerializeField] private Text txtTutorial;
        [SerializeField] private GameObject tutorialObject;
        [SerializeField] RectTransform topPanel;
        [SerializeField] GameObject buttonSkip;
        [SerializeField] Button buttonChanllenge;
        [SerializeField] RectTransform rectTransformButtonChanllenge;
        [SerializeField] Button buttonNormal;
        [SerializeField] RectTransform rectTransformButtonNormal;
        [SerializeField] GameObject noMoveObject;
        [SerializeField] GameObject levelObject;
        [SerializeField] GameObject backLevelMode;
        private float startAnchorYTopPanel;
        private Image tutorialObjectColor;
        public int CountReplayClick { set; get; }
        public int CountRePlayClickChallenge { set; get; }
        private void Start()
        {
            buttonChanllenge.onClick.AddListener(OnClick_BtnChanllenge);

            buttonNormal.onClick.AddListener(OnClick_BtnNormal);

            startAnchorYTopPanel = topPanel.anchoredPosition.y;
            LevelManager.Instance.OnUserTapHolder += Instance_OnUserTapHolder;
            LevelManager.Instance.OnNotificationMove += OnNotificationMove;
        }
        public void SetUp()
        {
            ShowTextLevel();
            InitTextTutorial();
            if (LevelManager.Instance.Level.no > 5)
            {
                tutorialObject.SetActive(false);
            }
            else
            {
                if (LevelManager.Instance.Level.map.Count > 6)
                    AutoHideTutorial();
            }

        }

        public void TopPanelOut()
        {
            float yTarget = topPanel.anchoredPosition.y + 500;
            topPanel.DOAnchorPosY(yTarget, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                topPanel.gameObject.SetActive(false);
            });
        }

        public void TopPanelIn()
        {
            topPanel.gameObject.SetActive(true);
            topPanel.DOAnchorPosY(-135, 0.5f).SetEase(Ease.Linear);
        }

        private void Instance_OnUserTapHolder()
        {
            backLevelMode.SetActive(false);
            if (LevelManager.Instance.GameMode == GameMode.Undefined)
                rectTransformButtonChanllenge.DOAnchorPosX(-100, 0.5f).SetEase(Ease.Linear);
            if (LevelManager.Instance.Level.no == 1)
            {
                if (LevelManager.Instance.countTutorial == 2)
                {
                    tutorialObject.SetActive(false);
                }
            }
            else if (LevelManager.Instance.Level.no == 2)
            {
                if (LevelManager.Instance.countTutorial == 1)
                {
                    txtTutorial.text = "Any color can be poured into an <color=#FFE641>empty bottle</color>";
                }
                else if (LevelManager.Instance.countTutorial == 2)
                {
                    tutorialObject.SetActive(false);
                }
                else if (LevelManager.Instance.countTutorial == 3)
                {
                    tutorialObject.SetActive(true);
                    txtTutorial.text = "The <color=#682F8E>top color is the same</color>, you can pour!";
                }
                else if (LevelManager.Instance.countTutorial == 5)
                {
                    tutorialObject.SetActive(false);
                }
                else if (LevelManager.Instance.countTutorial == 6)
                {
                    tutorialObject.SetActive(true);
                    txtTutorial.text = "The <color=#FFE641>top color is the same</color>, you can pour!";
                }
                else if (LevelManager.Instance.countTutorial == 8)
                {
                    tutorialObject.SetActive(false);
                }
            }
            else
            {
                SetActiveTutorial(false);
            }
        }


        private void ShowTextLevel()
        {
            if (LevelManager.Instance.GameMode == GameMode.Undefined)
            {
                _lvlTxt.text = LevelManager.Instance.Level.no.ToString();
                buttonNormal.gameObject.SetActive(false);
                DateTime curTime = DailyChallenge.Instance.Now;
                bool isHaveChallenge = DailyChallenge.Instance.LoadCurrentProgress(curTime.Year, curTime.Month, curTime.Day) <= 5;
                buttonChanllenge.gameObject.SetActive(LevelManager.Instance.Level.no > 3 && isHaveChallenge);
                backLevelMode.SetActive(false);
                rectTransformButtonChanllenge.DOAnchorPosX(270, 0.5f).SetEase(Ease.Linear);
            }
            else
            {
                _lvlTxt.text = $"{DailyChallenge.Instance.LevelInOrder} / 5";
                buttonNormal.gameObject.SetActive(true);
                buttonChanllenge.gameObject.SetActive(false);
                GameStatics.BACK_LEVEL_MODE = DailyChallenge.Instance.LevelInOrder;
                backLevelMode.SetActive(GameStatics.BACK_LEVEL_MODE <= 2 && DailyChallenge.Instance.LevelInOrder <= 2 && CountRePlayClickChallenge < 2);
                rectTransformButtonChanllenge.anchoredPosition = new Vector2(-100, -218);
            }
        }

        public void OnClickRestart()
        {
            if (LevelManager.Instance.Level.no <= 2 || LevelManager.Instance.CurrentState != LevelManager.State.Playing)
                return;
            LevelManager.Instance.CurrentState = LevelManager.State.None;
            LevelManager.Instance.DeleteSaveGame();
            if (LevelManager.Instance.GameMode == GameMode.Undefined)
            {
                CountReplayClick++;
                if (CountReplayClick % 2 == 0 && LevelManager.Instance.Level.no > 3 && !WaterSort.GameManager.IsTestMode)
                {
                    D2S.Ads.AdsController.Instance.ShowInterstitial();
                }
            }
            else
            {
                CountRePlayClickChallenge++;
                if (CountRePlayClickChallenge % 2 == 0 && !WaterSort.GameManager.IsTestMode)
                {
                    D2S.Ads.AdsController.Instance.ShowInterstitial();
                }
            }

            LevelManager.Instance.Restart();
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }

        public void OnClickMenu()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (LevelManager.Instance.Level.no <= 2 || LevelManager.Instance.CurrentState != LevelManager.State.Playing)
                return;
            UIManager.Instance.ShowPauseMenu();
        }

        public void OnClickSkip()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (LevelManager.Instance.GameMode == GameMode.Undefined)
                CountReplayClick = 0;
            else
                CountRePlayClickChallenge = 0;

            SetStateButtonSkip();
            if (WaterSort.GameManager.IsTestMode)
                LevelManager.Instance.Skip();
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
                        LevelManager.Instance.Skip();
                    }
                    else
                    {
                        Toast.ShowShortText("Video not finished yet!");
                    }
                });
            }
        }

        private void OnClick_BtnChanllenge()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (LevelManager.Instance.Level.no <= 2 || LevelManager.Instance.CurrentState != LevelManager.State.Playing)
                return;
            LevelManager.Instance.SaveGame();
            UIManager.Instance.SetActiveDaily(true);
        }

        private void OnClick_BtnNormal()
        {
            LevelManager.Instance.SaveGame();
            GameManager.Instance.LoadData();
        }
        private void OnNotificationMove(bool isMove)
        {
            if (LevelManager.Instance.GameMode == GameMode.Undefined)
                buttonSkip.SetActive(!isMove || CountReplayClick >= 2);
            else
                buttonSkip.SetActive(!isMove || CountRePlayClickChallenge >= 2);
           
            noMoveObject.SetActive(!isMove);
            levelObject.SetActive(isMove);
        }

        public void SetStateButtonSkip()
        {
            if (LevelManager.Instance.GameMode == GameMode.Undefined)
                buttonSkip.SetActive(CountReplayClick >= 2);
            else
                buttonSkip.SetActive(CountRePlayClickChallenge >= 2);
        }


        private void InitTextTutorial()
        {

            tutorialObjectColor = tutorialObject.GetComponent<Image>();
            if (LevelManager.Instance.Level.no == 1)
            {
                txtTutorial.text = "Tap to the bottle and pour water <color=#FFE641>of the same color</color>";
                tutorialObject.SetActive(true);
            }
            else if (LevelManager.Instance.Level.no == 2)
            {
                txtTutorial.text = "Only liquids of the same color can be poured into the top of the bottle!";
                tutorialObject.SetActive(true);
            }
            else
            {
                txtTutorial.text = "The top color is the same, you can pour!";
                tutorialObject.SetActive(true);
            }
        }

        private void SetActiveTutorial(bool isActive)
        {
            if (!tutorialObject.activeInHierarchy)
                return;
            float time = 0.5f;
            Color32 color = isActive ? new Color32(0, 0, 0, 204) : new Color32(0, 0, 0, 0);
            tutorialObjectColor.DOColor(color, time).OnComplete(() =>
            {
                tutorialObject.SetActive(isActive);
            });
            Color32 colorText = isActive ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
            txtTutorial.DOColor(colorText, time);
        }

        private void AutoHideTutorial()
        {
            DOVirtual.DelayedCall(2, () =>
            {
                if (tutorialObject.activeInHierarchy)
                    SetActiveTutorial(false);
            });
        }

        private void OnDestroy()
        {
            buttonChanllenge.onClick.RemoveListener(OnClick_BtnChanllenge);
            buttonNormal.onClick.RemoveListener(OnClick_BtnNormal);
            LevelManager.Instance.OnUserTapHolder -= Instance_OnUserTapHolder;
            LevelManager.Instance.OnNotificationMove -= OnNotificationMove;
        }
    }
}