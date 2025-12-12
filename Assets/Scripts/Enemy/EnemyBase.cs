using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private int health;
    [SerializeField] private EnemyData data;
    [SerializeField] private AudioClip death;
    [SerializeField, Range(0.0f, 1.0f)] private float deathVolume;

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

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        SoundManager.Instance.PlaySound(death, transform.position, deathVolume);
        Destroy(gameObject);
        // ui stuff here
    }
}
