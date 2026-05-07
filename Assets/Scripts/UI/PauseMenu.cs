using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// Pause menu controller.
/// Attach to a PausePanel GameObject inside HUDCanvas.
/// Keep PausePanel inactive by default.
/// Press Escape to toggle pause during gameplay.
public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;     //the pause menu panel
    [SerializeField] private Button resumeButton;       //resumes the game
    [SerializeField] private Button mainMenuButton;     //returns to main menu
    [SerializeField] private Button quitButton;         //quits the application

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Awake()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        //make sure panel starts hidden
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        //only allow pausing if game is actually running
        if (Time.timeScale == 0f && !isPaused)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    private void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}