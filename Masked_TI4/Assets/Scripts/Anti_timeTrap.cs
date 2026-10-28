using Unity.VisualScripting;
using UnityEngine;

public class Anti_timeTrap : MonoBehaviour
{
    GameController gameController;

    // Update is called once per frame
    void Update()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.disableTravel();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.enableTravel();
        }
    }


}
