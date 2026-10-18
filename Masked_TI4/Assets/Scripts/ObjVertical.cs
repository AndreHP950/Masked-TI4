using UnityEngine;

public class ObjVertical : MonoBehaviour
{
    public int y;
    public float range; // Range de movimentacao
    private bool reachedTop = false; // Booleana para conferir se chegou no topo ou na base para trocar o sentido de movimentacao
    public void Move(int direction)
    {
        transform.position += (new Vector3(0, y, 0) * Time.deltaTime * y) * direction;
    }
    void Update()
    {
        if (transform.position.y >= range && reachedTop == false) // Se chegou no topo inverter sentido 
        {
            reachedTop = true;
        }
        if (transform.position.y <= range * -1 && reachedTop == true) // Se chegou na base inverter sentido
        {
            reachedTop = false;
        }
        if (transform.position.y <= range && reachedTop == false) // Subindo
        {
            Move(1);
        }
        if (transform.position.y >= range * -1 && reachedTop == true) // Descendo
        {
            Move(-1);
        }
    }
}