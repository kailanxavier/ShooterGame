using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Enemy/Weapon")]
public class EnemyWeaponData : ScriptableObject
{
    public string weaponName;
    public float damage = 10f;
    public float fireRate = 0.5f;
    public float range = 20f;

    public GameObject bulletPrefab;
}
