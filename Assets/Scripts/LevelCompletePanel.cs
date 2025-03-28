
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;

namespace WaterSort
{
    public class LevelCompletePanel : MonoBehaviour
    {
        [Header("Button Next")]
        [SerializeField] private Button _nextBtn;

        [Header("EffectLevel")]
        [SerializeField] SkeletonGraphic skeletonGraphicLevel;

        


        [Header("NextButton")]
        [SerializeField] private GameObject buttonNext;
        [SerializeField] private Image backGroundNextButton;
        [SerializeField] private Text textNextButton;

        [SerializeField] RectTransform panelParent;


        [SerializeField] RectTransform panelDailyParent;

        [Header("Effect Daily")]
        [SerializeField] private RectTransform effectDaily;

        [Header("Tile Daily")]
        [SerializeField] private GameObject tileNomal;
        [SerializeField] SkeletonGraphic skeletonGraphicChallengePart;
        [SerializeField] private GameObject tileComplete;
        [SerializeField] SkeletonGraphic skeletonGraphicChallengeFull;

        [Header("Button Claim")]
        [SerializeField] private Button _claimBtn;
        [Header("ClaimButtonObject")]
        [SerializeField] private GameObject buttonClaim;
        [SerializeField] private Image backGroundClaimButton;
        [SerializeField] private Text textClaimButton;

        [Header("Button X2")]
        [SerializeField] private Button _x2Btn;
        [Header("x2ButtonObject")]
        [SerializeField] private GameObject buttonx2;
        [SerializeField] private Image backGroundx2Button;
        [SerializeField] private Text textx2Button;
        [SerializeField] private Image backIconx2Button;

        [Header("icon reward undo")]
        [SerializeField] private GameObject iconRewardUndo;

        [Header("icon reward expand")]
        [SerializeField] private GameObject iconRewardExpand;




        private int countWinGame = 0;
        private bool isRewardUndo;
        private void Awake()
        {
            _nextBtn.onClick.AddListener(OnClickNext);
            _claimBtn.onClick.AddListener(OnClickClaim);
            _x2Btn.onClick.AddListener(OnClickX2);

        }

        public void Show()
        {
            if (LevelManager.Instance.GameMode == GameMode.Undefined)
            {

                panelParent.gameObject.SetActive(true);
                skeletonGraphicLevel.Initialize(true);
                skeletonGraphicLevel.AnimationState.SetAnimation(0, "animation", false);
                skeletonGraphicLevel.AnimationState.Complete += AnimationState_Complete_Level;
            }
            else
            {
                panelDailyParent.gameObject.SetActive(true);

                isRewardUndo = DailyChallenge.Instance.LevelInOrder <= 5;


                tileNomal.SetActive(isRewardUndo);
                tileComplete.SetActive(!isRewardUndo);
                if (isRewardUndo)
                {
                    skeletonGraphicChallengePart.Initialize(true);
                    skeletonGraphicChallengePart.AnimationState.SetAnimation(0, "animation", false);
                    skeletonGraphicChallengePart.AnimationState.Complete += AnimationState_Complete_Challege;
                }
                else
                {
                    skeletonGraphicChallengeFull.Initialize(true);
                    skeletonGraphicChallengeFull.AnimationState.SetAnimation(0, "animation", false);
                    skeletonGraphicChallengeFull.AnimationState.Complete += AnimationState_Complete_Challege;
                }
            }
            gameObject.SetActive(true);
            countWinGame++;
        }

        private void AnimationState_Complete_Challege(Spine.TrackEntry trackEntry)
        {
            iconRewardUndo.SetActive(isRewardUndo);
            iconRewardExpand.SetActive(!isRewardUndo);

            if (isRewardUndo)
                effectDaily.transform.position = iconRewardUndo.transform.position;
            else
                effectDaily.transform.position = iconRewardExpand.transform.position;

            effectDaily.gameObject.SetActive(true);
            effectDaily.DOScale(1.0f, 1.0f).SetEase(Ease.Linear);

            buttonClaim.SetActive(true);
            backGroundClaimButton.DOColor(Color.white, 2.0f).SetEase(Ease.Linear);
            textClaimButton.DOColor(Color.white, 2.0f).SetEase(Ease.Linear);

            buttonx2.SetActive(true);
            backGroundx2Button.DOColor(Color.white, 2.0f).SetEase(Ease.Linear);
            textx2Button.DOColor(Color.white, 2.0f).SetEase(Ease.Linear);
            backIconx2Button.DOColor(Color.white, 2.0f).SetEase(Ease.Linear);
        }

        private void AnimationState_Complete_Level(Spine.TrackEntry trackEntry)
        {
            buttonNext.SetActive(true);
            backGroundNextButton.DOColor(Color.white, 2.0f).SetEase(Ease.Linear);
            textNextButton.DOColor(Color.white, 2.0f).SetEase(Ease.Linear);
        }

       
       


        private void HideDailyPopup()
        {

            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (countWinGame % 2 == 0) D2S.Ads.AdsController.Instance.ShowInterstitial();

           
            iconRewardUndo.SetActive(false);
            iconRewardExpand.SetActive(false);

            tileNomal.SetActive(false);
            tileComplete.SetActive(false);

            effectDaily.gameObject.SetActive(false);
            effectDaily.localScale = Vector3.zero;

            buttonClaim.SetActive(false);
            backGroundClaimButton.color = new Color32(255, 255, 255, 0);
            textClaimButton.color = new Color(255, 255, 255, 0);

            buttonx2.SetActive(false);
            backGroundx2Button.color = new Color32(255, 255, 255, 0);
            textx2Button.color = new Color(255, 255, 255, 0);
            backIconx2Button.color = new Color(255, 255, 255, 0);

            panelDailyParent.gameObject.SetActive(false);

            gameObject.SetActive(false);

            LevelManager.Instance.NextGame();
        }

        private void OnClickNext()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (countWinGame % 2 == 0 && LevelManager.Instance.Level.no > 6) D2S.Ads.AdsController.Instance.ShowInterstitial();



            buttonNext.SetActive(false);
            backGroundNextButton.color = new Color32(255, 255, 255, 0);
            textNextButton.color = new Color(255, 255, 255, 0);

           
            panelParent.gameObject.SetActive(false);

            gameObject.SetActive(false);

            LevelManager.Instance.NextGame();
        }

        private void OnClickClaim()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (isRewardUndo)
                GameStatics.ITEM_UNDO += 1;
            else
                GameStatics.ITEM_EXPAND += 1;

            LevelManager.Instance.HandelInvokeUpdateNumberItem();
            HideDailyPopup();

        }

        private void OnClickX2()
        {
            if (GameManager.IsTestMode)
            {
                if (isRewardUndo)
                    GameStatics.ITEM_UNDO += 2;
                else
                    GameStatics.ITEM_EXPAND += 2;
                HideDailyPopup();
            }
            else if (D2S.Ads.AdsController.Instance.IsLoadedVideoReward())
            {
                D2S.Ads.AdsController.Instance.ShowVideoReward((result) =>
                {
                    if (result)
                    {
                        if (isRewardUndo)
                            GameStatics.ITEM_UNDO += 2;
                        else
                            GameStatics.ITEM_EXPAND += 6;
                        HideDailyPopup();
                    }
                    else
                        Toast.ShowShortText("Rewarded video is finish yet!");

                });
            }
            else
            {
                Toast.ShowShortText("Rewarded video is not ready!");
            }

            DOVirtual.DelayedCall(0.5f, () => { LevelManager.Instance.HandelInvokeUpdateNumberItem(); });

            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }

        private void OnDestroy()
        {
            _nextBtn.onClick.RemoveListener(OnClickNext);
            _claimBtn.onClick.RemoveListener(OnClickClaim);
            _x2Btn.onClick.RemoveListener(OnClickX2);
        }


    }
}