using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GasZoneTrigger : MonoBehaviour
{
    [Header("Configurações do Shader")]
    [SerializeField] private Material fullscreenGasMaterial;
    [SerializeField] private ScriptableRendererFeature fullscreenPassFeature;
    [SerializeField] private float fadeSpeed = 3f;

    private float targetAlpha = 0f;
    private float currentAlpha = 0f;

    private void Start()
    {
        if (fullscreenGasMaterial != null)
            fullscreenGasMaterial.SetFloat("_AlphaWeight", 0f);

        if (fullscreenPassFeature != null)
            fullscreenPassFeature.SetActive(false);
    }

    private void Update()
    {
        if (fullscreenGasMaterial == null) return;

        if (Mathf.Abs(currentAlpha - targetAlpha) > 0.001f)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
            fullscreenGasMaterial.SetFloat("_AlphaWeight", currentAlpha);

            if (targetAlpha == 0f && currentAlpha < 0.01f)
            {
                currentAlpha = 0f;
                fullscreenGasMaterial.SetFloat("_AlphaWeight", 0f);

                if (fullscreenPassFeature != null)
                    fullscreenPassFeature.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera"))
        {
            if (fullscreenPassFeature != null)
                fullscreenPassFeature.SetActive(true);

            targetAlpha = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("MainCamera"))
        {
            targetAlpha = 0f;
        }
    }

    private void OnDisable()
    {
        if (fullscreenGasMaterial != null)
            fullscreenGasMaterial.SetFloat("_AlphaWeight", 0f);

        if (fullscreenPassFeature != null)
            fullscreenPassFeature.SetActive(false);
    }
}