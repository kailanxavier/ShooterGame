using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyAI : MonoBehaviour
{
    [Header("Target: ")]
    public Transform player;
    [SerializeField] private string playerTag = "Player";

    [Header("Movement settings: ")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseRange = 7f;
    [SerializeField] private float sightCheckFrequency = 0.2f;

    [Header("Visibility checks: ")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform enemyEyeHeight;

    [Header("Attack settings: ")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float windUpTime = 1.3f;
    [SerializeField] private AudioClip attackSound;

    [Header("Memory: ")]
    [SerializeField] private float visionBuffer = 1.5f; // seconds

    private Rigidbody rb;
    private bool canSeePlayer;
    private float performCheckTimer;
    private float lostSightTimer;

    // attack player
    private float nextAttackTime;
    private bool isAttacking = false;

    // pathfinding stuff
    private Pathfinder pathfinder;
    private List<Vector3> currentPath;
    private int currentWaypoint;
    private float repathDelay = 0.5f;
    private float repathTimer = 0f;

    private EnemyAnimator animator;

    private void Start()
    {
        repathDelay = 0.4f;
        repathTimer = Random.Range(0f, 1f);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pathfinder = GetComponent<Pathfinder>();

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        player = playerObj.GetComponent<Transform>();

        animator = GetComponent<EnemyAnimator>();
    }

    private void Update()
    {
        if (player == null) return;
        if (isAttacking) return;

        performCheckTimer -= Time.deltaTime;
        if (performCheckTimer <= 0)
        {
            performCheckTimer = sightCheckFrequency;
            bool seesNow = CheckLineOfSight();

            if (seesNow)
            {
                canSeePlayer = true;
                lostSightTimer = visionBuffer;
            }
        }

        // memory timer
        if (lostSightTimer > 0f)
            lostSightTimer -= Time.deltaTime;
        else
            canSeePlayer = false;

        // calculate distance from player
        float dist = Vector3.Distance(transform.position, player.position);
        repathTimer -= Time.deltaTime;

        // chase
        if (dist <= chaseRange && canSeePlayer)
        {
            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f)
            {
                repathTimer = repathDelay;
                Vector3 pathToPlayer = player.position - transform.position;
                pathToPlayer.y = 0.0f;
                Vector3 target = player.position - pathToPlayer.normalized * (attackRange * 0.9f);
                QueueManager.RequestPath(transform.position, target, OnPathFound);
            }
            FollowPath();
        }

        // attack
        if (dist <= attackRange && CanAttackPlayer())
        {
            Attack();
            return;
        }
    }

    private void OnPathFound(List<Vector3> path)
    {
        currentPath = path;
        currentWaypoint = 0;
    }

    private void FollowPath()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange) return;
        if (isAttacking) return;

        if (currentPath == null || currentWaypoint >= currentPath.Count) return;

        Vector3 target = currentPath[currentWaypoint];
        Vector3 moveTarget = new Vector3(target.x, transform.position.y, target.z); // flatten pos to remain grounded
        Vector3 dir = (moveTarget - transform.position).normalized;

        // move and rotate enemy towards player
        Quaternion targetRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 10f * Time.deltaTime));
        rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * dir);

        float near = 0.5f;
        if (Vector3.Distance(transform.position, moveTarget) < near)
        {
            currentWaypoint++;
        }
    }

    private bool CheckLineOfSight()
    {
        if (player == null) return false;

        Vector3 origin = enemyEyeHeight.position;
        Vector3 dir = (player.position + Vector3.up * 1f) - origin;
        float dist = dir.magnitude;
        int combinedMask = obstacleMask | playerMask;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hitInfo, dist, combinedMask))
        {
            return hitInfo.collider.CompareTag(playerTag);
        }

        return false;
    }

    public void Attack()
    {
        if (isAttacking) return;
        if (Time.time < nextAttackTime) return;

        isAttacking = true;

        rb.linearVelocity = Vector3.zero;
        currentPath = null;

        StartCoroutine(AttackCoroutine());
    }

    // TODO: just make this better, idk whats wrong rn
    private IEnumerator AttackCoroutine()
    {
        float t = 0.0f;
        while (t < windUpTime)
        {
            t += Time.deltaTime;

            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.001)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, Time.deltaTime * 8.0f);
            }

            // check if player escaped
            if (Vector3.Distance(transform.position, player.position) > attackRange * 1.2f)
            {
                isAttacking = false;
                yield break;
            }

            yield return null;
        }

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            // animate
            animator.SetAttackBool(true);

            // fucked way to sync audio and animation/damage
            yield return new WaitForSeconds(0.2f);

            if (player.TryGetComponent<PlayerHealth>(out var health))
            {
                health.TakeDamage(attackDamage);
                // play damage sound
                SoundManager.Instance.PlaySoundWithRandomPitch(attackSound, player.position, 0.5f);
            }
        }

        nextAttackTime = Time.time + attackCooldown;

        // recover
        yield return new WaitForSeconds(0.95f);

        animator.SetAttackBool(false);
        isAttacking = false;
    }

    public bool CanAttackPlayer()
    {
        if (player == null) return false;
        if (isAttacking) return false;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange) return false;
        return Time.time >= nextAttackTime;
    }


    public void SetMoveSpeed(float value) => moveSpeed = value;

    public float MoveSpeed => rb.linearVelocity.magnitude;
}
