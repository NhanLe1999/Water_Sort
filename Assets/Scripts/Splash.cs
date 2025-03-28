using System.Collections;
using UnityEngine;

namespace WaterSort
{
    public class Splash : MonoBehaviour
    {
        private void Start()
        {
            /*
#if UNITY_ANDROID
            if (!GameConfig.HaveSetupConsent)
            {
                SharedUIManager.ConsentPanel.Show();
                yield return new WaitUntil(() => !SharedUIManager.ConsentPanel.Showing);
            }
#endif
            yield return new WaitForEndOfFrame();
            */
            GameManager.LoadScene("MainMenu");
        }
    }
}