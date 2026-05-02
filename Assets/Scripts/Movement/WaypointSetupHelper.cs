#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class WaypointSetupHelper : EditorWindow
{
    [MenuItem("Tools/Waypoint Setup Helper")]
    public static void ShowWindow()
    {
        GetWindow<WaypointSetupHelper>("Waypoint Helper");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Quick Waypoint Setup", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Create Waypoint at Camera Position"))
        {
            GameObject waypoint = new GameObject($"Waypoint_{Selection.gameObjects.Length}");
            waypoint.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
            Selection.activeGameObject = waypoint;
        }
        
        if (GUILayout.Button("Create Path Parent"))
        {
            GameObject pathParent = new GameObject("PatrolPath");
            Selection.activeGameObject = pathParent;
        }
        
        if (GUILayout.Button("Select all Waypoints in Selection"))
        {
            var selected = Selection.activeGameObject;
            if (selected != null && selected.name == "PatrolPath")
            {
                var children = selected.GetComponentsInChildren<Transform>();
                foreach (var child in children)
                {
                    if (child != selected && child.name.StartsWith("Waypoint"))
                    {
                        Debug.Log($"Found waypoint: {child.name} at {child.position}");
                    }
                }
            }
        }
    }
}
#endif