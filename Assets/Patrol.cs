using System;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    public GameObject[] waypoints;
    private NavMeshAgent navMeshAgent;
    private int destination = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        GoToPoint();
    }

    private void GoToPoint()
    {
        if (waypoints.Length != 0)
        {
            navMeshAgent.destination = waypoints[destination].transform.position;
            destination = (destination + 1) % waypoints.Length;
        }
    }


    void Update ()
    {
        transform.LookAt(navMeshAgent.destination);
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            GoToPoint();
    }
}
