using System;
using UnityEngine;
namespace WaterSort
{
    public class VibrationManager : Singleton<VibrationManager>
    {
        public static event Action<bool> VibrationStateChanged;

        public static bool IsVibrationEnable
        {
            
            get {
                return false;
                //return PlayerPrefs.GetInt(nameof(IsVibrationEnable), 0) == 1;
            }
            set
            {

                /*
                if (value == IsVibrationEnable)
                {
                    return;
                }

                PlayerPrefs.SetInt(nameof(IsVibrationEnable), value ? 1 : 0);
                VibrationStateChanged?.Invoke(value);
                */
            }
        }
    }
}