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
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask obstacleMask;

    private int aliveCount = 0;
    private float timer = 0.0f;
    private GridManager gridManager;
    private bool spawning = false;

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
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
        if (!player || !gridManager)
        {
            spawnPos = Vector3.zero;
            return false;
        }

        for (int i = 0; i < 15; i++)
        {
            Vector3 candidate = player.position + Random.insideUnitSphere * spawnRadius;

            candidate.y = 50f;

            // raycast to spawn at correct height
            if (Physics.Raycast(candidate, Vector3.down, out RaycastHit hitInfo, 100.0f, groundMask))
            {
                // check if walkable
                if (!Physics.CheckSphere(hitInfo.point, 0.5f, obstacleMask))
                {
                    spawnPos = hitInfo.point;
                    return true;
                }
            }

            if (!gridManager.IsInsideGrid(candidate))
            {
                continue;
            }

            PathNode node = gridManager.NodeFromWorldPoint(candidate);
            if (!node.walkable) continue;

            if (!gridManager.CanReach(node.worldPos, player.position))
            {
                continue;
            }

            spawnPos = node.worldPos;
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
