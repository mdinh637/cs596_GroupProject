using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Handles the win and lose screen overlay shown when the game ends.
/// Call ShowWin() or ShowLose() from whichever script detects a base being destroyed.
/// Attach to the root of a full-screen Canvas set to Screen Space - Overlay.
/// Keep the GameObject inactive by default — this script activates it when needed.
/// </summary>
public class WinLoseScreen : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // UI References
    // -------------------------------------------------------------------------
    [Header("UI References")]
    [SerializeField] private GameObject winPanel;           // shown on victory
    [SerializeField] private GameObject losePanel;          // shown on defeat
    [SerializeField] private TMP_Text resultText;           // large result label ("Victory!" / "Defeat!")
    [SerializeField] private Button restartButton;          // restarts the current scene
    [SerializeField] private Button mainMenuButton;         // loads the main menu scene

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Awake()
    {
        // make sure both panels start hidden
        if (winPanel != null)  winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        gameObject.SetActive(false); // entire overlay starts hidden

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartScene);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    // -------------------------------------------------------------------------
    // Public API — call these from base destruction logic
    // -------------------------------------------------------------------------

    /// <summary>
    /// Call this when the enemy base is destroyed (player wins).
    /// </summary>
    public void ShowWin()
    {
        gameObject.SetActive(true);

        if (winPanel != null)  winPanel.SetActive(true);
        if (losePanel != null) losePanel.SetActive(false);

        if (resultText != null) resultText.text = "Victory!";

        PauseGame();
    }

    /// <summary>
    /// Call this when the player's base is destroyed (player loses).
    /// </summary>
    public void ShowLose()
    {
        gameObject.SetActive(true);

        if (losePanel != null) losePanel.SetActive(true);
        if (winPanel != null)  winPanel.SetActive(false);

        if (resultText != null) resultText.text = "Defeat!";

        PauseGame();
    }

    // -------------------------------------------------------------------------
    // Button handlers
    // -------------------------------------------------------------------------

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private void PauseGame()
    {
        Time.timeScale = 0f; // freeze the game while the result screen is shown
    }
}
