using System.Collections;
using UnityEngine;

public class EnemyDirector : MonoBehaviour
{
    [Header("Horde settings: ")]
    [SerializeField] private int maxAlive = 40;
    [SerializeField] private int hordeSize = 25;

    [Header("Time settings: ")]
    [SerializeField] private float timeBetweenWaves = 30.0f;
    [SerializeField] private float spawnInterval = 0.1f;

    [Header("Spawn settings: ")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 25.0f;
    [SerializeField] private Transform player;

    private int aliveCount = 0;
    private float timer = 0.0f;
    private Pathfinder pathfinder;
    private bool spawning = false;

    private void Awake()
    {
        pathfinder = FindFirstObjectByType<Pathfinder>();
        timer = timeBetweenWaves - 1.0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (!spawning && timer >= timeBetweenWaves && aliveCount < maxAlive)
        {
            timer = 0.0f;
            StartCoroutine(SpawnHorde());
        }
    }

    private IEnumerator SpawnHorde()
    {
        spawning = true;
        int toSpawn = Mathf.Min(hordeSize, maxAlive -  aliveCount);

        for (int i = 0; i < toSpawn; i++)
        {
            if (TrySpawnEnemy())
            {
                aliveCount++;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
        spawning = false;
    }

    private bool TrySpawnEnemy()
    {
        if (!TryGetSpawnPos(out Vector3 pos))
            return false;

        Instantiate(enemyPrefab, pos, Quaternion.identity);
        return true;
    }

    public void OnEnemyKilled()
    {
        aliveCount = Mathf.Max(0, aliveCount - 1);
    }

    private bool TryGetSpawnPos(out Vector3 spawnPos)
    {
        if (!player || !pathfinder)
        {
            spawnPos = Vector3.zero;
            return false;
        }

        for (int i = 0; i < 15; i++)
        {
            Vector3 candidate = player.position + Random.insideUnitSphere * spawnRadius;

            candidate.y = player.position.y;

            if (!pathfinder.CanReach(candidate, player.position)) continue;

            spawnPos = candidate;
            return true;
        }

        spawnPos = Vector3.zero;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!player) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, spawnRadius);
    }
}
