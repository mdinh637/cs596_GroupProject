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

    [Header("Placement Indicator")]
    [SerializeField] private GameObject placementIndicatorPrefab; //flat disc shown at cursor while placing

    [Header("Troop Container")]
    [SerializeField] private Transform troopContainer;            //optional parent for spawned troops, keeps Hierarchy tidy

    //internal state
    private GameObject selectedPrefab = null;
    private TroopCard selectedCard = null;
    private GameObject activeIndicator = null;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Update()
    {
        if (selectedPrefab == null)
            return;

        UpdateIndicator();

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
    // Placement indicator
    // -------------------------------------------------------------------------

    private void UpdateIndicator()
    {
        if (placementIndicatorPrefab == null || troopPlacer == null)
            return;

        if (activeIndicator == null)
            activeIndicator = Instantiate(placementIndicatorPrefab);

        if (troopPlacer.CanPlaceHere())
        {
            activeIndicator.SetActive(true);
            activeIndicator.transform.position = troopPlacer.GetPlacementPosition();
        }
        else
        {
            activeIndicator.SetActive(false);
        }
    }

    private void DestroyIndicator()
    {
        if (activeIndicator != null)
        {
            Destroy(activeIndicator);
            activeIndicator = null;
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
            troopPlacer.SetActiveZone(card.GetPlacementZone());
    }

    public void CancelSelection()
    {
        selectedPrefab = null;
        selectedCard = null;
        DestroyIndicator();
    }

    // -------------------------------------------------------------------------
    // Placement
    // -------------------------------------------------------------------------

    private void TryPlaceTroop()
    {
        if (troopPlacer == null || !troopPlacer.CanPlaceHere())
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