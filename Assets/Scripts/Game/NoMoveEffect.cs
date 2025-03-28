using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMoveEffect : MonoBehaviour
{
    [SerializeField] GameObject txtNo ;
    [SerializeField] GameObject txtMove;
    private float timeCount;
    private float timeDelay = 0.5f;

    private void OnEnable()
    {
        txtNo.SetActive(true);
        txtMove.SetActive(true);
        timeCount = 0;
    }
    private void Update()
    {
        if(timeCount < timeDelay)
        {
            timeCount += Time.deltaTime;
        }
        else
        {
            timeCount = 0;
            txtNo.SetActive(!txtNo.activeInHierarchy);
            txtMove.SetActive(!txtMove.activeInHierarchy);
        }
    }
}
