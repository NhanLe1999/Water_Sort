using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaterSort
{
    public class LevelsValidator : MonoBehaviour
    {
        private int defaultMaxWater = 4;

        int maxWater;

        private Dictionary<int, int> valueColors = new Dictionary<int, int>();
        private IEnumerator Start()
        {
            string Error = string.Empty;
            Debug.Log("normal level");
            for (int i = 1; i <= ResourceManager.Instance.TotalLevel; i++)
            {
                string path = "levels/lv" + i;
                Level level = ResourceManager.GetLevel(path);
                if (CheckErrorLevel(level))
                {
                    Error += i + "; ";
                }
                yield return new WaitForEndOfFrame();
            }
            for (int i = 0; i < ResourceManager.Instance.TotalChallenge.Length; i++)
            {
                Debug.Log("challenge level " + (i + 1));
                int totalLevel = ResourceManager.Instance.TotalChallenge[i];
                for (int j = 1; j <= totalLevel; j++)
                {
                    string path = "level" + (i + 1) + "/lv" + (j);
                    Level level = ResourceManager.GetLevel(path);
                    if (CheckErrorLevel(level))
                    {
                        Error += string.Format("[{0},{1}]", i + 1, j);
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
            Debug.LogError("Levels error: " + Error);
        }
        private bool CheckErrorLevel(Level level)
        {
            bool isError = false;
            maxWater = level.maxWaterInTube;
            if (maxWater == 0) maxWater = defaultMaxWater;
            valueColors.Clear();

            var listTube = level.map;
            foreach (var tube in listTube)
            {
                var values = tube.values;

                foreach (int value in values)
                {
                    if (valueColors.ContainsKey(value))
                        valueColors[value] += 1;
                    else valueColors.Add(value, 1);
                }
            }

            var keycolors = valueColors.Values.ToList();
            foreach (var key in keycolors)
            {
                if (key % maxWater != 0)
                {
                    isError = true;
                    break;
                }
            }
            return isError;
        }
    }
}
