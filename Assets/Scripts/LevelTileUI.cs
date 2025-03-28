using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace WaterSort
{
    public class LevelTileUI : MonoBehaviour, IPointerClickHandler
    {
        public event Action<LevelTileUI> Clicked;

        [SerializeField] private Text _txt;
        [SerializeField] private GameObject _completeMark;
        [SerializeField] private GameObject _lockMark;
        [SerializeField] private Image _fillImg;

        private ViewModel _mViewModel;


        public ViewModel MViewModel
        {
            get => _mViewModel;
            set
            {
                _txt.text = value.levelId.ToString();
                _fillImg.color = _fillImg.color.WithAlpha(value.Locked ? 0 : 1);
                _completeMark.SetActive(value.Completed);
                _lockMark.SetActive(value.Locked);
                _mViewModel = value;
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(this);
        }


        public struct ViewModel
        {
            public int levelId { get; set; }
            //public Level Level { get; set; }
            public bool Locked { get; set; }
            public bool Completed { get; set; }
        }
    }
}