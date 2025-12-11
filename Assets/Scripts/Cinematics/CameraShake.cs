using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Camera Shake Settings")]
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin perlin;

    [Header("Shake Curve")]
    public AnimationCurve shakeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Frequency Settings")]
    public float frequency = 10f; // vibrations rapides

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (virtualCamera == null)
            virtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();

        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (perlin == null)
            Debug.LogError("Le composant CinemachineBasicMultiChannelPerlin n'existe pas sur la VirtualCamera !");
    }
    public void Shake(float intensity, float duration)
    {
        if (perlin != null)
            StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;

        float originalFrequency = perlin.m_FrequencyGain;
        perlin.m_FrequencyGain = frequency;

        while (elapsed < duration)
        {
            float normalizedTime = elapsed / duration;
            float curveValue = shakeCurve.Evaluate(normalizedTime);

            // Shake aléatoire sur X, Y et Z pour un tremblement multi-direction
            float shakeX = Random.Range(-1f, 1f) * intensity * curveValue;
            float shakeY = Random.Range(-1f, 1f) * intensity * curveValue;
            float shakeZ = Random.Range(-1f, 1f) * intensity * curveValue;

            perlin.m_AmplitudeGain = Mathf.Sqrt(shakeX * shakeX + shakeY * shakeY + shakeZ * shakeZ); // amplitude globale

            elapsed += Time.deltaTime;
            yield return null;
        }

        perlin.m_AmplitudeGain = 0f;
        perlin.m_FrequencyGain = originalFrequency;
    }
}
