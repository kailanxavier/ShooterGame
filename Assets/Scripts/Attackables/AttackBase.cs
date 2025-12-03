using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    public void BaseAttack()
    {
        Attack();
    }

    protected virtual void Attack()
    {

    }
}
