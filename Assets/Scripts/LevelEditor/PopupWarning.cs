using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class PopupWarning : MonoBehaviour
    {
        [SerializeField] Text content;
        [SerializeField] Button okButton;
        [SerializeField] Button cancelButton;
        public enum TypePopup
        {
            warningLoadLevel = 0,
            warningSaveLevel = 1
        }
        private TypePopup typePopup;

        public enum ButtonType
        {
            okButton = 0,
            cancelButton = 1
        }
        public delegate void DelegateClick(TypePopup typePopup, ButtonType buttonType);
        public event DelegateClick OnClick;
        void Start()
        {
            okButton.onClick.AddListener(ButtonOKListener);
            cancelButton.onClick.AddListener(ButtonCancelListener);
        }

        public void SetActive(bool isActive, TypePopup typePopup = TypePopup.warningLoadLevel, string content = "")
        {
            gameObject.SetActive(isActive);
            this.typePopup = typePopup;
            this.content.text = content;
        }
        private void ButtonOKListener()
        {
            OnClick?.Invoke(typePopup, ButtonType.okButton);
        }

        private void ButtonCancelListener()
        {
            OnClick?.Invoke(typePopup, ButtonType.cancelButton);
        }
    }
}
