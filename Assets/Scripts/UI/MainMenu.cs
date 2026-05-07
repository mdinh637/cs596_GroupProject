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
    [SerializeField] private string gameSceneName = "TowerTestingScene";

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("UI (optional)")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text versionText;

    private void Awake()
    {
        //make sure time is running normally in the main menu
        Time.timeScale = 1f;

        if (playButton != null)
            playButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (versionText != null)
            versionText.text = "v" + Application.version;
    }

    private void StartGame()
    {
        //set flag so GameStarter knows to unpause immediately
        GameStarter.startedFromMenu = true;
        SceneManager.LoadScene(gameSceneName);
    }

    private void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}