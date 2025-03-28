using WaterSort;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
namespace WaterSort
{
    public class PopupDailyChallenge : MonoBehaviour
    {
        [SerializeField] private Text txtYear;
        [SerializeField] private Text txtMonth;
        [SerializeField] private Text txtProcess;
        [SerializeField] GameObject completeChallenge;
        [SerializeField] GameObject nomalChallenge;
        [SerializeField] GameObject newChallenge;
        [SerializeField] private Image trophyCup;


        [SerializeField] private CallenderMonth callenderMonth;

        [SerializeField] private Button btnPrevMonth;
        [SerializeField] private Button btnNextMonth;
        [SerializeField] private Button btnChallenge;
        [SerializeField] private Button btnBack;
        [SerializeField] private Button btnCollections;

        private DateTime curTime;
        private DateTime startTime;
        private int Year;
        private int Month;

        private int Day = 0;

        public static event Action<int> OnSelectedDay;
        private bool isInitMap;

        private void Awake()
        {
            btnChallenge.onClick.AddListener(OnBtnChallenge_Clicked);
            btnPrevMonth.onClick.AddListener(PrevMonth_Click);
            btnNextMonth.onClick.AddListener(NextMonth_Click);
            btnBack.onClick.AddListener(Back_Click);
            btnCollections.onClick.AddListener(Collections_Click);
        }

        private void OnDestroy()
        {
            btnChallenge.onClick.RemoveListener(OnBtnChallenge_Clicked);
            btnPrevMonth.onClick.RemoveListener(PrevMonth_Click);
            btnNextMonth.onClick.RemoveListener(NextMonth_Click);
            btnBack.onClick.RemoveListener(Back_Click);
            btnCollections.onClick.RemoveListener(Collections_Click);
        }

        private void OnEnable()
        {
            if (isInitMap)
            {
                var dataMonth = DailyChallenge.Instance.DataCompleteMonthlyTarget(Year, Month);
                callenderMonth.UpdateState(dataMonth.dataDays);
                ProccessUpdate();
                btnChallenge.gameObject.SetActive(DailyChallenge.Instance.LoadCurrentProgress(Year, Month, Day) <= 5);
            }
            CallenderDay.OnClickItemDay += CallenderDay_OnClickItemDay;
        }
        private void OnDisable()
        {
            CallenderDay.OnClickItemDay -= CallenderDay_OnClickItemDay;

        }

        private void CallenderDay_OnClickItemDay(int idDay)
        {
            if (this.Day == idDay) return;
            this.Day = idDay;
            ProccessUpdate();
            OnSelectedDay?.Invoke(Day);
        }

        void Start()
        {
            curTime = DailyChallenge.Instance.Now;
            startTime = DailyChallenge.Instance.StartDate;
            Year = curTime.Year;
            Month = curTime.Month;
            Day = curTime.Day;
            UpdateUI();
        }

        private void UpdateUI()
        {
            this.txtYear.text = Year.ToString();
            this.txtMonth.text = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(Month);

            int index = (Year - startTime.Year) * 12 + (Month - startTime.Month);
            this.trophyCup.sprite = ResourceManager.LoadTropy(index % GameConfig.TOTAL_TROPHY);

            /*
            if (curTime.Year == Year && curTime.Month == Month)
            {
                Day = curTime.Day;
            }
            else Day = 1;
            */
            Day = curTime.Day;

            var dataMonth = DailyChallenge.Instance.DataCompleteMonthlyTarget(Year, Month);



            if (curTime.Year > Year || (curTime.Year == Year && curTime.Month > Month))
            {
                btnNextMonth.gameObject.SetActive(true);
            }
            else btnNextMonth.gameObject.SetActive(false);

            if (Year > DailyChallenge.Instance.StartDate.Year || (DailyChallenge.Instance.StartDate.Year == Year && Month > DailyChallenge.Instance.StartDate.Month))
            {
                btnPrevMonth.gameObject.SetActive(true);
            }
            else btnPrevMonth.gameObject.SetActive(false);




            callenderMonth.Init(Year, Month, Day, dataMonth.dataDays, curTime);

            btnChallenge.gameObject.SetActive(DailyChallenge.Instance.LoadCurrentProgress(Year, Month, Day) <= 5);

            ProccessUpdate();
            isInitMap = true;
        }

#if UNITY_EDITOR
        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextMonth_Click();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PrevMonth_Click();
            }
        }
#endif

        private void PrevMonth_Click()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            Month--;
            if (Month < 1)
            {
                Month = 12;
                Year--;
            }
            UpdateUI();
        }
        private void NextMonth_Click()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            Month++;
            if (Month > 12)
            {
                Month = 1;
                Year++;
            }
            UpdateUI();
        }

        private void Back_Click()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            UIManager.Instance.SetActiveDaily(false);
        }

        private void OnBtnChallenge_Clicked()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            DailyChallenge.Instance.StartChallenge(Year, Month, Day);
            GameStatics.BACK_LEVEL_MODE = DailyChallenge.Instance.LoadCurrentProgress(Year, Month, Day);
            GameManager.Instance.LoadDataDaily();
            UIManager.Instance.SetActiveDaily(false);

        }

        private void Collections_Click()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            UIManager.Instance.ShowPanelDaily(false);
        }

        private void ProccessUpdate()
        {
            int process = DailyChallenge.Instance.LoadCurrentProgress(Year, Month, Day);
            if (process == 1)
            {
                newChallenge.SetActive(true);
                nomalChallenge.SetActive(false);
                completeChallenge.SetActive(false);
            }
            else if (process <= 5)
            {
                txtProcess.text = (process) + "/5";
                newChallenge.SetActive(false);
                nomalChallenge.SetActive(true);
                completeChallenge.SetActive(false);
            }
            else
            {
                newChallenge.SetActive(false);
                nomalChallenge.SetActive(false);
                completeChallenge.SetActive(true);
            }
        }
    }
}