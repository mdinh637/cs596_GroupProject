using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// Represents a single troop card in the bottom deployment panel.
/// Handles cost display, cooldown fill overlay, and affordability greying.
/// Attach to each card GameObject inside the deployment panel.
public class TroopCard : MonoBehaviour
{
    public enum TroopType { Default, Heavy, Ranged }

    // -------------------------------------------------------------------------
    // Card data
    // -------------------------------------------------------------------------
    [Header("Troop Data")]
    [SerializeField] private GameObject troopPrefab;        //the troop prefab this card deploys
    [SerializeField] private TroopType troopType = TroopType.Default; //drives which placement zone is used
    [SerializeField] private float troopCost = 3f;          //currency cost to deploy
    [SerializeField] private float deployCooldown = 2f;     //seconds before this card can be used again
    [SerializeField] private Sprite troopIcon;              //icon shown on the card
    [SerializeField] private string troopName = "";         //display name shown on the card label

    [Header("UI References")]
    [SerializeField] private Image iconImage;               //main card icon
    [SerializeField] private Image cooldownOverlay;         //dark fill overlay draining down during cooldown
    [SerializeField] private TMP_Text costText;             //cost label at the bottom of the card
    [SerializeField] private Button cardButton;             //the clickable button

    //internal state
    private float cooldownTimer = 0f;
    private bool onCooldown = false;
    private CurrencyManager currencyManager;
    private TroopDeploymentPanel deploymentPanel;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Awake()
    {
        currencyManager = FindFirstObjectByType<CurrencyManager>();
        deploymentPanel = FindFirstObjectByType<TroopDeploymentPanel>();

        if (iconImage != null && troopIcon != null)
            iconImage.sprite = troopIcon;

        // show name and cost together if a name is provided, otherwise just the cost number
        if (costText != null)
        {
            if (!string.IsNullOrEmpty(troopName))
                costText.text = troopName + "\n" + troopCost.ToString("0") + "g";
            else
                costText.text = troopCost.ToString("0") + "g";
        }

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 0f;

        if (cardButton != null)
            cardButton.onClick.AddListener(OnCardClicked);
    }

    private void Update()
    {
        HandleCooldown();
        UpdateAffordability();
    }

    // -------------------------------------------------------------------------
    // Card interaction
    // -------------------------------------------------------------------------

    private void OnCardClicked()
    {
        if (onCooldown) return;
        if (currencyManager == null) return;

        if (!currencyManager.TrySpend(troopCost)) return;

        deploymentPanel?.SelectTroop(troopPrefab, this);
    }

    /// Returns which placement zone this card's troop type maps to.
    public TroopPlacer.PlacementZone GetPlacementZone()
    {
        return troopType switch
        {
            TroopType.Heavy => TroopPlacer.PlacementZone.Front,
            TroopType.Ranged => TroopPlacer.PlacementZone.Back,
            _ => TroopPlacer.PlacementZone.Middle,
        };
    }

    /// Called by TroopDeploymentPanel after a troop is successfully placed.
    public void NotifyPlaced()
    {
        onCooldown = true;
        cooldownTimer = deployCooldown;
    }

    // -------------------------------------------------------------------------
    // Cooldown
    // -------------------------------------------------------------------------

    private void HandleCooldown()
    {
        if (!onCooldown) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = Mathf.Clamp01(cooldownTimer / deployCooldown);

        if (cooldownTimer <= 0f)
        {
            onCooldown = false;
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = 0f;
        }
    }

    // -------------------------------------------------------------------------
    // Affordability greying
    // -------------------------------------------------------------------------

    private void UpdateAffordability()
    {
        if (cardButton == null || currencyManager == null) return;

        bool canAfford = currencyManager.GetCurrentCurrency() >= troopCost && !onCooldown;
        cardButton.interactable = canAfford;

        if (iconImage != null)
            iconImage.color = canAfford ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    // -------------------------------------------------------------------------
    // Public accessors
    // -------------------------------------------------------------------------

    public GameObject GetTroopPrefab() => troopPrefab;
    public float GetCost() => troopCost;
    public bool IsOnCooldown() => onCooldown;
}