using UnityEngine;
using UnityEngine.SceneManagement;

/// Attach to GameManager in TowerTestingScene.
/// Freezes the game on start if loaded directly (e.g. from Unity editor).
/// If loaded from the main menu, starts immediately.
/// 
/// Also handles pausing Time.timeScale properly on win/lose
/// so that restarting from WinLoseScreen works correctly.
public class GameStarter : MonoBehaviour
{
    //static flag set by MainMenu when Play is clicked
    //persists between scene loads because it is static
    public static bool startedFromMenu = false;

    private void Start()
    {
        if (startedFromMenu)
        {
            //loaded from main menu — start normally
            Time.timeScale = 1f;
            startedFromMenu = false; //reset so next direct play in editor freezes again
        }
        else
        {
            //loaded directly in editor — freeze everything
            Time.timeScale = 0f;
            Debug.Log("GameStarter: game paused — press Play from the main menu to start.");
        }
    }
}