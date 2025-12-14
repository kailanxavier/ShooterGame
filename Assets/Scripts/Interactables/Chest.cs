using UnityEngine;

public class Chest : InteractBase
{
    [Header("Chest settings: ")]
    [SerializeField] private AnimationClip openClip;
    [SerializeField] private Collider lidCollider;
    [SerializeField] private AudioClip chestOpen;
    [SerializeField] private AudioClip goldCollect;
    [SerializeField, Range(0.0f, 1.0f)] private float chestOpenVolume = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float goldCollectVolume = 1.0f;
    [SerializeField] private float minGoldReward = 1.0f;
    [SerializeField] private float maxGoldReward = 20.0f;

    [Header("Coin burst: ")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float explosionForce = 5.0f;
    [SerializeField] private float explosionRadius = 1.0f;
    [SerializeField] private float upwardModifier = 1.0f;

    private int coinCount;
    private Animator chestAnimator;
    private bool interacted = false;
    private bool secondInteracted = false;

    private void Awake()
    {
        chestAnimator = GetComponentInChildren<Animator>();
    }

    protected override void Interact()
    {
        if (secondInteracted) return;

        base.Interact();
        chestAnimator.Play(openClip.name);
        lidCollider.enabled = true;

        if (!interacted)
        {
            interacted = true;
            SoundManager.Instance.PlaySound(chestOpen, transform.position, chestOpenVolume);
            return;
        }

        // pickup interaction goes here
        secondInteracted = true;
        ExplodeCoins();
        GameManager.Instance.AddPlayerGold(coinCount);
        SoundManager.Instance.PlaySound(goldCollect, transform.position, goldCollectVolume);
    }

    private void ExplodeCoins()
    {
        coinCount = Mathf.RoundToInt(Random.Range(minGoldReward, maxGoldReward));
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 dir = Random.insideUnitSphere;
            dir.y = Mathf.Abs(dir.y + 2.0f); // bias upward

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
