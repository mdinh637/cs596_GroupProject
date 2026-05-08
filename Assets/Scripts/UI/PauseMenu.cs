using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// Pause menu controller.
/// Attach to an always-active GameObject (HUD Canvas or GameManager).
/// Keep PausePanel inactive by default.
/// Press Escape or call TogglePause() to toggle pause.
public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;     // the pause menu panel (contains Resume/MainMenu/Quit)
    [SerializeField] private Button resumeButton;       // resumes the game
    [SerializeField] private Button mainMenuButton;     // returns to main menu
    [SerializeField] private Button quitButton;         // quits the application

    [Header("Top-right Play/Pause Button (optional)")]
    [Tooltip("Assign the small Pause HUD button so it can be hidden while paused.")]
    [SerializeField] private GameObject playPauseButton;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "BetterMainMenu";

    private bool isPaused = false;

    private void Awake()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // make sure panel starts hidden
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // make sure the HUD play/pause button is visible at start (if assigned)
        if (playPauseButton != null)
            playPauseButton.SetActive(true);

        //make sure audio is not stuck paused when entering the scene
        AudioListener.pause = false;
    }

    private void Update()
    {
        // only allow pausing if game is actually running
        if (Time.timeScale == 0f && !isPaused)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    // Public method for Button.OnClick to call
    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true; //pause all game audio while pause menu is open

        if (pausePanel != null)
            pausePanel.SetActive(true);
        else
            Debug.LogWarning("PauseMenu: pausePanel not assigned.");

        if (playPauseButton != null)
            playPauseButton.SetActive(false);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false; //resume game audio when unpausing

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (playPauseButton != null)
            playPauseButton.SetActive(true);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false; //reset audio before changing scenes
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void QuitGame()
    {
        AudioListener.pause = false; //reset audio before quitting play mode

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}