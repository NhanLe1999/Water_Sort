using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class UIButtonGift : MonoBehaviour
    {
        [SerializeField] private Text txtCountTime;
        [SerializeField] private GameObject objFree;
        private const float intervalGiftTime = 600;
        public bool canGetGift = false;

        private static float _giftTime = -1;
        private float _currentTime;
        private int _currentRemainTime;

        void Start()
        {
            if (_giftTime < 0) _giftTime = Time.realtimeSinceStartup + intervalGiftTime;
            _currentTime = Time.realtimeSinceStartup;

            if (_currentTime < _giftTime) // chưa tới
            {
                canGetGift = false;
                _currentRemainTime = Mathf.RoundToInt(_giftTime - _currentTime);
                InvokeRepeating("UpdateTime", 0, 1);
            }
            else
            {
                canGetGift = true;
            }
            UpdateState();

            GetComponent<Button>().onClick.AddListener(OnClick_BtnGift);
        }

        private void UpdateState()
        {
            if (canGetGift)
            {
                objFree.SetActive(true);
                txtCountTime.gameObject.SetActive(false);
            }
            else
            {
                objFree.SetActive(false);
                txtCountTime.gameObject.SetActive(true);
            }
        }

        private void UpdateTime()
        {
            _currentRemainTime -= 1;

            if (_currentRemainTime <= 0)
            {
                canGetGift = true;
                UpdateState();
                CancelInvoke("UpdateTime");
            }
            else txtCountTime.text = string.Format("{0:00}:{1:00}", _currentRemainTime / 60, _currentRemainTime % 60);
        }


        private void OnClick_BtnGift()
        {
            if (!canGetGift)
            {
                Toast.ShowShortText("Rewarded is not ready!");
                return;
            }
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
        }
        public void GetGift()
        {
            canGetGift = false;
            _giftTime = Time.realtimeSinceStartup + intervalGiftTime;
            _currentRemainTime = (int)intervalGiftTime;
            UpdateState();
            InvokeRepeating("UpdateTime", 0, 1);
        }

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveListener(OnClick_BtnGift);
        }
    }
}
