using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class CallenderMonth : MonoBehaviour
    {
        [SerializeField] private CallenderDay _dayPrefab;
        [SerializeField] private GameObject _dayEmtyPrefab;

        [SerializeField] private Transform tempModelPosDays;
        public Transform[] modelPos;

        private DateTime thisMonth;
        private DateTime curDay;

        private int curSelectedDayId;

        private int numEmptyDay;
        private int numDay;


        private List<GameObject> emptyItems = new List<GameObject>();
        private List<CallenderDay> dayItems = new List<CallenderDay>();

        public void Init(int year, int month, int day, bool[] dataDays, DateTime curDay)
        {
            this.thisMonth = new DateTime(year, month, 1);
            this.curSelectedDayId = day;
            this.curDay = curDay;
            this.numDay = dataDays.Length /*DateTime.DaysInMonth(year, month)*/;
            this.numEmptyDay = (int)thisMonth.DayOfWeek;
            CreateMap(dataDays);
        }


        private void Awake()
        {
            modelPos = tempModelPosDays.GetComponentsInChildren<Transform>();
        }
        private void Start()
        {
            for (int i = 0; i < numEmptyDay; i++)
            {
                emptyItems[i].transform.localPosition = modelPos[i + 1].localPosition;
            }
            for (int i = 0; i < numDay; i++)
            {
                dayItems[i].transform.localPosition = modelPos[i + numEmptyDay + 1].localPosition;
            }
        }
        private void CreateMap(bool[] dataDays)
        {
            #region empty days
            for (int i = 0; i < numEmptyDay; i++)
            {
                if (emptyItems.Count > i)
                {
                    emptyItems[i].SetActive(true);
                }
                else
                {
                    var obj = Instantiate(_dayEmtyPrefab, this.transform);
                    emptyItems.Add(obj);
                }
                emptyItems[i].transform.SetSiblingIndex(i);
                if (modelPos.Length != 0) emptyItems[i].transform.localPosition = modelPos[i + 1].localPosition;

            }
            for (int i = numEmptyDay; i < emptyItems.Count; i++)
            {
                emptyItems[i].SetActive(false);
            }
            #endregion

          
           
            for (int i = 0; i < numDay; i++)
            {
                if (dayItems.Count <= i)
                {
                    var obj = Instantiate(_dayPrefab, this.transform);
                    dayItems.Add(obj.GetComponent<CallenderDay>());
                }
                dayItems[i].gameObject.SetActive(true);
                dayItems[i].transform.SetSiblingIndex(i + numEmptyDay);
                if (modelPos.Length != 0) dayItems[i].transform.localPosition = modelPos[i + numEmptyDay + 1].localPosition;
                DateTime tempDateTime = new DateTime(thisMonth.Year, thisMonth.Month, i + 1);
                bool isCurrentDay = (tempDateTime.Year == curDay.Year && tempDateTime.Month == curDay.Month && tempDateTime.Day == curDay.Day);
                
                dayItems[i].Init(idDay: (i + 1), 
                    isSunday: ((i + numEmptyDay) % 7 == 0), 
                    isCompleted: (dataDays[i]), 
                    isSelected: isCurrentDay, 
                    isOver: new DateTime(thisMonth.Year, thisMonth.Month, i + 1) <= curDay);
            }
            for (int i = numDay; i < dayItems.Count; i++)
            {
                dayItems[i].gameObject.SetActive(false);
            }
            
        }

        public void UpdateState(bool[] dataDays)
        {
            for (int i = 0; i < numDay; i++)
            {
                DateTime tempDateTime = new DateTime(thisMonth.Year, thisMonth.Month, i + 1);
                bool isCurrentDay = (tempDateTime.Year == curDay.Year && tempDateTime.Month == curDay.Month && tempDateTime.Day == curDay.Day);
                dayItems[i].Init(idDay: (i + 1), 
                    isSunday: ((i + numEmptyDay) % 7 == 0), 
                    isCompleted: (dataDays[i]), 
                    isSelected: isCurrentDay, //((i + 1) == curSelectedDayId), 
                    isOver: new DateTime(thisMonth.Year, thisMonth.Month, i + 1) <= curDay);
            }
            
        }
    }
}