using System.Collections;
using UnityEngine;

public class AttackEffectsManager : MonoBehaviour
{
    public static AttackEffectsManager Instance { get; private set; }

    [Header("화면 흔들림 설정")]
    private Transform mainCameraTransform;
    private Vector3 originalCameraPos;
    private Coroutine cameraShakeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    public void PlayScreenShake(float duration, float magnitude)
    {
        if (mainCameraTransform == null) return;

        if (cameraShakeCoroutine != null)
        {
            StopCoroutine(cameraShakeCoroutine);
            mainCameraTransform.localPosition = originalCameraPos;
        }

        originalCameraPos = mainCameraTransform.localPosition;
        cameraShakeCoroutine = StartCoroutine(ScreenShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ScreenShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCameraTransform.localPosition = originalCameraPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCameraTransform.localPosition = originalCameraPos;
        cameraShakeCoroutine = null;
    }
}