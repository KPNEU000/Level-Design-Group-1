using UnityEngine;

public class FogScript : MonoBehaviour
{
    GameObject Player;
    public float distanceWhenInFog = 13.5f;
    public float distanceToPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
        if(Vector3.Distance(transform.position, Player.transform.position) <= distanceWhenInFog)
        {
            RenderSettings.fogDensity = 1;
        }
        else
        {
            RenderSettings.fogDensity = 0;
        }
    }
}
