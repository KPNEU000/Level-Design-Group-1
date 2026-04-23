using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PaperBehavior : MonoBehaviour
{

    private MaterialPropertyBlock propBlock;
    private Renderer rend;
    private float highSetting = 10;
    private float baseSetting = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Ins.OnPapersGrounded += Illuminate;
        propBlock = new MaterialPropertyBlock();
        rend = GetComponent<Renderer>();
        propBlock.SetFloat("_SelfIllumination", baseSetting);
        //rend.SetPropertyBlock(propBlock);
    }

    void OnDisable()
    {
        GameManager.Ins.OnPapersGrounded -= Illuminate;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monitor"))
        {
            GameManager.Ins.CollectPaper(gameObject);
        }
    }

    public void Illuminate()
    {
        Debug.Log("illuminate papers");
        propBlock.SetFloat("_SelfIllumination", highSetting);
        rend.SetPropertyBlock(propBlock);
    }
}
