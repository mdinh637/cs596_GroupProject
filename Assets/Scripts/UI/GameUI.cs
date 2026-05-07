using UnityEngine;

/// Master UI manager for Battle Knights.
/// Holds references to all major UI systems and acts as the
/// single point of contact for game-state changes that affect the UI.
///
/// Attach to a persistent manager GameObject in the scene.
/// Wire up all references in the Inspector.
public class GameUI : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Singleton
    // -------------------------------------------------------------------------
    public static GameUI Instance { get; private set; }

    // -------------------------------------------------------------------------
    // UI system references
    // -------------------------------------------------------------------------
    [Header("UI Systems")]
    [SerializeField] private CurrencyManager currencyManager;
    [SerializeField] private TroopDeploymentPanel deploymentPanel;
    [SerializeField] private WinLoseScreen winLoseScreen;

    [Header("Base Health Bars")]
    [SerializeField] private WorldSpaceHealthBar playerBaseHealthBar;   // floats above the player's base
    [SerializeField] private WorldSpaceHealthBar enemyBaseHealthBar;    // floats above the enemy tower

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // -------------------------------------------------------------------------
    // Public API — call these from gameplay scripts
    // -------------------------------------------------------------------------

    /// Call from allied unit or projectile when the enemy base takes a hit.
    public void DamageEnemyBase(float amount)
    {
        if (enemyBaseHealthBar == null) return;

        enemyBaseHealthBar.TakeDamage(amount);

        if (enemyBaseHealthBar.IsDead())
            winLoseScreen?.ShowWin();
    }

    /// Call from enemy unit when the player base takes a hit.
    public void DamagePlayerBase(float amount)
    {
        if (playerBaseHealthBar == null) return;

        playerBaseHealthBar.TakeDamage(amount);

        if (playerBaseHealthBar.IsDead())
            winLoseScreen?.ShowLose();
    }

    /// Adds currency — call when an enemy unit is defeated.
    public void AwardCurrency(float amount)
    {
        currencyManager?.AddCurrency(amount);
    }

    // -------------------------------------------------------------------------
    // Accessors
    // -------------------------------------------------------------------------

    public CurrencyManager GetCurrencyManager() => currencyManager;
    public WorldSpaceHealthBar GetPlayerHealthBar() => playerBaseHealthBar;
    public WorldSpaceHealthBar GetEnemyHealthBar() => enemyBaseHealthBar;
}
