using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack settings: ")]
    [SerializeField] private LayerMask attackableMask;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private int damage = 35;
    [SerializeField] private bool canAttack = true;

    [Header("Hit detection: ")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float spreadAngle = 45.0f;
    [SerializeField] private int boxCount = 5;
    [SerializeField] private Vector3 boxSize = new(0.4f, 0.4f, 0.4f);

    [Header("Visuals: ")]
    [SerializeField] private GameObject bloodPrefab;

    [SerializeField] private Transform attackOrigin;

    [Header("Audio clips: ")]
    [SerializeField] private AudioClip swingClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField, Range(0.0f, 1.0f)] private float swingVolume = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float hitVolume = 1.0f;

    private PlayerAnimator playerAnimator;

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        if (InputManager.Instance.Attack && canAttack)
        {
            canAttack = false;

            StartCoroutine(Attack(0.2f));
            playerAnimator.AnimateAttack();

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private IEnumerator Attack(float delay)
    {
        yield return new WaitForSeconds(delay);

        SoundManager.Instance.PlaySound(swingClip, transform.position, swingVolume);

        HashSet<Collider> hitAlready = new();

        float halfAngle = spreadAngle / 2f;
        Transform cam = Camera.main.transform;

        for (int i = 0; i < boxCount; i++)
        {
            float t = (boxCount == 1) ? 0.5f : (float)i / (boxCount - 1);

            float angle = Mathf.Lerp(halfAngle, -halfAngle, t);

            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 dir = rot * cam.forward;

            Vector3 spawnPos = attackOrigin.position + dir.normalized * attackRange;

            Debug.DrawRay(attackOrigin.position, dir * attackRange, Color.red, 0.3f);

            Collider[] hits = Physics.OverlapBox(
                spawnPos, boxSize * 0.5f,
                Quaternion.LookRotation(dir),
                attackableMask
            );

            foreach (Collider hit in hits)
            {
                if (hitAlready.Contains(hit)) continue;

                hitAlready.Add(hit);

                GameObject instance = Instantiate(bloodPrefab, spawnPos, Quaternion.identity);
                Destroy(instance, 1f);

                hit.GetComponent<EnemyBase>()?.TakeDamage(damage); // i will use null propagation as much as i want unity
                SoundManager.Instance.PlaySound(hitClip, hit.transform.position, hitVolume);
            }

            yield return null;
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (attackOrigin == null) return;

    //    Gizmos.color = Color.yellow;

    //    float halfAngle = spreadAngle / 2f;
    //    Transform cam = Camera.main != null ? Camera.main.transform : this.transform;

    //    for (int i = 0; i < boxCount; i++)
    //    {
    //        float t = (boxCount == 1) ? 0.5f : (float)i / (boxCount - 1);
    //        float angle = Mathf.Lerp(halfAngle, -halfAngle, t);

    //        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
    //        Vector3 dir = rot * cam.forward;

    //        Vector3 spawnPos = attackOrigin.position + dir.normalized * attackRange;

    //        Gizmos.matrix = Matrix4x4.TRS(spawnPos, Quaternion.LookRotation(dir), Vector3.one);
    //        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    //    }
    //}
}
