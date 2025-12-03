using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private LayerMask attackableMask;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private float attackAngle = 75f;
    [SerializeField] private int damage = 35;
    [SerializeField] private bool canAttack = true;

    [SerializeField] private Transform attackOrigin;

    private PlayerAnimator playerAnimator;

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        if (InputManager.Instance.Attack && canAttack)
        {
            Attack();
            playerAnimator.AnimateAttack();

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void Attack()
    {
        Vector3 origin = attackOrigin.position;
        Vector3 forward = attackOrigin.forward;

        Collider[] hits = Physics.OverlapSphere(origin, attackRadius, attackableMask);

        List<Collider> validHits = new();

        foreach (var c in hits)
        {
            Vector3 dir = (c.transform.position - origin).normalized;

            if (Vector3.Angle(forward, dir) <= attackAngle)
            {
                if (Physics.Raycast(origin, dir, out RaycastHit hitInfo, attackRange, attackableMask))
                {
                    if (hitInfo.collider == c)
                    {
                        validHits.Add(c);
                    }
                }
            }
        }

        foreach (var hit in validHits)
        {
            Debug.Log("Hit: " + hit.name);
            hit.GetComponent<EnemyBase>()?.TakeDamage(damage);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        if (!attackOrigin) return;

        Gizmos.color = new Color(1, 0, 0, 0.4f);
        Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }
}
