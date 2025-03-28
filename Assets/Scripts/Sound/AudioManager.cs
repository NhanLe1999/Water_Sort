using System;
using UnityEngine;

namespace WaterSort
{
    public class AudioManager : Singleton<AudioManager>
    {
        public static event Action<bool> SoundStateChanged;

        public static bool IsSoundEnable
        {
            get { return PlayerPrefs.GetInt(nameof(IsSoundEnable), 1) == 1; }
            set
            {
                if (value == IsSoundEnable)
                {
                    return;
                }

                PlayerPrefs.SetInt(nameof(IsSoundEnable), value ? 1 : 0);
                SoundStateChanged?.Invoke(value);
            }
        }


        public static event Action<bool> MusicStateChanged;
        public static bool IsMusicEnable
        {
            get { return PlayerPrefs.GetInt(nameof(IsMusicEnable), 1) == 1; }
            set
            {
                if (value == IsMusicEnable)
                {
                    return;
                }

                PlayerPrefs.SetInt(nameof(IsMusicEnable), value ? 1 : 0);
                MusicStateChanged?.Invoke(value);
            }
        }
    }
}