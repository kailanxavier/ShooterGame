using System.Collections.Generic;
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

    [Header("Memory: ")]
    [SerializeField] private float visionBuffer = 1.5f; // seconds

    private Rigidbody rb;
    private bool canSeePlayer;
    private float performCheckTimer;
    private float lostSightTimer;

    // pathfinding stuff
    private Pathfinder pathfinder;
    private List<Vector3> currentPath;
    private int currentWaypoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pathfinder = GetComponent<Pathfinder>();

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        player = playerObj.GetComponent<Transform>();
    }

    private void Update()
    {
        if (player == null) return;

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
            else
            {
                // do you remember
            }
        }

        if (lostSightTimer > 0f)
            lostSightTimer -= Time.deltaTime;
        else
            canSeePlayer = false;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= chaseRange && canSeePlayer)
        {
            if (currentPath == null || currentPath.Count == 0 || Vector3.Distance(player.position, currentPath[^1]) > 1f)
            {
                currentPath = pathfinder.FindPath(transform.position, player.position);
                currentWaypoint = 0;
            }

            FollowPath();
        }
    }

    private void FollowPath()
    {
        if (currentPath == null || currentWaypoint >= currentPath.Count) return;

        Vector3 target = currentPath[currentWaypoint];

        Vector3 moveTarget = new Vector3(target.x, transform.position.y, target.z); // flatten pos to remain grounded
        Vector3 dir = (moveTarget - transform.position).normalized;

        // move and rotate enemy towards player
        Quaternion targetRot = Quaternion.LookRotation(dir);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 10f * Time.fixedDeltaTime));
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * dir);

        float near = 0.3f;
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

    public void SetMoveSpeed(float value) => moveSpeed = value;
}
