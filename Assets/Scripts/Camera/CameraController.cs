using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool canControl;
    [SerializeField] private Vector3 levelCenterPoint;  //Click on center of level to find coordinates
    [SerializeField] private float maxDistanceFromCenter;

    [Header("Movement Details")]
    [SerializeField] private float movementSpeed = 120;
    [SerializeField] private float mouseMovementSpeed = 5;
    [SerializeField] private float edgeMovementSpeed = 50;
    [SerializeField] private float edgeTreshold = 10;
    private float screenWidth;
    private float screenHeight;

    [Header("Rotation Details")]
    [SerializeField] private Transform focusPoint;
    [SerializeField] private float maxFocusPointDistance = 15;
    [SerializeField] private float rotationSpeed = 200;
    [Space]
    private float pitch;
    [SerializeField] private float minPitch = 5;
    [SerializeField] private float maxPitch = 85;

    [Header("Zoom Details")]
    [SerializeField] private float zoomSpeed = 35;
    [SerializeField] private float minZoom = 3;
    [SerializeField] private float maxZoom = 15;


    private float smoothTime = 0.1f;
    private Vector3 movementVelocity = Vector3.zero;
    private Vector3 mouseMovementVelocity = Vector3.zero;
    private Vector3 edgeMovementVelocity = Vector3.zero;
    private Vector3 zoomVelocity = Vector3.zero;
    private Vector3 lastMousePosition;

    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    void Update()
    {
        if(canControl == false)
            return;
        HandleRotation();
        HandleZoom();
        HandleEdgeMovement();
        HandleMouseMovement();
        HandleMovement();

        focusPoint.position = transform.position + (transform.forward * GetFocusPointDistance());
    }

    public void EnableCameraControls(bool enable) => canControl = enable;
    public float AdjustPitchValue(float value) => pitch = value;

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomDirection = transform.forward * scroll * zoomSpeed;
        Vector3 targetPosition = transform.position + zoomDirection;

        if(transform.position.y < minZoom && scroll > 0)    //Stop camera from zooming in too close / too far
            return;

        if(transform.position.y > maxZoom && scroll < 0)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref zoomVelocity, smoothTime);
    }

    private float GetFocusPointDistance()   //Calculate and handle object collisions and interferance (using raycasting)
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxFocusPointDistance))
            return hit.distance;

        return maxFocusPointDistance;
    }

    private void HandleRotation()
    {
        if(Input.GetMouseButton(1)) //RMB for rotations
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            
            pitch = Mathf.Clamp(pitch - verticalRotation, minPitch, maxPitch);  //Pitch limitations for camera angles

            transform.RotateAround(focusPoint.position, Vector3.up, horizontalRotation);
            transform.RotateAround(focusPoint.position, transform.right, pitch - transform.eulerAngles.x); //Lock camera onto focus point sphere

            transform.LookAt(focusPoint);

        }
    }

    private void HandleMovement()
    {
        Vector3 targetPosition = transform.position;

        float vInput = Input.GetAxisRaw("Vertical");
        float hInput = Input.GetAxisRaw("Horizontal");

        if(vInput == 0 && hInput == 0)  //Condition for function
            return;

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        if(vInput > 0)
            targetPosition += flatForward *movementSpeed * Time.deltaTime;
        if(vInput < 0)
            targetPosition -= flatForward *movementSpeed * Time.deltaTime;

        if(hInput > 0)
            targetPosition += transform.right * movementSpeed * Time.deltaTime;
        if(hInput < 0)
            targetPosition -= transform.right * movementSpeed * Time.deltaTime;

        if(Vector3.Distance(levelCenterPoint, targetPosition) > maxDistanceFromCenter)
        {
            targetPosition = levelCenterPoint + (targetPosition - levelCenterPoint).normalized * maxDistanceFromCenter;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref movementVelocity, smoothTime);
    }
    
    private void HandleMouseMovement()
    {
        if(Input.GetMouseButtonDown(2)) //Condition for function
        {
            lastMousePosition = Input.mousePosition;
        }

        if(Input.GetMouseButton(2))
        {
            Vector3 positionDifference = Input.mousePosition - lastMousePosition;
            Vector3 moveRight = transform.right * (-positionDifference.x) * mouseMovementSpeed * Time.deltaTime;
            Vector3 moveForward = transform.forward * (-positionDifference.y) * mouseMovementSpeed * Time.deltaTime;

            moveRight.y = 0;
            moveForward.y = 0;

            Vector3 movement = moveRight + moveForward;
            Vector3 targetPosition = transform.position + movement;

            if(Vector3.Distance(levelCenterPoint, targetPosition) > maxDistanceFromCenter)  //Handle level constraints
            {
                targetPosition = levelCenterPoint + (targetPosition - levelCenterPoint).normalized * maxDistanceFromCenter;
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref mouseMovementVelocity, smoothTime);
            lastMousePosition = Input.mousePosition;
        }
    }

    private void HandleEdgeMovement()   //Camera movement when moving mouse to sides of screen (Optional but felt right to include)
    {
        Vector3 targetPosition = transform.position;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        if(mousePosition.x > screenWidth - edgeTreshold)
            targetPosition += transform.right * edgeMovementSpeed * Time.deltaTime;

        if(mousePosition.x < edgeTreshold)
            targetPosition -= transform.right * edgeMovementSpeed * Time.deltaTime;

        if(mousePosition.y > screenHeight - edgeTreshold)
            targetPosition += flatForward * edgeMovementSpeed * Time.deltaTime;

        if(mousePosition.y < edgeTreshold)
            targetPosition -= flatForward * edgeMovementSpeed * Time.deltaTime;

        if(Vector3.Distance(levelCenterPoint, targetPosition) > maxDistanceFromCenter)  //Handle level constraints
        {
            targetPosition = levelCenterPoint + (targetPosition - levelCenterPoint).normalized * maxDistanceFromCenter;
        }
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref edgeMovementVelocity, smoothTime);
    }
}
