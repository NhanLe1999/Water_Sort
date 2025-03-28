using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelEditor
{
    public class PopupNotification : MonoBehaviour
    {
        [SerializeField] Text content;
        [SerializeField] Button okButton;
        void Start()
        {
            okButton.onClick.AddListener(() => { SetActive(false); });
        }

       public void SetActive(bool isActive, string content = "")
        {
            gameObject.SetActive(isActive);
            this.content.text = content;
        }
    }
}
