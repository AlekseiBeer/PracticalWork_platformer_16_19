using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        UIManager.Instance.ShowWindow("PauseMenu");
    }

    private void ResumeGame()
    {
        UIManager.Instance.HideWindow("PauseMenu");
        Time.timeScale = 1f;
        isPaused = false;
    }
}
