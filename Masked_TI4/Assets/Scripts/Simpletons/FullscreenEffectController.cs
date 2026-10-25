using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FullscreenEffectController : MonoBehaviour
{
    public static FullscreenEffectController instace;

    [SerializeField] private string rendererFeatureName = "FullScreenPassRendererFeature";

    private FullScreenPassRendererFeature reder;

    void Awake()
    {
        instace = this;
    }

    void Start()
    {
        var data = GameController.instance.data;

        foreach (var feature in data.rendererFeatures)
        {
            if (feature != null && feature.name == rendererFeatureName)
            {
                reder = feature as FullScreenPassRendererFeature;
                break;
            }
        }

        if (reder == null)
        {
            Debug.LogError(
                $"Renderer Feature '{rendererFeatureName}' not found or is not a FullScreenPassRendererFeature."
            );
        }
    }

    public void UpdateFullscreenEffect(float teste, Material material)
    {
        if (reder == null)
            return;

        if (teste >= 1.5f)
        {
            reder.passMaterial = material;
            reder.SetActive(true);
        }
        else
        {
            reder.SetActive(false);
        }
    }
}