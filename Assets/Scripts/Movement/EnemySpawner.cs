using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Area")]
    [SerializeField] private Transform spawnAreaCenter;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10f, 1f, 10f);
    [SerializeField] private bool showSpawnGizmo = true;
    
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] patrolWaypoints;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float spawnDelaySeconds = 3f;
    [SerializeField] private bool spawnOnStart = true;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    void Start()
    {
        if (spawnOnStart)
        {
            for (int i = 0; i < maxEnemies; i++)
            {
                SpawnEnemy();
            }
        }
    }
    
    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("No enemy prefab assigned to spawner!");
            return;
        }
        
        // Remove null enemies from list
        activeEnemies.RemoveAll(e => e == null);
        
        if (activeEnemies.Count >= maxEnemies)
            return;
        
        Vector3 spawnPosition = GetRandomPositionInSpawnArea();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Assign waypoints to the enemy's WaypointMover
        WaypointMover mover = newEnemy.GetComponent<WaypointMover>();
        if (mover != null && patrolWaypoints != null && patrolWaypoints.Length > 0)
        {
            // Create a copy of waypoints for this enemy
            mover.SetWaypoints(patrolWaypoints);
        }
        
        activeEnemies.Add(newEnemy);
    }
    
    private Vector3 GetRandomPositionInSpawnArea()
    {
        Vector3 center = spawnAreaCenter != null ? spawnAreaCenter.position : transform.position;
        
        float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomZ = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);
        
        return new Vector3(center.x + randomX, center.y, center.z + randomZ);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!showSpawnGizmo) return;
        
        Vector3 center = spawnAreaCenter != null ? spawnAreaCenter.position : transform.position;
        
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(center, spawnAreaSize);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, spawnAreaSize);
    }
    #endif
}