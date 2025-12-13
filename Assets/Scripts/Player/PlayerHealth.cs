using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    private void Awake()
    {
        GameManager.Instance.SetCurrentHealth(currentHealth);
        currentHealth = maxHealth;
    }

    public void TakeDamage(int value)
    {
        GameManager.Instance.SetCurrentHealth(currentHealth);
        currentHealth -= value;
    }
}
