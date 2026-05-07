using System.Text;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    private Troop target;
    private float damage;
    private float speed;

    [SerializeField] private float lifeTime = 5f;
    [Header("Behavior")]
    [SerializeField] private bool homing = false; // false = one-shot straight velocity, true = re-aim each FixedUpdate
    [Header("Diagnostics")]
    [SerializeField] private bool enableDiagnostics = false;

    private Rigidbody rb;
    private Collider coll;
    private readonly string[] ignoreLayerNames = new[] { "Ground", "Placement" };

    private void Awake()
    {
        rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        // Straight projectile: disable gravity
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (coll != null)
            coll.isTrigger = true;

        // try to suppress noisy ground/placement collisions for this projectile layer
        foreach (var name in ignoreLayerNames)
        {
            int layer = LayerMask.NameToLayer(name);
            if (layer >= 0)
            {
                Physics.IgnoreLayerCollision(gameObject.layer, layer, true);
                if (enableDiagnostics) Debug.Log($"[Arrow] Ignoring collisions between projectile layer ({LayerMask.LayerToName(gameObject.layer)}) and layer '{name}'.");
            }
        }
    }

    public void SetTarget(Troop newTarget, float newDamage, float newSpeed)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;

        if (enableDiagnostics)
            Debug.Log($"[Arrow] Spawned at {transform.position}, target={(target != null ? target.gameObject.name : "null")} dmg={damage} spd={speed} homing={homing}");

        Destroy(gameObject, lifeTime);

        // set initial straight velocity toward the target's aim point
        if (target != null)
        {
            Vector3 aimPoint = GetTargetAimPoint();
            Vector3 dir = aimPoint - transform.position;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Vector3 vel = dir.normalized * speed;
                rb.linearVelocity = vel;
                if (enableDiagnostics) Debug.Log($"[Arrow] Initial velocity = {vel}");
                transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
            }
        }
    }

    private Vector3 GetTargetAimPoint()
    {
        if (target == null)
            return transform.position;

        // attempt to use a collider closest point on the target (handles towers)
        Collider targetCol = target.GetComponentInChildren<Collider>();
        if (targetCol != null)
            return targetCol.ClosestPoint(transform.position);

        // fallback to root position
        return target.transform.position;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (homing)
        {
            Vector3 aimPoint = GetTargetAimPoint();
            Vector3 dir = aimPoint - transform.position;
            if (dir.sqrMagnitude > 0.0001f)
            {
                rb.linearVelocity = dir.normalized * speed;
            }
        }

        // face movement
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }

    private bool IsIgnoredLayer(GameObject go)
    {
        string name = LayerMask.LayerToName(go.layer);
        if (string.IsNullOrEmpty(name)) return false;
        foreach (var n in ignoreLayerNames)
            if (name.Equals(n, System.StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // filter noisy layers early
        if (IsIgnoredLayer(other.gameObject))
        {
            if (enableDiagnostics) Debug.Log($"[Arrow] Ignored trigger with {other.gameObject.name} (layer={LayerMask.LayerToName(other.gameObject.layer)})");
            return;
        }

        // ignore other arrows
        if (other.GetComponentInParent<ArrowProjectile>() != null && other.GetComponentInParent<Troop>() == null)
            return;

        Troop hit = other.GetComponentInParent<Troop>();
        if (hit == null)
        {
            if (enableDiagnostics) Debug.Log("[Arrow] Trigger object not a Troop - ignoring.");
            return;
        }

        // only damage the intended target
        if (hit != target)
        {
            if (enableDiagnostics) Debug.Log($"[Arrow] Hit {hit.gameObject.name} but intended target is {(target != null ? target.gameObject.name : "null")} - ignoring.");
            return;
        }

        if (enableDiagnostics) Debug.Log($"[Arrow] Hit target {hit.gameObject.name}; applying {damage} damage.");
        hit.TakeDamage(damage);
        Destroy(gameObject);
    }
}
