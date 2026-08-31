public interface ITrigger
{
    void Activate();
}


```using UnityEngine;

public class SpikeTrap : MonoBehaviour, ITrigger
{
    public void Activate()
    {
        Debug.Log("Armadilha ativada");
    }
}```