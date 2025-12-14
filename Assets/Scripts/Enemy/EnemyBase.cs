using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private int health;
    [SerializeField] private EnemyData data;
    [SerializeField] private AudioClip death;
    [SerializeField, Range(0.0f, 1.0f)] private float deathVolume;

    [Header("Coin burst: ")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float explosionForce = 5.0f;
    [SerializeField] private float explosionRadius = 1.0f;
    [SerializeField] private float upwardModifier = 1.0f;
    [SerializeField] private float minCoins = 5.0f;
    [SerializeField] private float maxCoins = 20.0f;

    private EnemyDirector enemyDirector;

    private int coinCount;

    private void Awake()
    {
        health = data.maxHealth;
        enemyDirector = FindFirstObjectByType<EnemyDirector>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        SoundManager.Instance.PlaySound(death, transform.position, deathVolume, true);

        ExplodeCoins();
        GameManager.Instance.IncrementKill();
        GameManager.Instance.AddPlayerGold(coinCount);
        enemyDirector.OnEnemyKilled();

        Destroy(gameObject);
    }

    private void ExplodeCoins()
    {
        coinCount = Mathf.RoundToInt(Random.Range(minCoins, maxCoins));
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 dir = Random.insideUnitSphere;
            dir.y = Mathf.Abs(dir.y); // bias upward

            Vector3 spawnPos = transform.position + dir * 0.5f;
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            coin.layer = LayerMask.NameToLayer("Coins");

            if (coin.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddExplosionForce(explosionForce, spawnPos, explosionRadius, upwardModifier, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 2.0f, ForceMode.Impulse);
            }
        }
    }
}
