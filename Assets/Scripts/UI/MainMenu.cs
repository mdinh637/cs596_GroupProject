using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// Main menu controller.
/// Attach to a GameObject in the main menu scene.
/// Wire up the buttons in the Inspector.
/// Make sure both scenes are added to File -> Build Settings.
public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "MainLevel";
    [SerializeField] private string secondarySceneName = "Barracks";

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button secondaryButton;

    [Header("UI (optional)")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text versionText;

    private void Awake()
    {
        // make sure time is running normally in the main menu
        Time.timeScale = 1f;

        if (playButton != null)
            playButton.onClick.AddListener(StartGame);

        if (secondaryButton != null)
            secondaryButton.onClick.AddListener(LoadSecondaryScene);

        if (versionText != null)
            versionText.text = "v" + Application.version;
    }

    public void StartGame()
    {
        // set flag so GameStarter knows to unpause immediately
        GameStarter.startedFromMenu = true;
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadSecondaryScene()
    {
        // load the other scene (e.g., Barracks)
        SceneManager.LoadScene(secondarySceneName);
    }
}