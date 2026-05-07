using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// A world-space health bar that floats above a base (player or enemy).
/// Place this on a World Space Canvas parented to or positioned above the base.
/// The bar always faces the main camera.
public class WorldSpaceHealthBar : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Health data
    // -------------------------------------------------------------------------
    [Header("Health")]
    [SerializeField] private float maxHealth = 500f;
    private float currentHealth;

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;           // fill slider
    [SerializeField] private TMP_Text healthText;           // optional "350 / 500" label
    [SerializeField] private Image fillImage;               // the slider's fill image for color changes

    [Header("Colors")]
    [SerializeField] private Color healthyColor = Color.green;
    [SerializeField] private Color damagedColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;

    [Header("Billboard")]
    [SerializeField] private bool faceCamera = true;        // always rotate to face the camera

    private Camera mainCamera;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        RefreshUI();
    }

    private void LateUpdate()
    {
        // rotate the canvas to always face the camera
        if (faceCamera && mainCamera != null)
            transform.forward = mainCamera.transform.forward;
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------

    /// Deals damage to this base and updates the health bar.
    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        RefreshUI();
    }

    /// Directly sets health — useful for syncing with an external health source.
    public void SetHealth(float current, float max)
    {
        maxHealth = max;
        currentHealth = Mathf.Clamp(current, 0f, max);
        RefreshUI();
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => currentHealth <= 0f;

    // -------------------------------------------------------------------------
    // UI refresh
    // -------------------------------------------------------------------------

    private void RefreshUI()
    {
        float ratio = Mathf.Clamp01(currentHealth / maxHealth);

        if (healthSlider != null)
            healthSlider.value = ratio;

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";

        // shift color based on health percentage
        if (fillImage != null)
        {
            if (ratio > 0.5f)
                fillImage.color = Color.Lerp(damagedColor, healthyColor, (ratio - 0.5f) * 2f);
            else
                fillImage.color = Color.Lerp(criticalColor, damagedColor, ratio * 2f);
        }
    }
}
