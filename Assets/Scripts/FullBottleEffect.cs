using DG.Tweening;
using UnityEngine;
namespace WaterSort
{
    public class FullBottleEffect : MonoBehaviour
    {
         private float timeActive = 3f;
       
        private void OnEnable()
        {         
            DOVirtual.DelayedCall(timeActive, () =>
            {
                Destroy(gameObject);
            });
        }     

        public void SorttingLayerId(int sortingOrderID, Color color)
        {
            /*
            gameObject.SetActive(true);

            var particleTrailMain = particleTrail.main;
            particleTrailMain.startColor = color;           
            particleTrail.GetComponent<Renderer>().sortingOrder = sortingOrderID;

            var particleFireworkMain = particleFirework.main;
            particleFireworkMain.startColor = color;
            particleFirework.GetComponent<Renderer>().sortingOrder = sortingOrderID + 1;
            */
        }

    }
}
