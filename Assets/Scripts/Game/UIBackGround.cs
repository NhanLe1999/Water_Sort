using UnityEngine;
using UnityEngine.UI;

namespace WaterSort
{
    public class UIBackGround : MonoBehaviour
    {
        [SerializeField] Image image;
        void Awake()
        {
            ShopPanel_OnUpdateSkin();
        }
        private void OnEnable()
        {
            ShopPanel.OnUpdateSkin += ShopPanel_OnUpdateSkin;
        }
        private void OnDisable()
        {
            ShopPanel.OnUpdateSkin -= ShopPanel_OnUpdateSkin;
        }

        private void ShopPanel_OnUpdateSkin()
        {
            image.sprite = ResourceManager.LoadBackground(GameConfig.ID_BACKGROUND);
        }
    }
}
