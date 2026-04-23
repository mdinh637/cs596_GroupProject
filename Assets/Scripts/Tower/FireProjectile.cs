using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [Header("Projectile")]
    public Rigidbody m_projectile;
    public Transform m_launchPoint;
    public float m_speed = 1f;

    [Header("Raycast Projectile Targeting")]
    private float m_raycastMaxDistance = 500f;
    /*this all layers is just for testing purposes
     but in practice this isn't what we want. 
     we need to specifically mask everything but ground so our 
     ray doesn't get short by hitting enemies or friendlies*/
    private LayerMask m_raycastLayers = Physics.AllLayers;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (m_projectile == null) { return; }

            Transform spawn = m_launchPoint != null ? m_launchPoint : transform;
            Rigidbody p = Instantiate(m_projectile, spawn.position, spawn.rotation);
            p.linearVelocity = spawn.forward * m_speed;
        }
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
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, ~0, QueryTriggerInteraction.Ignore))
            return false;

        point = hit.point;
        return true;
    }
}
