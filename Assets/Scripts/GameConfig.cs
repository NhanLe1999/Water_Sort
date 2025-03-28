using System;
using UnityEngine;
namespace WaterSort
{
    public class GameConfig
    {
        public static string APPLE_ID = "1589179194";

        public const int TOTAL_BOTTLE = 6;
        public const int TOTAL_BACKGROUND = 7;
        public const int TOTAL_TROPHY = 5;
        public static bool HAS_BLIND_MODE = false;
        public const float HIEGHT_IMAGE_BOTTLE = 4.7f;
        public static event Action OnChangeCoin;
        public static int COIN
        {
            get => PrefManager.GetInt(nameof(COIN), 0);
            set
            {
                PrefManager.SetInt(nameof(COIN), value);
                OnChangeCoin?.Invoke();
            }
        }
        public static bool ConsentActive
        {
            get => PrefManager.GetBool(nameof(ConsentActive));
            set => PrefManager.SetBool(nameof(ConsentActive), value);
        }
        public static bool HaveSetupConsent => PrefManager.HasKey(nameof(ConsentActive));

        public static bool Rated
        {
            get { return PrefManager.GetBool(nameof(Rated)); }
            set { PrefManager.SetBool(nameof(Rated), value); }
        }
        public static bool BLIND_MODE
        {
            get => PrefManager.GetInt(nameof(BLIND_MODE), 0) == 1;
            set => PrefManager.SetInt(nameof(BLIND_MODE), value ? 1 : 0);
        }
        public static int ID_BOTTLE
        {
            get => PrefManager.GetInt(nameof(ID_BOTTLE), 0);
            set => PrefManager.SetInt(nameof(ID_BOTTLE), value);
        }

        public static int ID_BOTTLE_SELECT;

        public static int ID_BACKGROUND
        {
            get => PrefManager.GetInt(nameof(ID_BACKGROUND), 0);
            set => PrefManager.SetInt(nameof(ID_BACKGROUND), value);
        }

        public static bool IsUnlock(ItemShop.Type type, int id)
        {
            if (id == 0) return true;
            if (type == ItemShop.Type.Bottle)
                return PrefManager.GetInt("Unlock_Bottle_" + id, 0) == 1;
            else
                return PrefManager.GetInt("Unlock_Background_" + id, 0) == 1;
        }
        public static void Unlock(ItemShop.Type type, int id)
        {
            if (type == ItemShop.Type.Bottle)
                PrefManager.SetInt("Unlock_Bottle_" + id, 1);
            else
                PrefManager.SetInt("Unlock_Background_" + id, 1);
        }

        public static bool IsRequestBottle(int id)
        {
            return PrefManager.GetInt("Request_Bottle_" + id, 0) == 1;
        }

        public static void RequestBottle(int id)
        {
            PrefManager.SetInt("Request_Bottle_" + id, 1);
        }

        public static bool IsRequestLevel(int id)
        {
            return PrefManager.GetInt("Request_Level_" + id, 0) == 1;
        }

        public static void RequestLevel(int id)
        {
            PrefManager.SetInt("Request_Level_" + id, 1);
        }

        public static float WidthScreen()
        {
            return PlayerPrefs.GetFloat("WidthScreen", Camera.main.ViewportToWorldPoint(new Vector3(1, 1)).x * 2);
        }
        public static void Rate()
        {
            D2S.Ads.AdsController.Instance.LeftApplicationWithoutShowAdsWhenComeBack();
#if UNITY_ANDROID
            Application.OpenURL("market://details?id=" + Application.identifier);
#elif UNITY_IOS
        Application.OpenURL("itms-apps://itunes.apple.com/app/" + APPLE_ID);
#endif
            GameConfig.Rated = true;
        }
    }
}