using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WaterSort
{
    public class TestSelectLevel : MonoBehaviour
    {
        [SerializeField] InputField inputField;
        [SerializeField] Button okButton;
        void Start()
        {
            okButton.onClick.AddListener(OnClickOK);
        }

        void OnClickOK()
        {
            
            if(inputField.text == "")
            {
                Toast.ShowShortText("Chọn level từ 1 đến " + ResourceManager.Instance.TotalLevel);
                return;
            }

            int level = int.Parse(inputField.text);
            if (level < 1 || level > ResourceManager.Instance.TotalLevel)
            {             
                Toast.ShowShortText("Chọn level từ 1 đến " + ResourceManager.Instance.TotalLevel);
                return;
            }
            else
            {
                LevelManager.Instance.SelectLevel(level);
            }
            
           // LevelManager.Instance.OverTheGame();
    }

        private void OnDestroy()
        {
            okButton.onClick.RemoveListener(OnClickOK);
        }
    }
}