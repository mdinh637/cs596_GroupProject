using UnityEngine;
using UnityEngine.EventSystems;

/// Manages the bottom-center troop deployment panel.
/// Tracks which troop card is selected and handles click-to-place.
///
/// When a card is selected, tells TroopPlacer which placement zone to
/// activate based on the card's TroopType (Heavy/Default/Ranged), so
/// the mouse cursor only registers valid hits in the correct zone.
///
/// Uses TroopWaypointInjector to pass scene waypoints into spawned troops
/// at runtime, keeping Troop.cs completely untouched.
public class TroopDeploymentPanel : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // References
    // -------------------------------------------------------------------------
    [Header("References")]
    [SerializeField] private TroopPlacer troopPlacer;           //placement validator and zone switcher

    [Header("Waypoints")]
    [SerializeField] private Transform[] allyWaypoints;         //lane waypoints from player side toward enemy base

    [Header("Troop Container")]
    [SerializeField] private Transform troopContainer;            //optional parent for spawned troops, keeps Hierarchy tidy

    //internal state
    private GameObject selectedPrefab = null;
    private TroopCard selectedCard = null;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Start()
    {
        //keeps the placement preview hidden until one of the troop buttons is selected
        if (troopPlacer != null)
            troopPlacer.DisablePlacement();
    }

    private void Update()
    {
        if (selectedPrefab == null)
            return;

        //cancel on right click
        if (Input.GetMouseButtonDown(1))
        {
            CancelSelection();
            return;
        }

        //place on left click — skip if clicking UI
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            TryPlaceTroop();
        }
    }

    // -------------------------------------------------------------------------
    // Selection — called by TroopCard
    // -------------------------------------------------------------------------

    /// Called by TroopCard when clicked. Registers the selected prefab and card,
    /// then tells TroopPlacer which zone layer to raycast against for this troop type.
    public void SelectTroop(GameObject prefab, TroopCard card)
    {
        //clicking the same card again cancels the selection
        if (selectedPrefab == prefab)
        {
            CancelSelection();
            return;
        }

        selectedPrefab = prefab;
        selectedCard = card;

        //tell TroopPlacer which zone to restrict placement to for this troop type
        if (troopPlacer != null)
        {
            troopPlacer.SetActiveZone(card.GetPlacementZone());
            troopPlacer.EnablePlacement(); //turns placement preview on only after selecting a troop button
        }
    }

    public void CancelSelection()
    {
        selectedPrefab = null;
        selectedCard = null;

        //hides the placement preview after deselecting or placing a troop
        if (troopPlacer != null)
            troopPlacer.DisablePlacement();
    }

    // -------------------------------------------------------------------------
    // Placement
    // -------------------------------------------------------------------------

    private void TryPlaceTroop()
    {
        if (troopPlacer == null || !troopPlacer.CanPlaceHere())
            return;

        CurrencyManager currencyManager = FindFirstObjectByType<CurrencyManager>();

        //this check ensures we're only spending our currency once confirmed the placement is valid
        if (currencyManager == null || selectedCard == null)
            return;

        if (!currencyManager.TrySpend(selectedCard.GetCost()))
            return;

        Vector3 placementPosition = troopPlacer.GetPlacementPosition();
        GameObject spawnedTroop = Instantiate(selectedPrefab, placementPosition, Quaternion.identity);

        if (troopContainer != null)
            spawnedTroop.transform.SetParent(troopContainer);

        //inject scene waypoints at runtime using TroopWaypointInjector
        //this bypasses the prefab-to-scene-object limitation without touching Troop.cs
        if (allyWaypoints != null && allyWaypoints.Length > 0)
        {
            TroopWaypointInjector injector = spawnedTroop.GetComponent<TroopWaypointInjector>();
            if (injector == null)
                injector = spawnedTroop.AddComponent<TroopWaypointInjector>();

            injector.Inject(allyWaypoints);
        }
        else
        {
            Debug.LogWarning("TroopDeploymentPanel: no ally waypoints assigned — spawned troop will not follow the lane.");
        }

        selectedCard?.NotifyPlaced();
        CancelSelection();
    }
}