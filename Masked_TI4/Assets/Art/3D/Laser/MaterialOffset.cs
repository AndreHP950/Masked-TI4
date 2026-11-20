using UnityEngine;

public class MaterialOffset : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float offsetX;
    public int interator;
    public bool jira = false;
    private Renderer rend;
    public GameObject obj;
    void Start()
    {

        rend = obj.GetComponent<Renderer>();
        rend.material.mainTextureOffset = Vector2.zero;
    }

    void FixedUpdate()
    {

    }
    void Update()
    {
        if (jira)
        {
            Vector2 offset = rend.material.mainTextureOffset;
            offset.x += offsetX * Time.deltaTime;

            rend.material.mainTextureOffset = offset;
        }
       


    }
    public void Girar()
    {
       
        jira = true;
    }


    public void AtivareDesativarGameObj()
    {
        obj.SetActive(!obj.activeSelf);
    }
}
