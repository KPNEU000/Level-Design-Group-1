using UnityEngine;

public class EdibleObject : MonoBehaviour
{
    public float damage = 10;
    public enum Type {ORGANIC, INORGANIC};
    public bool held;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (held)
        {
            
        }
    }
}
