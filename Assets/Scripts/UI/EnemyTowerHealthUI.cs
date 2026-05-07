using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Screen-space health bar for the enemy tower.
/// Attach this to a Canvas GameObject set to Screen Space - Overlay.
/// Wire up the Slider (and optional Text label) in the Inspector.
/// The bar is hidden by default and only appears when the tower takes damage,
/// then fades out automatically after a short delay.
/// </summary>
public class EnemyTowerHealthUI : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Inspector references
    // -------------------------------------------------------------------------
    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;           // fill slider showing remaining health
    [SerializeField] private Text healthText;               // optional — displays "350 / 500" style label

    [Header("Auto-hide")]
    [SerializeField] private float hideDelay = 3f;          // seconds of inactivity before the bar disappears
    private float hideTimer;
    private bool isVisible;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Update()
    {
        if (!isVisible)
            return;

        // count down and hide once the delay expires
        hideTimer -= Time.deltaTime;
        if (hideTimer <= 0f)
            Hide();
    }

    // -------------------------------------------------------------------------
    // Public API — called by EnemyTower
    // -------------------------------------------------------------------------

    /// <summary>
    /// Reveals the health bar and refreshes it to reflect the current health value.
    /// Resets the auto-hide countdown each time this is called.
    /// </summary>
    public void ShowAndUpdate(float current, float max)
    {
        gameObject.SetActive(true);
        isVisible  = true;
        hideTimer  = hideDelay;

        // update the slider fill (normalised 0–1)
        if (healthSlider != null)
            healthSlider.value = Mathf.Clamp01(current / max);

        // update the optional text label
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    /// <summary>
    /// Hides the health bar immediately.
    /// </summary>
    public void Hide()
    {
        isVisible = false;
        gameObject.SetActive(false);
    }
}
