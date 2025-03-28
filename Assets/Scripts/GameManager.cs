using System;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace WaterSort
{

    public partial class GameManager : Singleton<GameManager>
    {
        public static bool ENABLE_POLICY = false;
        public static string POLICY_LINK = "";
        public static bool IsTestMode
        {
            get
            {
                return Debug.isDebugBuild;
            }
        }

        public static event Action OnFinishLoadData;
        public static int TOTAL_GAME_COUNT
        {
            get => PrefManager.GetInt(nameof(TOTAL_GAME_COUNT));
            set => PrefManager.SetInt(nameof(TOTAL_GAME_COUNT), value);
        }

        public static LoadGameData LoadGameData { get; set; }

        protected override void OnInit()
        {
            base.OnInit();
            Application.targetFrameRate = 60;

         
        }

        private void Start()
        {
            LoadData();
        }
        public void LoadData()
        {            
           
            var level = ResourceManager.GetMaxCompletedLevel(GameMode.Undefined);
            LoadGameData = new LoadGameData
            {
                Level = ResourceManager.GetLevel(GameMode.Undefined, level.no),
                GameMode = GameMode.Undefined
            };
            OnFinishLoadData?.Invoke();
        }

        public void LoadDataDaily()
        {
            LoadGameData = new LoadGameData
            {
                Level = ResourceManager.GetLevel(GameMode.DailyChallenge, DailyChallenge.Instance.LevelInOrder),
                GameMode = GameMode.DailyChallenge
            };
            OnFinishLoadData?.Invoke();
        }
        
        
        public static HolderDataPour HolderDataPour => ResourceManager.DataPourFromResources().holderDataPours[GameConfig.ID_BOTTLE_SELECT];
       
    }

    public partial class GameManager
    {
        // ReSharper disable once FlagArgument
        public static void LoadScene(string sceneName, bool showLoading = true, float loadingScreenSpeed = 5f)
        {
           
            if (showLoading )
            {
              
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        public static int CountRestart = 0;

        public static void LoadGame(LoadGameData data, bool showLoading = true, float loadingScreenSpeed = 1f)
        {
            CountRestart = 0;
            LoadGameData = data;
            LoadScene("Main", showLoading, loadingScreenSpeed);
        }
        public static void RestartGame(LoadGameData data, bool showLoading = true, float loadingScreenSpeed = 1f)
        {
            CountRestart++;
            LoadGameData = data;
            LoadScene("Main", showLoading, loadingScreenSpeed);
        }

    }


    public struct LoadGameData
    {
        public GameMode GameMode { get; set; }
        public Level Level { get; set; }
    }
}