using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace WaterSort
{
    public partial class ResourceManager : Singleton<ResourceManager>
    {
        public static bool EnableAds
        {
            get => PrefManager.GetBool(nameof(EnableAds), true);
            set => PrefManager.SetBool(nameof(EnableAds), value);
        }

        protected override void OnInit()
        {
            base.OnInit();
        }
    }


    public partial class ResourceManager
    {
        public int TotalLevel = 300;
        public int[] TotalChallenge;

        public Color[] _normalColors = new Color[11];
        public Color[] _blindColors = new Color[11];
        public static int GetLevelCount(GameMode mode)
        {
            if (mode == GameMode.Undefined) return Instance.TotalLevel;
            return 0;
        }

        public static Level GetLevel(GameMode mode, int no)
        {
            if (mode == GameMode.DailyChallenge)
            {
                if (!IsHaveData(GameStatics.DATA_DAILY_LEVEL))
                {
                    string pathChallenge = string.Empty;
                    switch (no)
                    {
                        case 1:
                            pathChallenge = string.Format("level1/lv{0}", UnityEngine.Random.Range(1, Instance.TotalChallenge[0]));
                            break;
                        case 2:
                            pathChallenge = string.Format("level2/lv{0}", UnityEngine.Random.Range(1, Instance.TotalChallenge[1]));
                            break;
                        case 3:
                            pathChallenge = string.Format("level3/lv{0}", UnityEngine.Random.Range(1, Instance.TotalChallenge[2] / 2));
                            break;
                        case 4:
                            pathChallenge = string.Format("level3/lv{0}", UnityEngine.Random.Range(Instance.TotalChallenge[2] / 2, Instance.TotalChallenge[2]));
                            break;
                        case 5:
                            pathChallenge = string.Format("level4/lv{0}", UnityEngine.Random.Range(1, Instance.TotalChallenge[3]));
                            break;
                    }
#if UNITY_EDITOR
                    Debug.Log(pathChallenge);
#endif
                    var levelChallenge = LoadLevelFromResources(pathChallenge);
                    if (levelChallenge != null) return (Level)levelChallenge;
                    else Debug.LogError("Không load được map challenge");
                }
                else
                {
                    Level levelChallenge = JsonUtility.FromJson<Level>(GameStatics.DATA_DAILY_LEVEL);
                    return levelChallenge;
                }
            }
            else if (mode == GameMode.Undefined)
            {
                // data level mới
                if (!IsHaveData(GameStatics.DATA_NOMAL_LEVEL))
                {
                    if (no > Instance.TotalLevel)
                        no = Instance.TotalLevel;
                    string path = "Levels/lv" + no;
                    var level = LoadLevelFromResources(path);
                    if (level != null) return (Level)level;
                    else
                    {
                        Debug.LogError("Không có map mới path: " + path + ". =>> Lấy map cũ");
                    }
                }
                else
                {
                    Level level = JsonUtility.FromJson<Level>(GameStatics.DATA_NOMAL_LEVEL);
                    return level;
                }
            }

            return new Level();
            // data level cũ


        }

        private static bool IsHaveData(string data)
        {
            return data != "" && data.Length > 5;
        }
        public static Level GetLevel(string path)
        {
            return (Level)LoadLevelFromResources(path);
        }
        private static Level? LoadLevelFromResources(string path)
        {
            var textAssets = Resources.Load<TextAsset>(path);
            if (textAssets != null)
            {
                Level data = JsonUtility.FromJson<Level>(textAssets.text);
                return data;
            }
            else
            {
                return null;
            }
        }

        public static bool IsLevelLocked(GameMode mode, int no)
        {
            var completedLevel = GetCompletedLevel(mode);

            return no > completedLevel + 1;
        }

        public static int GetCompletedLevel(GameMode mode)
        {
            return PrefManager.GetInt($"{mode}_Level_Complete");
        }

        public static void CompleteLevel(GameMode mode, int lvl)
        {
            if (mode == GameMode.DailyChallenge)
            {
                Debug.Log(DailyChallenge.Instance.LevelInOrder + "ccccc");
                if (DailyChallenge.Instance.LevelInOrder == 5) // hoàn thành đủ 5 cấp độ
                    DailyChallenge.Instance.CompleteChallenge();
                return;
            }
            else
            {
                if (GetMaxCompletedLevel(mode).no > lvl)
                {
                    return;
                }
                PrefManager.SetInt($"{mode}_Level_Complete", lvl);
            }
        }

        public static void SelectLevel(GameMode mode, int lvl)
        {
            PrefManager.SetInt($"{mode}_Level_Complete", lvl);
        }


        public static bool HasLevel(GameMode mode, int lvl) => GetLevelCount(mode) >= lvl;

        public static Level GetMaxCompletedLevel(GameMode mode)
        {
            return GetLevel(mode, PrefManager.GetInt($"{mode}_Level_Complete") + 1);
        }


        public static DataPour DataPourFromResources()
        {
            var textAssets = Resources.Load<TextAsset>("dataPour");
            if (textAssets != null)
            {
                DataPour data = JsonUtility.FromJson<DataPour>(textAssets.text);
                return data;
            }
            else
            {
                return null;
            }
        }

        public static Sprite LoadBackground(int id)
        {
            return Resources.Load<Sprite>("Images/Background/BG-" + (id + 1));
        }

        public static Sprite LoadBottle(int id)
        {
            return Resources.Load<Sprite>("Images/Bottle/bottle-" + (id + 1));
        }


        public static Sprite LoadMask(int id)
        {
            return Resources.Load<Sprite>("Images/Bottle/bottle-" + (id + 1) + "_mask");
        }

        public static Sprite LoadTropy(int id)
        {
            return Resources.Load<Sprite>("Images/Trophy/trophy_" + (id + 1));
        }

        public static float[] XDistanceBottle()
        {
            float[] xDistance = new float[8] { 0, 4.0f, 3.03f, 2.42f, 2f, 1.72f, 1.5f, 1.35f };
            return xDistance;
        }

        public static float[] ExpectWidth()
        {
            float[] xDistance = new float[6] { 0, 3.5f, 2f, 4, 5, 6 };
            return xDistance;
        }
    }

    public enum GameMode
    {
        Easy, Normal, Hard, Expert,
        Undefined, DailyChallenge
    }
}