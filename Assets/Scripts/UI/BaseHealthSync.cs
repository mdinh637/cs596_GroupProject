using UnityEngine;

/// Syncs a BaseTower's current health to a WorldSpaceHealthBar every frame.
/// Attach to the same GameObject as BaseTower, or wire both references manually.
/// This bridges the gap between Troop's internal health tracking and the UI bar
/// without modifying Troop.cs.
public class BaseHealthSync : MonoBehaviour
{
    [SerializeField] private Troop troop;                       // the BaseTower (which is a Troop)
    [SerializeField] private WorldSpaceHealthBar healthBar;     // the world space health bar above this base

    private float cachedHealth = -1f;

    private void Start()
    {
        if (troop == null)
            troop = GetComponent<Troop>();

        // initialise the bar to full health on start
        if (troop != null && healthBar != null)
            healthBar.SetHealth(troop.GetCurrentHealth(), troop.GetMaxHealth());
    }

    private void Update()
    {
        if (troop == null || healthBar == null)
            return;

        float current = troop.GetCurrentHealth();

        // only update UI when health actually changes, avoids unnecessary redraws
        if (current == cachedHealth)
            return;

        cachedHealth = current;
        healthBar.SetHealth(current, troop.GetMaxHealth());
    }
}
