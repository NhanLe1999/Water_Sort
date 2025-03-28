using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class ClaimPanel : MonoBehaviour
    {
        
        

        [Header("Button Close")]
        [SerializeField] private Button _closeBtn;
       

        [Header("Object")]
        [SerializeField] GameObject botleItem;
        [SerializeField] GameObject backGroundItem;
        [SerializeField] GameObject panelParent;

        [Header("Image Item")]
        [SerializeField] Image[] arrayImageItem;
        [SerializeField] Image[] arrayImageMask;

        [Header("Image Back Ground")]
        [SerializeField] Image imageBackGround;

        [Header("FireWork")]
        [SerializeField] RectTransform rectTranformFireWork;
        

        private void Awake()
        {
           
           
        }
        private void OnEnable()
        {
            _closeBtn.onClick.AddListener(OnClickCloseButton);
        }
        public void Show(ItemShop.Type type, int id)
        {
            rectTranformFireWork.anchoredPosition = new Vector2(0, -650);
            SetData(type, id);
            panelParent.SetActive(true);
            gameObject.SetActive(true);

        }

       
        void SetData(ItemShop.Type type, int id)
        {
            botleItem.SetActive(type == ItemShop.Type.Bottle ? true : false);
            backGroundItem.SetActive(type == ItemShop.Type.BackGround ? true : false);
            if (type == ItemShop.Type.Bottle)
            {
                
                Sprite sprite = ResourceManager.LoadBottle(id);
                for (int i = 0; i < arrayImageItem.Length; i++)
                    arrayImageItem[i].sprite = sprite;
                
                Sprite maskSprite = ResourceManager.LoadMask(id);
                if (arrayImageMask != null)
                    Debug.Log(arrayImageMask.Length + "______");
                else
                    Debug.Log("Mask Null");
                /*
                for (int i = 0; i < arrayImageMaskItem.Length; i++)
                {
                    arrayImageMaskItem[i].sprite = maskSprite;
                   
                }
                */
            }
            else
            {
                Sprite sprite = ResourceManager.LoadBackground(id);
                imageBackGround.sprite = sprite;
            }
        }
        private void OnClickCloseButton()
        {
            SoundController.Instance.PlaySound(AUDIO_KEY.SOUND_UI_CLICK);
            panelParent.SetActive(false);
           
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _closeBtn.onClick.RemoveListener(OnClickCloseButton);
        }
    }
}
