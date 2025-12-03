using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private int health;
    [SerializeField] private EnemyData data;

    private void Awake()
    {
        health = data.maxHealth;
        //EquipWeapon(data.weapon);
    }

    private void EquipWeapon(EnemyWeaponData weapon)
    {
        Debug.Log("Enemy using weapon: " + weapon.weaponName);
        // do all the jazz here
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Current enemy health: {health}");

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
        // ui stuff here
    }
}
