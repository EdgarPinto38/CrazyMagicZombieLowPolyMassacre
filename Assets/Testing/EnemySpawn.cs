using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int maxEnemies = 10;
    public float spawnInterval = 5f;

    private int currentEnemies = 0;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", spawnInterval, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (currentEnemies < maxEnemies)
        {
            GameObject enemy = (GameObject)Instantiate(enemyPrefab, transform.position, transform.rotation);
            currentEnemies++;
        }
    }
}
