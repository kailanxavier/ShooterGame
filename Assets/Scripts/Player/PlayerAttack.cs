using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private LayerMask attackableMask;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private int damage = 35;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private Transform camRef;

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
        canAttack = false;

        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hitInfo, attackRange, attackableMask))
        {
            if (hitInfo.collider.CompareTag(enemyTag))
            {
                EnemyBase enemy = hitInfo.collider.gameObject.GetComponent<EnemyBase>();
                if (enemy)
                {
                    enemy.TakeDamage(damage);
                }
            }
            // maybe other destructibles will go here soon
        }
        // else we didnt hit anything
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Debug.DrawLine(ray.origin, ray.direction * attackRange, Color.red);
    }
}
