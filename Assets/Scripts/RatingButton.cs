using UnityEngine;
using UnityEngine.EventSystems;
namespace WaterSort
{
    public class RatingButton : MonoBehaviour, IPointerClickHandler
    {
        public static bool Rated
        {
            get { return PrefManager.GetBool(nameof(Rated)); }
            private set { PrefManager.SetBool(nameof(Rated), value); }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OpenUrl();
        }

        public static void OpenUrl()
        {
            GameConfig.Rate();
        }
    }
}