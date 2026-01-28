using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public List<Transform> spawnPositions = new List<Transform>();
    public int maxMonsters = 10;
    public GameObject monsterPrefab;
    public bool isActive = false;

    void Start()
    {
        
    }

    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Mess").Length == 0 && isActive == false)
        {
            isActive = true;
        }
        if(isActive) {
            if(GameObject.FindGameObjectsWithTag("Enemy").Length < maxMonsters)
            {
                SpawnMonster();
            }
        }
    }

    void SpawnMonster()
    {
        Instantiate(monsterPrefab, spawnPositions[Random.Range(0, spawnPositions.Count - 1)]);
    }
}
