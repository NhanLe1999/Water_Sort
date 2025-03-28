using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class EffectFullBottleManager : Singleton<EffectFullBottleManager>
    {
        [SerializeField] GameObject[] arrayEffect;
        private int numberEffect;
        private void Start()
        {
            numberEffect = arrayEffect.Length;
        }

        public void CreateEffectFull(Vector3 positon)
        {
            GameObject effect = Instantiate(arrayEffect[Random.Range(0, numberEffect)], positon, transform.rotation);
        }
    }
}
