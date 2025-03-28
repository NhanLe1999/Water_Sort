using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class CCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] SpriteRenderer spriteRenderer;
    void Start()
    {
        float size = spriteRenderer.bounds.size.x * Screen.height / Screen.width * 0.5f;
        mainCamera.orthographicSize = size;
    }
}
