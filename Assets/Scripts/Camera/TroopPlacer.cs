using UnityEngine;
using UnityEngine.EventSystems;

public class TroopPlacer : MonoBehaviour
{
    [Header("Placement")]
    [SerializeField] private LayerMask placementLayer; //layer for the valid placement area
    [SerializeField] private GameObject previewObject; //ghost object following cursor
    [SerializeField] private float placementYOffset = 0.5f; //small height offset so preview does not clip into ground

    private bool canPlace = false; //tracks if current mouse position is valid
    private Vector3 currentPlacementPosition; //current valid world position for placement

    void Update()
    {
        //do not place when clicking UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        //if a preview object exists, move it with mouse
        if (previewObject != null)
        {
            UpdatePreviewPosition();
        }

        //left click placement can be added later once troop prefabs are ready
    }

    //update preview object position based on mouse
    private void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //ray from camera through mouse position

        //raycast only against valid placement layer
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayer, QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = hit.point;
            pos.y += placementYOffset;
            previewObject.transform.position = pos;

            currentPlacementPosition = pos;
            canPlace = true;
        }
        else
        {
            canPlace = false;
        }
    }

    //returns if the current mouse position is valid for placement
    public bool CanPlaceHere()
    {
        return canPlace;
    }

    //returns the current placement position
    public Vector3 GetPlacementPosition()
    {
        return currentPlacementPosition;
    }
}
