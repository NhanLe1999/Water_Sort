using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParticlePosition : MonoBehaviour
{
    
    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            DOVirtual.DelayedCall(0.5f, () => {
                Vector3 posTopLeft = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
                transform.position = new Vector3(posTopLeft.x, posTopLeft.y, 10);
            });
        }
        else
        {
            Vector3 posTopLeft = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
            transform.position = new Vector3(posTopLeft.x, posTopLeft.y, 10);
        }
    }

    
}
