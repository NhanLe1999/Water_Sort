
using UnityEngine;

namespace WaterSort
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] LevelCompletePanel _levelCompletePanel;
        [SerializeField] PausePanel pausePanel;
        [SerializeField] ShopPanel shopPanel;
        [SerializeField] ClaimPanel claimPanel;
        [SerializeField] GamePlayPanel playPanel;
        [SerializeField] PopupGetSkin getBootlePanel;
        [SerializeField] RatePopup ratePopup;
        [SerializeField] DailyChallenge dailyChallenge;
        public void ShowLevelCompleted()
        {
            _levelCompletePanel.Show();
        }

        public void ShowPauseMenu()
        {
            pausePanel.Show();
        }

        public void ShowShop()
        {
            shopPanel.Show();
        }

        public void PlayPaneSetup()
        {
            playPanel.SetUp();
        }
        public void PlayPanelOut()
        {
            playPanel.TopPanelOut();
        }
        public void SetStateSkip()
        {
            playPanel.SetStateButtonSkip();
        }
        public void ResetCountRestart()
        {
            playPanel.CountReplayClick = 0;
        }

        public void ResetCountRestartChallege()
        {
            playPanel.CountRePlayClickChallenge = 0;
        }


        public void PlayPanelIn()
        {          
            playPanel.TopPanelIn();
        }

        public void ShowGetBottle(int idBottle)
        {
            getBootlePanel.Show(idBottle);
        }
        public void ShowClaim(ItemShop.Type type, int id)
        {
            claimPanel.Show(type, id);
        }       

        public void ShowRate()
        {
            ratePopup.Show();
        }

        public void SetActiveDaily(bool isActive)
        {
            dailyChallenge.SetActive(isActive);
        }

        public void ShowPanelDaily(bool isShowChallenge)
        {
            dailyChallenge.ShowPanel(isShowChallenge);
        }
    }
}