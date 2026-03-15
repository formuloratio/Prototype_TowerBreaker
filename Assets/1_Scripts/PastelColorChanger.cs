using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PastelColorChanger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (spriteRenderer == null) return;

        // HSV를 이용한 파스텔톤 생성
        float h = Random.Range(0f, 1f);
        float s = Random.Range(0.2f, 0.4f);
        float v = Random.Range(0.9f, 1f);

        spriteRenderer.color = Color.HSVToRGB(h, s, v);
    }
}