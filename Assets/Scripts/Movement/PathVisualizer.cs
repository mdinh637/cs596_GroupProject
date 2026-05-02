
using UnityEngine;

// Create a PathManager script for visual reference
public class PathVisualizer : MonoBehaviour
{
    public Transform[] waypoints;
    public Color pathColor = Color.green;

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = pathColor;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}