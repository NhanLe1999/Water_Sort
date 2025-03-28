using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WaterSort
{
    public class CallenderDay : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Text txtDay;
        [SerializeField] private GameObject objSelected;
        [SerializeField] private GameObject objCompleted;
        [SerializeField] private GameObject objUnCompleted;
        [SerializeField] private GameObject objSelected2;

        private int idDay;
        private bool isCompleted;
        private bool isSelected;
       
        public static event Action<int> OnClickItemDay;

        private Color32 sundayColor = new Color32(226, 47, 0, 255);
        private Color32 sundayColorOff = new Color32(150, 104, 93, 255);

        public void Init(int idDay, bool isSunday, bool isCompleted, bool isSelected, bool isOver)
        {
            this.idDay = idDay;
            this.isCompleted = isCompleted;
            this.isSelected = isSelected;
           

            this.txtDay.text = idDay.ToString();


            if (isSunday)
            {
                this.txtDay.color = isOver ? sundayColor : sundayColorOff;
            }
            else this.txtDay.color = isOver ? Color.black : Color.gray;


            objCompleted.SetActive(isCompleted);
            //objUnCompleted.SetActive(!isCompleted && isOver && !isSelected);
            UpdateUI();
        }
        void OnEnable()
        {
            PopupDailyChallenge.OnSelectedDay += PopupChallenge_OnSelectedDay;
        }
        void OnDisable()
        {
            PopupDailyChallenge.OnSelectedDay -= PopupChallenge_OnSelectedDay;
        }
        private void PopupChallenge_OnSelectedDay(int selectedId)
        {
            this.isSelected = (idDay == selectedId);
            UpdateUI();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            /*
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            if (canSelect)
                OnClickItemDay?.Invoke(idDay);
            */
        }

        private void UpdateUI()
        {
            objSelected2.SetActive(isCompleted && isSelected);
            objSelected.SetActive(!isCompleted && isSelected);
        }
    }
}