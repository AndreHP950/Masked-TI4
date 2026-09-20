using UnityEngine;

public class ObjSide : MonoBehaviour
{
    public int z;
    public float range;
    float pos;
    private bool reachedLimit = false;

    private void Start()
    {
        pos = this.gameObject.transform.position.z;
    }
    public void Move(int direction)
    {
        transform.position += (new Vector3(0, 0, z) * Time.deltaTime * z) * direction;
    }
    void Update()
    {
        if (transform.position.z >= range + pos && reachedLimit == false) // Se chegou ao limite da frente inverter sentido
        {
            reachedLimit = true;
        }
        if (transform.position.z <= (range - pos) * -1 && reachedLimit == true) // Se chegou ao limite de tras inverter sentido
        {
            reachedLimit = false;
        }
        if (transform.position.z <= range + pos && reachedLimit == false) // Mexe pra frente
        {
            Move(1);
        }
        if (transform.position.z >= (range - pos) * -1 && reachedLimit == true) // Mexe pra tras
        {
            Move(-1);
        }
    }
}