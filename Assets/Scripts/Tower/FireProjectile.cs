using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public Rigidbody m_projectile;
    public Transform m_launchPoint;
    public float m_speed = 1f;

    [Header("Raycast Targeting")]
    private float m_raycastMaxDistance = 500f;
    /*this all layers is just for testing purposes
     but in practice this isn't what we want. 
     we need to specifically mask everything but ground so our 
     ray doesn't get short by hitting enemies or friendlies*/
    private LayerMask m_raycastLayers = Physics.AllLayers;
    private float m_currMaxFiringRange = 12f;
    private float m_lobHeight = 2f;

    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            if (m_projectile == null) { return; }

            Transform spawn = m_launchPoint != null ? m_launchPoint : transform;
            Rigidbody p = Instantiate(m_projectile, spawn.position, spawn.rotation);
            p.linearVelocity = spawn.forward * m_speed;
        }*/

        /*If we haven't clicked and we are not in range then don't fire*/
        /*we have to use single line guard to avoid short circuit no assign errors*/
        if (!Input.GetMouseButtonDown(0)) return;
        if (m_projectile == null) return;
        if (!TryGetMouseWorldPoint(out Vector3 targetPoint)) return;
        if (!TargetInRange(targetPoint, out float flatDistance)) return;

        FireLobAtTarget(targetPoint);
    }

    void FireLobAtTarget(Vector3 targetPoint)
    {
        Transform spawn = m_launchPoint != null ? m_launchPoint : transform;

        if (!TryCalculateLobVelocity(spawn.position, targetPoint, m_lobHeight, out Vector3 launchVelocity))
            return;

        Rigidbody p = Instantiate(m_projectile, spawn.position, spawn.rotation);

        // Use lob result (not spawn.forward)
        p.linearVelocity = launchVelocity * m_speed;
    }

    bool TryCalculateLobVelocity(Vector3 start, Vector3 target, float extraPeakHeight, out Vector3 velocity)
    {
        velocity = default;

        float g = Mathf.Abs(Physics.gravity.y);
        if (g < 0.001f) return false;

        Vector3 toTarget = target - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);

        float peakY = Mathf.Max(start.y, target.y) + Mathf.Max(0.1f, extraPeakHeight);
        float upDistance = peakY - start.y;
        float downDistance = peakY - target.y;

        float timeUp = Mathf.Sqrt(2f * upDistance / g);
        float timeDown = Mathf.Sqrt(2f * downDistance / g);
        float totalTime = timeUp + timeDown;

        if (totalTime < 0.001f) return false;

        Vector3 velocityXZ = toTargetXZ / totalTime;
        float velocityY = g * timeUp;

        velocity = velocityXZ + Vector3.up * velocityY;
        return true;
    }

    /*out means that var must be assigned before function returns*/
    bool TryGetMouseWorldPoint(out Vector3 point)
    {
        point = default;

        /*find the main camera*/
        Camera cam = Camera.main;
        if (cam == null) return false;

        /*convert the mouse pos to a ray so we can raycast*/
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        /*raycast needs to ignore other colliders so we just check for dist to ground point we want*/
        if (!Physics.Raycast(ray, out RaycastHit hit, m_raycastMaxDistance, m_raycastLayers, QueryTriggerInteraction.Ignore))
            return false;

        point = hit.point;
        return true;
    }

    bool TargetInRange(Vector3 targetPoint, out float flatDistance)
    {
        Transform spawn = m_launchPoint == null ? transform : m_launchPoint;

        Vector3 delta = targetPoint - spawn.position;
        /*we just want to deal with the x component by itself*/
        delta.y = 0f;

        flatDistance = delta.magnitude;
        return flatDistance <= m_currMaxFiringRange;
    }

    void OnDrawGizmosSelected()
    {
        Transform spawn = m_launchPoint != null ? m_launchPoint : transform;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawn.position, m_currMaxFiringRange);
    }
}
