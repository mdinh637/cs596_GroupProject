using UnityEngine;

/// <summary>
/// moves this object through waypoint points at a steady speed
/// moves this transform along an ordered list of waypoints at constant speed
/// attach to a character root; assign empty transforms in the scene as path points
/// </summary>
public class WaypointMover : MonoBehaviour
{
    // path setup values
    [Header("Path")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool loopPath;

    // movement values
    [Header("Motion")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float arrivalThreshold = 0.15f;
    [SerializeField] private bool faceMovementDirection = true;
    [SerializeField] private float rotationSpeedDegrees = 540f;

    // animation hookup values
    [Header("Animation (optional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedFloatParameter = "Speed";

    // current waypoint index
    private int _currentIndex;

    private void Update()
    {
        // stop early if there is no path
        if (waypoints == null || waypoints.Length == 0)
            return;

        // get the current waypoint we should move toward
        Transform target = GetNextValidWaypoint();
        if (target == null)
            return;

        Vector3 from = transform.position;
        Vector3 to = target.position;
        Vector3 delta = to - from;
        // use x and z only so distance ignores height
        Vector3 flatDelta = new Vector3(delta.x, 0f, delta.z);

        if (flatDelta.sqrMagnitude < 0.0001f)
        {
            // already at this point, move to the next one
            AdvanceIndex();
            return;
        }

        float distanceToTarget = flatDelta.magnitude;
        Vector3 direction = flatDelta.normalized;
        Vector3 nextPos = Vector3.MoveTowards(from, to, moveSpeed * Time.deltaTime);

        // move to the next waypoint position, including height
        transform.position = nextPos;

        if (faceMovementDirection && direction.sqrMagnitude > 0.0001f)
        {
            // turn the character to face where it is moving
            Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeedDegrees * Time.deltaTime);
        }

        // set animation speed: 1 when moving, 0 when close enough
        SetAnimatorSpeed(distanceToTarget > arrivalThreshold ? 1f : 0f);

        // once close enough to this point, switch to the next one
        if (Vector3.Distance(new Vector3(nextPos.x, 0f, nextPos.z), new Vector3(to.x, 0f, to.z)) <= arrivalThreshold)
            AdvanceIndex();
    }

    // gets the current valid waypoint to move toward
    private Transform GetNextValidWaypoint()
    {
        // skip empty waypoint slots
        while (_currentIndex < waypoints.Length && waypoints[_currentIndex] == null)
            _currentIndex++;

        if (_currentIndex >= waypoints.Length)
            return null;

        return waypoints[_currentIndex];
    }

    // moves the waypoint index forward or stops at the end
    private void AdvanceIndex()
    {
        // go to the next waypoint
        _currentIndex++;
        if (_currentIndex >= waypoints.Length)
        {
            if (loopPath)
                // start over when loop is on
                _currentIndex = 0;
            else
            {
                // stop moving and force idle at the end
                SetAnimatorSpeed(0f);
                enabled = false;
            }
        }
    }

    // updates the animator speed parameter when available
    private void SetAnimatorSpeed(float value)
    {
        // skip if animation is not wired yet
        if (animator == null || string.IsNullOrEmpty(speedFloatParameter))
            return;

        // update the animator speed parameter
        animator.SetFloat(speedFloatParameter, value);
    }

#if UNITY_EDITOR
    // draws the path lines in scene view when selected
    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        // draw simple lines so the path is easy to see
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null)
                continue;
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
#endif
}
