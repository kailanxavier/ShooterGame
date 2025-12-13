using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Reference settings: ")]
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI gold;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public void UpdateHealthUI(int currentHealth)
    {
        health.text = $"HEALTH: {currentHealth}";
    }

    public void UpdateGoldUI(int currentGold)
    {
        health.text = $"GOLD: {currentGold}";
    }
}
