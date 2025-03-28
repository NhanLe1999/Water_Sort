using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class Surface : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _surface;
        [SerializeField] private SpriteRenderer _anphaSurface;

        public void SetSprite(Sprite sprite)
        {
            _surface.sprite = sprite;
            _anphaSurface.sprite = sprite;
        }

        public void SetColor(int idColor)
        {
            if (GameConfig.BLIND_MODE)
            {
                _surface.color = ResourceManager.Instance._blindColors[idColor];
            }
            else
            {
                _surface.color = ResourceManager.Instance._normalColors[idColor];
            }
        }

        public void SetActive(bool isActive)
        {
            _surface.gameObject.SetActive(isActive);
        }

        public void sortingOrder(int idLayerSurface, int idAnphaLayerSurface)
        {
            _surface.sortingOrder = idLayerSurface;
            _anphaSurface.sortingOrder = idAnphaLayerSurface;
            
        }
        public void sortingLayerName(string nameLayer)
        {
            _surface.sortingLayerName = nameLayer;
            _anphaSurface.sortingLayerName = nameLayer;           
        }

        public void SetPosition(Vector2 position)
        {
            _surface.transform.position = position;
        }
    }
}
