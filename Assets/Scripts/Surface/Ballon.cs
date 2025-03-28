using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class Ballon : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particleSystem;
        [SerializeField] ParticleSystemRenderer _particleSystemRenderer;
       

        
        
        public void SetActive(bool isActive)
        {
            _particleSystem.gameObject.SetActive(isActive);
        }

        public void SetTime(float value)
        {
            var main = _particleSystem.main;
            main.startLifetime = 8 * value;
           
        }
        public void sortingOrder(int idLayer)
        {
            _particleSystemRenderer.sortingOrder = idLayer;


        }
        public void sortingLayerName(string nameLayer)
        {
            _particleSystemRenderer.sortingLayerName = nameLayer;
        }
    }
}
