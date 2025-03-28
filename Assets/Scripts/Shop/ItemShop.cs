using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WaterSort
{
    public class ItemShop : MonoBehaviour, IPointerClickHandler
    {
        public enum Type
        {
            Bottle = 0,
            BackGround = 1
        }

        private Type type
        {
            set
            {
                botleItem.SetActive(value == Type.Bottle ? true : false);
                backGroundItem.SetActive(value == Type.BackGround ? true : false);
            }
        }

        private bool isLock;
        public bool IsLock
        {
            set
            {
                isLock = value;
                _lock.SetActive(isLock);
                imageBackGroundUItem.sprite = isLock ? arrayImageBackGroundUItem[0] : arrayImageBackGroundUItem[1];
            }
            get { return isLock; }
        }
        public int ID { set; get; }

        private bool isSelected;
        public bool IsSelected
        {
            set
            {
                isSelected = value;
                select.SetActive(isSelected);
            }
            get { return isSelected; }
        }

        [Header("Object")]
        [SerializeField] GameObject botleItem;
        [SerializeField] GameObject backGroundItem;
        [SerializeField] GameObject select;
        [SerializeField] GameObject _lock;

        [Header("Image Item")]
        [SerializeField] Image[] arrayImageItem;

        [Header("Image Back Ground")]
        [SerializeField] Image imageBackGround;

        [Header("Back Ground Item")]
        [SerializeField] Image imageBackGroundUItem;
        [SerializeField] Sprite[] arrayImageBackGroundUItem;

        public static event Action<int> OnSelectItem;
        private void OnEnable()
        {
            OnSelectItem += SkinItem_OnSelectItem;
        }

        private void SkinItem_OnSelectItem(int id)
        {           
            if (this.ID == id)
                IsSelected = true;
            else
                IsSelected = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsLock)
                OnSelectItem?.Invoke(ID); 
        }

       
        public void SetData(int id, Type type, bool isLock)
        {
            this.ID = id;
            this.type = type;
            this.IsLock = isLock;

            if (type == Type.Bottle)
            {
                Sprite sprite = ResourceManager.LoadBottle(id);
                for (int i = 0; i < arrayImageItem.Length; i++)
                    arrayImageItem[i].sprite = sprite;
            }
            else
            {
                Sprite sprite = ResourceManager.LoadBackground(id);
                imageBackGround.sprite = sprite;
            }
            IsSelected = false;
            gameObject.SetActive(true);
        }

        public void SetDisable()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            OnSelectItem -= SkinItem_OnSelectItem;
        }
    }
}