using UnityEngine;

public class MoverETeleportar : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 5f;

    [Header("Chao")]
    public Transform detectorChao;
    public float distanciaChao = 0.2f;
    public LayerMask camadaChao;

    [Header("Teletransporte")]
    public GameObject[] destinos;

    private Rigidbody rb;
    private bool noChao;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Verifica se está encostando no chão
        noChao = Physics.Raycast(
            detectorChao.position,
            Vector3.down,
            distanciaChao,
            camadaChao
        );

        // Só se move se estiver no chão
        if (noChao)
        {
            Vector3 movimento =
                Vector3.forward * velocidade * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + movimento);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tele"))
        {
            Teletransportar();
        }
    }

    void Teletransportar()
    {
        if (destinos == null || destinos.Length == 0)
            return;

        int indice = Random.Range(0, destinos.Length);
        GameObject destino = destinos[indice];

        if (destino != null)
        {
            rb.position = destino.transform.position;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectorChao == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            detectorChao.position,
            detectorChao.position + Vector3.down * distanciaChao
        );
    }
}