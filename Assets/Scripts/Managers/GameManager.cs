using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentPlayerHealth;
    private int currentPlayerGold = 0;
    private int killCount = 0;

    private float timer;
    private bool isPlaying = true;

    public bool IsPlaying => isPlaying;

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
        if (!isPlaying) return;

        timer += Time.deltaTime;
    }

    public string GetFormattedTime()
    {
        TimeSpan t = TimeSpan.FromSeconds(timer);
        return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}", t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
    }

    public void IncrementKill()
    {
        killCount++;
        UIManager.Instance.UpdateKillsUI(killCount);
    }

    public void SetCurrentHealth(int val)
    {
        currentPlayerHealth = val;
        UIManager.Instance.UpdateHealthUI(currentPlayerHealth);
    }

    public void AddPlayerGold(int val)
    {
        currentPlayerGold += val;
        UIManager.Instance.UpdateGoldUI(currentPlayerGold);
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPlaying = false;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPlaying = true;
    }

    public void EndGame()
    {
        SceneManager.LoadSceneAsync((int)Scenes.GameOver);
        Time.timeScale = 0.0f;
        isPlaying = false;
    }

    private enum Scenes
    {
        MainGame,
        GameOver
    }
}
