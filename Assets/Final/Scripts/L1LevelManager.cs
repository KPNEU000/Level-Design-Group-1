using UnityEngine;

public class L1LevelManager : MonoBehaviour
{
    public GameObject arenaSpawnerTripwirePrefab;
    public GameObject player;
    public GameObject spawnBedDespawnTripwirePrefab;
    public GameObject arenaPrefab;
    public GameObject hallwayPrefab;
    public GameObject targetBedPrefab;
    public GameObject spawnBedPrefab;
    public GameObject walkerPrefab;
    Vector3 hallwaySpawnPoint;
    GameObject hallwayInstance;
    GameObject targetBedInstance;
    GameObject arenaSpawnerTripwireInstance;
    GameObject spawnBedInstance;
    GameObject walkerInstance;
    GameObject spawnBedDespawnTripwireInstance;
    ComaPlayerController pc;

    void Start()
    {
        pc = player.GetComponent<ComaPlayerController>();
        hallwaySpawnPoint = new Vector3(2.2f, 0.36337f, -1.84108f);
        hallwayInstance = Instantiate(hallwayPrefab, hallwaySpawnPoint, Quaternion.identity);
        targetBedInstance = Instantiate(targetBedPrefab, targetBedPrefab.transform.position, Quaternion.identity);
        spawnBedInstance = Instantiate(spawnBedPrefab, spawnBedPrefab.transform.position, Quaternion.identity);
        walkerInstance = Instantiate(walkerPrefab, walkerPrefab.transform.position, Quaternion.identity);
        arenaSpawnerTripwireInstance = Instantiate(arenaSpawnerTripwirePrefab, arenaSpawnerTripwirePrefab.transform.position, Quaternion.identity);
        spawnBedDespawnTripwireInstance = Instantiate(spawnBedDespawnTripwirePrefab, spawnBedDespawnTripwirePrefab.transform.position, Quaternion.identity);
    }

    public void OnTrigger(string name)
    {
        Debug.Log(name);
        Debug.Log(arenaSpawnerTripwireInstance.name);
        if (arenaSpawnerTripwireInstance.name.Contains(name))
        {
            SpawnPrefab(arenaPrefab);
            DespawnInstance(hallwayInstance);
            DespawnInstance(walkerInstance);
        }
        else if (spawnBedDespawnTripwireInstance.name.Contains(name))
        {
            DespawnInstance(spawnBedInstance);
        }
    }

    void SpawnPrefab(GameObject prefab)
    {
        Instantiate(prefab, prefab.transform.position, Quaternion.identity);
    }

    void DespawnInstance(GameObject ins)
    {
        if (ins != null)
        {
            Destroy(ins);
        }
    }
}