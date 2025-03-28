using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class ParentTempPour : Singleton<ParentTempPour>
    {
        private List<Transform> listObject;
        private int amountObjectInPool = 5;
        private void Start()
        {
            listObject = new List<Transform>();
            for(int count = 0; count < amountObjectInPool; count++)
            {
                GameObject parentTemp = new GameObject("parentPourTemp");
                parentTemp.SetActive(false);
                Transform parentTempTrans = parentTemp.transform;
                listObject.Add(parentTempTrans);
            }
        }

        public Transform GetTransform(Vector3 positon, Quaternion quaternion)
        {
            
            for(int count = 0; count < listObject.Count; count++)
            {
                if(!listObject[count].gameObject.activeInHierarchy)
                {
                    listObject[count].gameObject.SetActive(true);
                    Transform _transform = listObject[count];
                    _transform.position = positon;
                    _transform.rotation = quaternion;
                    return _transform;
                }
            }

            GameObject parentTemp = new GameObject("parentPourTemp");
            Transform parentTempTrans = parentTemp.transform;
            listObject.Add(parentTempTrans);
            parentTempTrans.position = positon;
            parentTempTrans.rotation = quaternion;
            return parentTempTrans;
        }
    }
}
