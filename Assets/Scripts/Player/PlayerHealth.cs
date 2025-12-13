using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    public int MaxHealth => maxHealth;

    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance.SetCurrentHealth(currentHealth);
    }

    public void TakeDamage(int value)
    {
        GameManager.Instance.SetCurrentHealth(currentHealth);
        currentHealth -= value;

        if (currentHealth <= 0) GameManager.Instance.EndGame();
    }
}
