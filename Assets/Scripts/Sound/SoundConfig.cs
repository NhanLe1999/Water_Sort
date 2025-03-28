using UnityEngine;
namespace WaterSort
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Config/AudioConfig")]
    public class SoundConfig : ScriptableObject
    {
        public SoundKeyItem[] sounds;
    }
}
