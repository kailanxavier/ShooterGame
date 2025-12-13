using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentPlayerHealth;

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

    public void SetCurrentHealth(int val)
    {
        currentPlayerHealth = val;
        UIManager.Instance.UpdateHealthUI(currentPlayerHealth);
    }

    public void EndGame()
    {
        SceneManager.LoadSceneAsync((int)Scenes.GameOver);
        Time.timeScale = 0.0f;
    }

    private enum Scenes
    {
        MainMenu,
        MainGame,
        GameOver
    }
}
