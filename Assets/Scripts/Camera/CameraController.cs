using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool canControl;
    [SerializeField] private Vector3 levelCenterPoint; //Click on center of level to find coordinates
    [SerializeField] private float maxDistanceFromCenter;

    [Header("Movement Details")]
    [SerializeField] private float movementSpeed = 120;

    [Header("Rotation Details")]
    [SerializeField] private float rotationSpeed = 200;
    private float yaw;
    private float pitch;
    [SerializeField] private float minPitch = 5;
    [SerializeField] private float maxPitch = 85;

    [Header("Zoom Details")]
    [SerializeField] private float zoomSpeed = 35;
    [SerializeField] private float minZoom = 3;
    [SerializeField] private float maxZoom = 15;

    private float smoothTime = 0.1f;
    private Vector3 movementVelocity = Vector3.zero;
    private Vector3 zoomVelocity = Vector3.zero;

    private void Start()
    {
        yaw = transform.eulerAngles.y; //store starting y rotation
        pitch = transform.eulerAngles.x; //store starting x rotation
    }

    void Update()
    {
        if (canControl == false)
            return;

        HandleRotation();
        HandleZoom();
        HandleMovement();
    }

    public void EnableCameraControls(bool enable) => canControl = enable;
    public float AdjustPitchValue(float value) => pitch = value;

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomDirection = transform.forward * scroll * zoomSpeed;
        Vector3 targetPosition = transform.position + zoomDirection;

        if (transform.position.y < minZoom && scroll > 0) //stop camera from zooming in too close
            return;

        if (transform.position.y > maxZoom && scroll < 0) //stop camera from zooming out too far
            return;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref zoomVelocity, smoothTime);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1)) //RMB for rotations
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            yaw += horizontalRotation; //rotate camera left and right
            pitch = Mathf.Clamp(pitch - verticalRotation, minPitch, maxPitch); //limit camera looking up and down

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f); //rotate in place instead of orbiting around focus point
        }
    }

    private void HandleMovement()
    {
        Vector3 targetPosition = transform.position;

        float vInput = Input.GetAxisRaw("Vertical");
        float hInput = Input.GetAxisRaw("Horizontal");

        if (vInput == 0 && hInput == 0) //condition for function
            return;

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        if (vInput > 0)
            targetPosition += flatForward * movementSpeed * Time.deltaTime;
        if (vInput < 0)
            targetPosition -= flatForward * movementSpeed * Time.deltaTime;

        if (hInput > 0)
            targetPosition += flatRight * movementSpeed * Time.deltaTime;
        if (hInput < 0)
            targetPosition -= flatRight * movementSpeed * Time.deltaTime;

        if (Vector3.Distance(levelCenterPoint, targetPosition) > maxDistanceFromCenter)
        {
            targetPosition = levelCenterPoint + (targetPosition - levelCenterPoint).normalized * maxDistanceFromCenter;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref movementVelocity, smoothTime);
    }
}