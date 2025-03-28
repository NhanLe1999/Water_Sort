using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class ButtonPickColorEditor : MonoBehaviour
    {
        private int id;
        private Button button;
        private Color color;
        public enum Type
        {
            pickColor = 0,
            clearColor = 1
        }
        [SerializeField] Type type;
        public delegate void DelegateClick(Type type,int id, Color color);
        public event DelegateClick OnClick;

        void Awake()
        {
            button = gameObject.GetComponent<Button>();
            color = gameObject.GetComponent<Image>().color;
        }

        public void SetAction(int id)
        {
            this.id = id;
            button.onClick.AddListener(() =>
            {
                OnClick.Invoke(this.type, this.id, color);
            });
        }
    }
}
