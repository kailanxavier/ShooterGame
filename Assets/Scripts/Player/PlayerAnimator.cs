using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    private Animator playerAnimator;
    [SerializeField] private string attackBool = "IsAttacking";

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
    }

    public void AnimateAttack()
    {
        StartCoroutine(nameof(AttackBoolCoroutine));
    }

    private IEnumerator AttackBoolCoroutine()
    {
        // not hacky wink wink
        playerAnimator.SetBool(attackBool, true);
        yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        playerAnimator.SetBool(attackBool, false);
    }
}
