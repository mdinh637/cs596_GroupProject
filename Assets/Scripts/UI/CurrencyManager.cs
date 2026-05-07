using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// Manages the player's currency.
/// Passively generates currency over time and handles spending.
/// Attach to a persistent manager GameObject in the scene.
public class CurrencyManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Currency settings
    // -------------------------------------------------------------------------
    [Header("Currency Settings")]
    [SerializeField] private float startingCurrency = 10f;
    [SerializeField] private float passiveGenerationRate = 2f;  // currency per second
    [SerializeField] private float maxCurrency = 100f;

    [Header("UI")]
    [SerializeField] private TMP_Text currencyText;             // top center currency label

    private float currentCurrency;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Start()
    {
        currentCurrency = startingCurrency;
        UpdateUI();
    }

    private void Update()
    {
        // passive generation
        if (currentCurrency < maxCurrency)
        {
            currentCurrency = Mathf.Min(currentCurrency + passiveGenerationRate * Time.deltaTime, maxCurrency);
            UpdateUI();
        }
    }

    // -------------------------------------------------------------------------
    // Public API
    // -------------------------------------------------------------------------


    /// Returns true and deducts cost if the player can afford it.
    public bool TrySpend(float cost)
    {
        if (currentCurrency < cost)
            return false;

        currentCurrency -= cost;
        UpdateUI();
        return true;
    }

    /// Adds currency — call this when an enemy unit is defeated.
    public void AddCurrency(float amount)
    {
        currentCurrency = Mathf.Min(currentCurrency + amount, maxCurrency);
        UpdateUI();
    }

    public float GetCurrentCurrency() => currentCurrency;
    public float GetMaxCurrency() => maxCurrency;

    // -------------------------------------------------------------------------
    // UI
    // -------------------------------------------------------------------------

    private void UpdateUI()
    {
        if (currencyText != null)
            currencyText.text = $"{Mathf.FloorToInt(currentCurrency)} / {Mathf.FloorToInt(maxCurrency)}";
    }
}
