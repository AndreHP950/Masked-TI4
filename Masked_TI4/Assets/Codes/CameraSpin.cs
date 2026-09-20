using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    public float spinSpeed = 30f;

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }
}