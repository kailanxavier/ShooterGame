using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy/Enemy Type")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHealth = 100;
    public float moveSpeed = 3f;

    public EnemyWeaponData weapon;

    public GameObject enemyPrefab;
}
