using UnityEngine;

public class SpeedLinesController : MonoBehaviour
{
    [SerializeField] private Material speedLinesMaterial;

    public void SetSpeedLinesIntensity(float value)
    {
        if (speedLinesMaterial != null)
        {
            speedLinesMaterial.SetFloat("_SpeedLinesIntensity", Mathf.Clamp01(value));
        }
    }
}