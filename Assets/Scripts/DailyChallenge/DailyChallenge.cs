using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace WaterSort
{
    public class DailyChallenge : MonoBehaviour
    {
        [Serializable]
        public struct DailyChallengeItemDay
        {
            public int Year;
            public int Month;
            public int Day;

            public DailyChallengeItemDay(int year, int month, int day)
            {
                Year = year;
                Month = month;
                Day = day;
            }

            //public bool Completed;

            public bool ThisDay(int year, int month, int day)
            {
                if (Year != year) return false;
                if (Month != month) return false;
                if (Day != day) return false;
                return true;
            }

            public bool ThisMonth(int year, int month)
            {
                if (Year != year) return false;
                if (Month != month) return false;
                return true;
            }
        }

        private const string KEY_CURRENT_PROGRESS = "current_progress_dailychallenge";
        private const byte START_INDEX_CHALLENGE = 1;
        private int LoadCurrentProgress()
        {
            string key = KEY_CURRENT_PROGRESS + "_" + currentChallengeDay.Year + "_" + currentChallengeDay.Month + "_" + currentChallengeDay.Day;            
            return PlayerPrefs.GetInt(key, START_INDEX_CHALLENGE);            
        }

        public int LoadCurrentProgress(int Year, int Month, int Day)
        {
            string key = KEY_CURRENT_PROGRESS + "_" + Year + "_" + Month + "_" + Day;
            return PlayerPrefs.GetInt(key, START_INDEX_CHALLENGE);
        }
        private void SaveCurrentProgress()
        {
            string key = KEY_CURRENT_PROGRESS + "_" + currentChallengeDay.Year + "_" + currentChallengeDay.Month + "_" + currentChallengeDay.Day;
            PlayerPrefs.SetInt(key, LevelInOrder);
            PlayerPrefs.Save();
        }

       
        public void StartChallenge(int Year, int Month, int Day)
        {
            currentChallengeDay = new DailyChallengeItemDay(Year, Month, Day);
            LevelInOrder = LoadCurrentProgress();           
        }
        public void CompletePartOfChallenge()
        {
            LevelInOrder++;
            SaveCurrentProgress();
        }
        public void CompleteChallenge()
        {           
            CompleteChallenge(currentChallengeDay.Year, currentChallengeDay.Month, currentChallengeDay.Day);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            ShowPanel(isActive);
        }

        [Serializable]
        public class SaveData
        {
            public DailyChallengeItemDay[] items;
        }

        public class DataMonthComplete
        {
            public bool isComplete;
            public int dayTotal;
            public int dayCompleted;
            public bool[] dataDays;

            public float FillRate
            {
                get
                {
                    return dayCompleted / (float)dayTotal;
                }
            }
        }
        public static DailyChallenge Instance { get; private set; }

        private string filePath;
        private List<DailyChallengeItemDay> listItems;

        public DateTime Now;
        public readonly DateTime StartDate = new DateTime(2022, 02, 1);

        private DailyChallengeItemDay currentChallengeDay;
        public int LevelInOrder { get; private set; }
        public int TotalMonths
        {
            get
            {
                if (StartDate > Now) return 0;
                return (Now.Year - StartDate.Year) * 12 + (Now.Month - StartDate.Month + 1);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);

                InitData();
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }
        private void InitData()
        {
            Now = DateTime.Now;
            filePath = Application.persistentDataPath + "/DailyChallenge.dat";
            listItems = new List<DailyChallengeItemDay>();
            LoadGame();
        }

        public void LoadGame()
        {

            if (File.Exists(filePath))
            {
                //File.Delete(filePath);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(filePath, FileMode.Open);
                SaveData data = (SaveData)bf.Deserialize(file);
                listItems.AddRange(data.items);
                file.Close();
                Debug.Log("Game data loaded!");
            }
            else
                Debug.Log("There is no save data!");

        }
        public void SaveGame()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(filePath);
            SaveData data = new SaveData();
            data.items = listItems.ToArray();
            bf.Serialize(file, data);
            file.Close();
            Debug.Log("Game data saved!");
        }
        public void CompleteChallenge(int year, int month, int day)
        {
            bool isFound = false;
            foreach (var item in listItems)
            {
                if (item.ThisDay(year, month, day))
                {
                    isFound = true;
                    //item.Complete();
                    break;
                }
            }
            if (!isFound)
            {
                listItems.Add(new DailyChallengeItemDay(year, month, day));
                SaveGame();
            }
        }
        public DataMonthComplete DataCompleteMonthlyTarget(int year, int month)
        {
            DataMonthComplete data = new DataMonthComplete();
            int totalDay = DateTime.DaysInMonth(year, month);

            data.dataDays = new bool[totalDay];
            int countComplete = 0;
            foreach (var item in listItems)
            {
                if (item.ThisMonth(year, month))
                {
                    countComplete += 1;
                    data.dataDays[item.Day - 1] = true;
                }
            }
            data.isComplete = countComplete == totalDay;
            //data.fillRate = countComplete / (float)totalDay;
            data.dayTotal = totalDay;
            data.dayCompleted = countComplete;
            return data;
        }

        [SerializeField] GameObject panelChallenge;
        [SerializeField] GameObject panelCollections;

        public void ShowPanel(bool isShowChallenge)
        {
            panelChallenge.SetActive(isShowChallenge);
            panelCollections.SetActive(!isShowChallenge);
        }
    }
}