using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles mouse-based troop placement preview and position validation.
/// Three placement zones restrict where each troop category can be placed:
///   Front zone  — Heavy troops (TankyKnight, Barbarian) — closest to enemy
///   Middle zone — Default troops (BasicKnight, Rogue)
///   Back zone   — Ranged troops (Archer) — furthest from enemy, near player castle
///
/// TroopDeploymentPanel calls SetActiveZone() when a card is selected so
/// the correct zone layer is raycasted against for that troop type.
/// </summary>
public class TroopPlacer : MonoBehaviour
{
    public enum PlacementZone { Front, Middle, Back }

    [Header("Placement Zones")]
    [SerializeField] private LayerMask frontZoneLayer;  //heavy troops — closest to enemy
    [SerializeField] private LayerMask middleZoneLayer; //default troops — center of deployment area
    [SerializeField] private LayerMask backZoneLayer;   //ranged troops — furthest from enemy

    [Header("Preview")]
    [SerializeField] private GameObject previewObject;      //ghost object following cursor
    [SerializeField] private float placementYOffset = 0.5f; //small height offset so preview does not clip into ground

    private PlacementZone activeZone = PlacementZone.Middle;
    private bool canPlace = false;
    private Vector3 currentPlacementPosition;

    private void Update()
    {
        //do not place when clicking UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            canPlace = false;
            if (previewObject != null) previewObject.SetActive(false);
            return;
        }

        if (previewObject != null)
            UpdatePreviewPosition();
    }

    /// Called by TroopDeploymentPanel when a card is selected.
    /// Switches which zone layer is raycasted against.
    public void SetActiveZone(PlacementZone zone)
    {
        activeZone = zone;
    }

    private LayerMask GetActiveLayer()
    {
        return activeZone switch
        {
            PlacementZone.Front => frontZoneLayer,
            PlacementZone.Back => backZoneLayer,
            _ => middleZoneLayer,
        };
    }

    private void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //ray from camera through mouse position

        //raycast only against the active zone layer
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, GetActiveLayer(), QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = hit.point;
            pos.y += placementYOffset;
            previewObject.transform.position = pos;
            previewObject.SetActive(true);

            currentPlacementPosition = pos;
            canPlace = true;
        }
        else
        {
            previewObject.SetActive(false);
            canPlace = false;
        }
    }

    //returns if the current mouse position is valid for placement
    public bool CanPlaceHere()
    {
        return canPlace;
    }

    //returns the current valid world placement position
    public Vector3 GetPlacementPosition()
    {
        return currentPlacementPosition;
    }
}