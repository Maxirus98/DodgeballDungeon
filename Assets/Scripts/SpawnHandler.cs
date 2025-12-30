using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour
{
    public List<GameObject> monsterPrefabsToSpawn;
    public float spawnCooldown = 5f;
    private float timeStamp;

    private List<Transform> spawnPoints;
    private int spawnedIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoints = new List<Transform>(GetComponentsInChildren<Transform>());
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn random monster at random spawn point with a 5 second interval
        if (Time.time >= timeStamp)
        {
            spawnedIndex = GenerateRandomIndex(spawnedIndex, spawnPoints.Count);
            var randomSpawnPoint = spawnPoints[spawnedIndex];
            var randomMonsterPrefab = monsterPrefabsToSpawn[Random.Range(0, monsterPrefabsToSpawn.Count)];
            Instantiate(randomMonsterPrefab, randomSpawnPoint.position, Quaternion.identity);
            timeStamp = Time.time + spawnCooldown;
        }
    }

    // Generate a random index excluding the specified index. TODO: Move to utility class if needed elsewhere.
    private int GenerateRandomIndex(int excludeIndex, int upperBound)
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, upperBound);
        } while (randomIndex == excludeIndex || randomIndex == 0);
        return randomIndex;
    }
}
