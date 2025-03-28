using LevelEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace WaterSort
{
    public class ConvertDataFromTxt : MonoBehaviour
    {
        public TextAsset sourceTexts;
        private int level;
        //private int numberBottle;
        private int numberWaterInBottle;
        [SerializeField] HolderEditor holderEditorPrefab;
        private List<HolderEditor> listHolderEditor = new List<HolderEditor>();
        private IEnumerator Start()
        {
            string[] lines = sourceTexts.text.Split('\n');
            int totalLevel = lines.Length;
            for (int i = 0; i < totalLevel; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var data = lines[i].Split(';');
                Debug.Log(data[0]);
                level = int.Parse(data[0].Trim());
                //Debug.Log(data[1]);
                //numberBottle = int.Parse(data[1].Trim());
                numberWaterInBottle = int.Parse(data[3].Trim());
                GenHolders(data[2].Trim().Split(','));
                SaveLevel(level);
                yield return new WaitForEndOfFrame();
            }
            Debug.LogError("Done!!!!");
            yield return null;
        }
        private void GenHolders(string[] data)
        {
            listHolderEditor.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                var holder = Instantiate(holderEditorPrefab);
                holder.SetMaxWater(numberWaterInBottle);
                if (!data[i].Equals("#"))
                {
                    var water = data[i].Split('/');
                    for (int j = 0; j < water.Length; j++)
                    {
                        int idColor = int.Parse(water[j].Trim());
                        holder.PickLiquid(idColor, 1);
                    }
                }
                listHolderEditor.Add(holder);
            }
        }
        private void SaveLevel(int levelID)
        {
            List<LevelColumn> map = new List<LevelColumn>();
            for (int count = 0; count < listHolderEditor.Count; count++)
            {
                List<int> values = new List<int>();
                List<LiquidData> listLiquidData = listHolderEditor[count].listLiquidData.ToList();
                for (int temp = 0; temp < listLiquidData.Count; temp++)
                {
                    int groupId = listLiquidData[temp].groupId;
                    float value = listLiquidData[temp].value;
                    for (int countValue = 0; countValue < value; countValue++)
                    {
                        values.Add(groupId);
                    }
                }
                LevelColumn levelColumn = new LevelColumn(values);
                map.Add(levelColumn);
            }

            Level level = new Level(levelID, map);
            level.maxWaterInTube = numberWaterInBottle;

            string jsonText = JsonUtility.ToJson(level, true);
            string filePath = GetFilePath(levelID);
            File.WriteAllText(filePath, jsonText);
            string info = "Đã lưu level " + levelID + " thành công!";
        }
        private string GetFilePath(int levelID)
        {
            return Path.Combine(Application.dataPath, sourceTexts.name + "/lv" + levelID + ".json");
        }


        [Serializable]
        public class LevelColumn
        {
            public List<int> values;
            public LevelColumn(List<int> values)
            {
                this.values = values;
            }
        }

        [Serializable]
        public class Level
        {
            public int no;
            public int maxWaterInTube;
            public List<LevelColumn> map;
            public Level(int no, List<LevelColumn> map)
            {
                this.no = no;
                this.map = map;
            }
        }

    }
}
