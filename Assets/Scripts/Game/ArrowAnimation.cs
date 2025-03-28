using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WaterSort
{
    public class ArrowAnimation : MonoBehaviour
    {
        private void Start()
        {
            transform.DOLocalMoveY(4.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

    }
}
