using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class PopupDailyCollection : MonoBehaviour
    {
        public class TrophyData
        {
            public int Year;
            public int Month;
            public int dayCompleted;
            public int dayTotal;
            public bool IsCompleted;

            public TrophyData(int year, int month, DailyChallenge.DataMonthComplete dataMonthComplete)
            {
                Year = year;
                Month = month;
                dayCompleted = dataMonthComplete.dayCompleted;
                dayTotal = dataMonthComplete.dayTotal;
                IsCompleted = dataMonthComplete.isComplete;
            }

            public void UpdateData(DailyChallenge.DataMonthComplete dataMonthComplete)
            {
                dayCompleted = dataMonthComplete.dayCompleted;
                dayTotal = dataMonthComplete.dayTotal;
                IsCompleted = dataMonthComplete.isComplete;
            }
        }

        [SerializeField] private GameObject trophyPrefab;
        [SerializeField] private Transform trophyContent;
        [SerializeField] Button backButton;

        private List<GameObject> trophyObjs = new List<GameObject>();
        private List<TrophyData> trophyDatas = new List<TrophyData>();
        private bool isInitData = false;
        private int totalMonth;
        private void Start()
        {
            backButton.onClick.AddListener(Back_Click);
            InitData();
        }
        private void OnEnable()
        {
            if (isInitData)
            {
                for (int i = 0; i < totalMonth; i++)
                {
                    trophyDatas[i].UpdateData(DailyChallenge.Instance.DataCompleteMonthlyTarget(trophyDatas[i].Year, trophyDatas[i].Month));
                    SetData(trophyObjs[i], trophyDatas[i]);
                }
            }
        }

        private void InitData()
        {
            totalMonth = DailyChallenge.Instance.TotalMonths;
            DateTime StartDate = DailyChallenge.Instance.StartDate;

            int Year = StartDate.Year;
            int Month = StartDate.Month;

            Debug.Log("Total months: " + totalMonth);
            for (int i = 0; i < totalMonth; i++)
            {
                TrophyData data = new TrophyData(Year, Month, DailyChallenge.Instance.DataCompleteMonthlyTarget(Year, Month));
                Month++;
                if (Month > 12)
                {
                    Month = 1;
                    Year++;
                }
                trophyDatas.Add(data);

                var obj = Instantiate(trophyPrefab, trophyContent);
                obj.SetActive(true);
                SetData(i, obj, data);
                trophyObjs.Add(obj);
            }
            isInitData = true;
        }
        private void SetData(int index, GameObject trophy, TrophyData data)
        {
            trophy.transform.Find("iconCup").GetComponent<Image>().sprite = ResourceManager.LoadTropy(index % GameConfig.TOTAL_TROPHY);
            SetData(trophy, data);
        }
        private void SetData(GameObject trophy, TrophyData data)
        {
            trophy.transform.Find("txtProgress").GetComponent<Text>().text = string.Format("{0}/{1}", data.dayCompleted, data.dayTotal);
            trophy.transform.Find("txtMonth").GetComponent<Text>().text = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(data.Month);
        }

        private void Back_Click()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            UIManager.Instance.ShowPanelDaily(true);
        }
        void OnDestroy()
        {
            backButton.onClick.RemoveListener(Back_Click);
        }
    }
}