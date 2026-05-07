using UnityEngine;

/// Watches whether the player base and enemy base GameObjects still exist.
/// When either is destroyed, triggers the win or lose screen.
/// Attach to GameManager. Wire both base GameObjects in the Inspector.
public class GameEndChecker : MonoBehaviour
{
    [Header("Base References")]
    [SerializeField] private GameObject playerBase;  // the 'tower' object
    [SerializeField] private GameObject enemyBase;   // the 'fortress' object

    [Header("References")]
    [SerializeField] private WinLoseScreen winLoseScreen;

    private bool gameEnded = false;

    private void Update()
    {
        if (gameEnded)
            return;

        if (playerBase == null)
        {
            gameEnded = true;
            winLoseScreen.gameObject.SetActive(true);
            winLoseScreen?.ShowLose();
        }

        if (enemyBase == null)
        {
            gameEnded = true;
            winLoseScreen.gameObject.SetActive(true);
            winLoseScreen?.ShowWin();
        }
    }
}