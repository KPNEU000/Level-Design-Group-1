using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float panSpeed = 20f;
    public float padding = 20f;
    public float scrollSpeed = 3000;
    public Vector2 panLimitX = new Vector2(-20, 20);
    public Vector2 panLimitZ = new Vector2(-20, 20);

    bool controlCamera = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentpos = transform.position;
        if (Input.GetKey(KeyCode.S) && currentpos.x < panLimitX.y)
        {
            currentpos.x += panSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.W) && currentpos.x > panLimitX.x)
        {
            currentpos.x -= panSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) && currentpos.z < panLimitZ.y)
        {
            currentpos.z += panSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A) && currentpos.z > panLimitZ.x)
        {
            currentpos.z -= panSpeed * Time.deltaTime;
        }

        transform.position = currentpos;
    }
}
