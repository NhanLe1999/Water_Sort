using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class RatePopup : MonoBehaviour
    {
        [Header("Frame")]
        [SerializeField] RectTransform frame;

        [Header("Close")]
        [SerializeField] private Button _closeBtn;
        [Header("Rate")]
        [SerializeField] private Button _rateBtn;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(OnClickCloseButton);

            _rateBtn.onClick.AddListener(OnClickRateButton);
        }


        public void Show()
        {
            if (GameConfig.Rated) return;
            DOTween.Kill(frame);
            gameObject.SetActive(true);
            frame.DOScale(1.0f, 1f).SetEase(Ease.OutElastic);
            StartCoroutine(IARManager.Instance.RequestReview());
        }

        private void OnClickCloseButton()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            Hide();
        }

        private void OnClickRateButton()
        {
            GameConfig.Rated = true;
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            StartCoroutine(IARManager.Instance.LauchReview());
            Hide();
        }

        private void Hide()
        {
            DOTween.Kill(frame);
            gameObject.SetActive(false);
            frame.localScale = Vector3.one * 0.9f;
        }

        private void OnDestroy()
        {
            _closeBtn.onClick.RemoveListener(OnClickCloseButton);

            _rateBtn.onClick.RemoveListener(OnClickRateButton);
        }
    }
}
