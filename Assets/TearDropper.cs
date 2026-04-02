using UnityEngine;

public class TearDropper : MonoBehaviour
{
    public float dropInterval = 3;
    public GameObject dropPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("DropTear", 0, dropInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropTear()
    {
        Instantiate(dropPrefab, transform.position, Quaternion.identity);
    }
}
