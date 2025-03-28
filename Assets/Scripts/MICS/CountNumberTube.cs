using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterSort
{
    public class CountNumberTube : MonoBehaviour
    {

        public List<int> listCountTube = new List<int>();
        private void Start()
        {
            for(int count = 1; count <= ResourceManager.Instance.TotalLevel; count++)
            {
                string path = "Levels/lv" + count;
                var textAssets = Resources.Load<TextAsset>(path);
                Level data = JsonUtility.FromJson<Level>(textAssets.text);
                int countTube = data.map.Count;
                if(!listCountTube.Contains(countTube))
                {
                    listCountTube.Add(countTube);
                }
            }
            listCountTube.Sort();

            foreach (int temp in listCountTube)
            {
                Debug.Log(temp);
            }
        }
    }
}
