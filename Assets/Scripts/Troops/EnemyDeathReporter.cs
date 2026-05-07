using UnityEngine;

/// <summary>
/// Lightweight component added by EnemyTower to each spawned unit at runtime.
/// Detects when the unit's GameObject is destroyed and notifies the tower
/// so it can decrement its active enemy count.
///
/// This avoids any modification to Troop.cs — OnDestroy fires automatically
/// whenever the GameObject is destroyed for any reason (death, cleanup, etc.).
/// </summary>
public class EnemyDeathReporter : MonoBehaviour
{
    private EnemyTower tower;

    public void Initialize(EnemyTower owningTower)
    {
        tower = owningTower;
    }

    private void OnDestroy()
    {
        // only report if the game is still running (not editor teardown)
        if (tower != null && Application.isPlaying)
            tower.OnEnemyUnitDied();
    }
}
