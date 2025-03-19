using UnityEngine;

public class WaterLayer : MonoBehaviour
{
    public Material waterMaterial;
    private SpriteRenderer spriteRenderer;
    public Transform cup;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (waterMaterial == null)
            waterMaterial = spriteRenderer.material;
    }

    void Update()
    {
        float angle = cup.eulerAngles.z * Mathf.Deg2Rad;

        float tiltAmount = Mathf.Tan(angle) * 0.5f;
        waterMaterial.SetFloat("_Tilt", tiltAmount);
    }
}
