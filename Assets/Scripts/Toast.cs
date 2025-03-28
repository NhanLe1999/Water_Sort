using UnityEngine;
namespace WaterSort
{
    public class Toast
    {
        public static void ShowShortText(string str)
        {
#if UNITY_EDITOR
            Debug.Log(str);
            return;
#endif
            UnityNative.Toasts.Example.UnityNativeToastsHelper.ShowShortText(str);
        }
    }
}
