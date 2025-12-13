using UnityEngine;

public class PlayerUIChecks : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;
    private bool pauseMenuActive;

    private void Update()
    {
        if (!InputManager.Instance.PauseMenu)
            return;

        TogglePause();
    }

    private void TogglePause()
    {
        pauseMenuActive = !pauseMenuActive;
        pauseMenuCanvas.SetActive(pauseMenuActive);

        if (pauseMenuActive)
        {
            InputManager.Instance.DisableInput();
            GameManager.Instance.PauseGame();
        }
        else
        {
            InputManager.Instance.EnableInput();
            GameManager.Instance.ResumeGame();
        }
    }
}
