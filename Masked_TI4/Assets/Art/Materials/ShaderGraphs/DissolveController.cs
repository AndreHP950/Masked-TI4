using UnityEngine;

public class DissolveController : MonoBehaviour
{
    public Renderer objectRenderer;
    public float dissolveSpeed = 0.5f;

    private Material material;
    private float dissolveAmount = 0f;

    void Start()
    {
        material = objectRenderer.material;
        material.SetFloat("_DisolveAmmount", 0f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            dissolveAmount += dissolveSpeed * Time.deltaTime;
            dissolveAmount = Mathf.Clamp01(dissolveAmount);

            material.SetFloat("_DisolveAmmount", dissolveAmount);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            dissolveAmount = 0f;
            material.SetFloat("_DisolveAmmount", dissolveAmount);
        }
    }
}