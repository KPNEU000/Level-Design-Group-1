using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    public static float mouseSensitivity = 100f;
    public float pitchMin = -90;
    public float pitchMax = 90;
    public Transform playerBody;
    float pitch;
    float roll = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // playerBody = transform.parent.transform; //the playerbody is the parent of the camera
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //float moveZ = Input.GetAxis("Mouse ScrollWheel") * mouseSensitivity * Time.deltaTime;

        //Yaw
        if (playerBody)
        {
            playerBody.Rotate(Vector3.up * moveX);
        }

        //Pitch
        pitch -= moveY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        transform.localRotation = Quaternion.Euler(pitch, 0, roll);

        //Roll
        //roll -= moveZ;
        //roll = Mathf.Clamp(roll, pitchMin, pitchMax);
        //transform.localRotation = Quaternion.Euler(pitch, 0, roll);
    }
}
