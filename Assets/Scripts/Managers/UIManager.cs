using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Reference settings: ")]
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI gold;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI kills;

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

    private void Update()
    {
        if (GameManager.Instance.IsPlaying)
            timer.text = GameManager.Instance.GetFormattedTime();
    }

    public void UpdateHealthUI(int currentHealth)
    {
        health.text = $"HEALTH: {currentHealth}";
    }

    public void UpdateKillsUI(int currentKills)
    {
        kills.text = $"KILLS: {currentKills}";
    }

    public void UpdateGoldUI(int currentGold)
    {
        gold.text = $"GOLD: {currentGold}";
    }
}
