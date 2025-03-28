using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class PopupInfor : MonoBehaviour
    {
        [SerializeField] Text txtTitle;
        [SerializeField] InputField inputField;
        [SerializeField] Button buttonOK;
        [SerializeField] Button buttonCancel;

        public enum Type
        {
            loadLevel = 0,
            saveLevel = 1
        }
        private Type type;
        private int levelSelected;
        public delegate void DelegateClick(Type type, int idLevel);
        public event DelegateClick OnClick;
        void Start()
        {
            buttonOK.onClick.AddListener(ButtonOKListener);
            buttonCancel.onClick.AddListener(ButtonCancelListener);
            inputField.onValueChanged.AddListener(delegate { InputFieldValueChanged(); });
            levelSelected = -1;
        }

        public void SetActive(bool isActive, Type type = Type.loadLevel, string title = "")
        {
            gameObject.SetActive(isActive);
            this.type = type;
            txtTitle.text = title;
        }

        private void ButtonOKListener()
        {
            OnClick?.Invoke(type, levelSelected);
        }

        private void ButtonCancelListener()
        {
            SetActive(false);
        }

        private void InputFieldValueChanged()
        {
            levelSelected = int.Parse(inputField.text);
        }
    }
}
