using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator enemyAnimator;
    private EnemyAI enemyAI;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        enemyAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (enemyAnimator == null) return;

        enemyAnimator.SetFloat("MoveSpeed", enemyAI.MoveSpeed);
    }
}
