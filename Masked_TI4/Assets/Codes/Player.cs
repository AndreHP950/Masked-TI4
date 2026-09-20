using UnityEngine;

public class Player : MonoBehaviour
{
    bool vida;
    void Start()
    {
       GameController.instance.Player = this.gameObject;
    }

    // Update is called once per frame
    
}
