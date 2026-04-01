using UnityEngine;

public class SpawnOnTouch : MonoBehaviour
{
    public GameObject arenaPrefab;
    public GameObject hallwayPrefab;
    Vector3 hallwaySpawnPoint;
    GameObject hallwayInstance;

    void Start()
    {
        hallwaySpawnPoint = new Vector3(2.2f, 0.36337f, -1.84108f);
        hallwayInstance = Instantiate(hallwayPrefab, hallwaySpawnPoint, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hey there");
        if (other.CompareTag("Player"))
        {
            SpawnArena();
            DespawnHallway();
        }
    }

    void SpawnArena()
    {
        Vector3 positionToSpawn = new Vector3(0, 0, 0);
        Instantiate(arenaPrefab, positionToSpawn, Quaternion.identity);
    }

    void DespawnHallway()
    {
        if (hallwayInstance != null)
        {
            Destroy(hallwayInstance);
        }
    }
}
