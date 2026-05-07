using UnityEngine;
using System.Reflection;

/// <summary>
/// Attached at runtime by EnemyTower (and optionally TroopDeploymentPanel)
/// to inject scene-object waypoints into a Troop-derived component.
///
/// Uses reflection to write to Troop's protected "waypoints" field so that
/// Troop.cs does not need to be modified at all.
/// </summary>
[DisallowMultipleComponent]
public class TroopWaypointInjector : MonoBehaviour
{
    private bool injected = false;

    /// <summary>
    /// Call immediately after adding this component.
    /// Writes the waypoints array into the Troop base class via reflection.
    /// </summary>
    public void Inject(Transform[] waypoints)
    {
        if (injected) return;

        Troop troop = GetComponent<Troop>();
        if (troop == null)
        {
            Debug.LogWarning("TroopWaypointInjector: no Troop component found on " + gameObject.name);
            return;
        }

        // use reflection to set the protected waypoints field on Troop
        FieldInfo field = typeof(Troop).GetField("waypoints", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(troop, waypoints);
            injected = true;
        }
        else
        {
            Debug.LogWarning("TroopWaypointInjector: could not find 'waypoints' field on Troop via reflection.");
        }

        // also reset the waypoint index field
        FieldInfo indexField = typeof(Troop).GetField("currentWaypointIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        if (indexField != null)
            indexField.SetValue(troop, 0);

        // self-destruct — no longer needed after injection
        Destroy(this);
    }
}
