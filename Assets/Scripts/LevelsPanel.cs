using WaterSort;
using System.Collections.Generic;
using UnityEngine;
namespace WaterSort
{
    public class LevelsPanel : ShowHidable
    {
        [SerializeField] private LevelTileUI _levelTileUIPrefab;
        [SerializeField] private RectTransform _content;
        public GameMode GameMode
        {
            get => _gameMode;
            set
            {
                _gameMode = value;

                //var levels = ResourceManager.GetLevels(value).ToList();
                int totalLevel = ResourceManager.GetLevelCount(value);

                for (var i = 0; i < /*levels.Count*/ totalLevel; i++)
                {
                    //var level = levels[i];
                    if (_tiles.Count <= i)
                    {
                        var levelTileUI = Instantiate(_levelTileUIPrefab, _content);
                        levelTileUI.Clicked += LevelTileUIOnClicked;
                        _tiles.Add(levelTileUI);
                    }
                    _tiles[i].MViewModel = new LevelTileUI.ViewModel
                    {
                        levelId = (i + 1)/* level.no*/,
                        //Level = level,
                        Locked = GameManager.IsTestMode ? false : ResourceManager.IsLevelLocked(value, (i + 1)/* level.no*/),
                        Completed = ResourceManager.GetCompletedLevel(value) >= (i + 1)/* level.no*/
                    };
                }

            }
        }



        private readonly List<LevelTileUI> _tiles = new List<LevelTileUI>();
        private GameMode _gameMode;


        private void LevelTileUIOnClicked(LevelTileUI tileUI)
        {
            if (tileUI.MViewModel.Locked)
            {
                return;
            }

            GameManager.LoadGame(new LoadGameData
            {
                Level = ResourceManager.GetLevel(GameMode, tileUI.MViewModel.levelId),// tileUI.MViewModel.Level, 
                GameMode = GameMode
            });
        }
    }
}