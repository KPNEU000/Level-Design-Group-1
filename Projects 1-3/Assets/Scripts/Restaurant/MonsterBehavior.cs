using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterBehavior : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public float fireCoolDown = 0;
    public float fireRate = 0.5f;
    public GameObject acidSpew;
    public Transform mouth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
        if(Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= 10)
        {
            if (fireCoolDown <= 0)
            {
                mouth.transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform.position);
                Instantiate(acidSpew, mouth.position + transform.forward, mouth.rotation);
                fireCoolDown = 5;
            }
            
            fireCoolDown -= Time.deltaTime;
        }
    }
}

