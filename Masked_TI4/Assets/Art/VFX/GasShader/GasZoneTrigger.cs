
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
        SetEffectAlpha(0f);
        if (fullscreenPassFeature != null) fullscreenPassFeature.SetActive(false);
    }

    private void Update()
    {
        if (fullscreenGasMaterial == null) return;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
        SetEffectAlpha(currentAlpha);

        if (targetAlpha == 0f && currentAlpha <= 0f && fullscreenPassFeature != null)
            fullscreenPassFeature.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;

        targetAlpha = 1f;
        if (fullscreenPassFeature != null) fullscreenPassFeature.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        targetAlpha = 0f;
    }

    public void ResetGasEffect()
    {
        targetAlpha = 0f;
        currentAlpha = 0f;
        SetEffectAlpha(0f);
        if (fullscreenPassFeature != null) fullscreenPassFeature.SetActive(false);
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("MainCamera");
    }

    private void SetEffectAlpha(float alpha)
    {
        if (fullscreenGasMaterial != null)
            fullscreenGasMaterial.SetFloat("_AlphaWeight", alpha);
    }

    private void OnDisable()
    {
        ResetGasEffect();
    }
}
