using UnityEngine;

/// <summary>
/// Passive enemy tower that periodically spawns enemy units down the lane.
/// Supports multiple enemy types with individual spawn weights.
/// The higher the weight relative to other entries, the more frequently
/// that unit type will be chosen.
/// </summary>
public class EnemyTower : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Spawn entry — one per enemy type
    // -------------------------------------------------------------------------
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public GameObject prefab;           // enemy prefab to spawn
        [Range(0f, 10f)]
        public float weight = 1f;           // relative spawn chance (higher = more frequent)
    }

    // -------------------------------------------------------------------------
    // Health
    // -------------------------------------------------------------------------
    [Header("Health")]
    [SerializeField] private float maxHealth = 500f;
    private float currentHealth;

    // -------------------------------------------------------------------------
    // Spawning
    // -------------------------------------------------------------------------
    [Header("Spawning")]
    [SerializeField] private EnemySpawnEntry[] enemyTypes;  // add one entry per enemy type
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private int maxActiveEnemies = 10;
    private float spawnTimer;
    private int activeEnemyCount;

    // -------------------------------------------------------------------------
    // Waypoints
    // -------------------------------------------------------------------------
    [Header("Lane Waypoints")]
    [SerializeField] private Transform[] laneWaypoints;

    // -------------------------------------------------------------------------
    // References
    // -------------------------------------------------------------------------
    [Header("References")]
    [SerializeField] private EnemyTowerHealthUI healthUI;
    [SerializeField] private WinLoseScreen winLoseScreen;

    private bool isTowerDestroyed = false;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Start()
    {
        currentHealth = maxHealth;
        spawnTimer = spawnInterval;

        if (healthUI != null)
            healthUI.Hide();
    }

    private void Update()
    {
        if (isTowerDestroyed)
            return;

        HandleSpawnTimer();
    }

    // -------------------------------------------------------------------------
    // Spawning
    // -------------------------------------------------------------------------

    private void HandleSpawnTimer()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnEnemy();
        }
    }

    private void TrySpawnEnemy()
    {
        if (enemyTypes == null || enemyTypes.Length == 0)
        {
            Debug.LogWarning("EnemyTower: no enemy types assigned.");
            return;
        }

        if (activeEnemyCount >= maxActiveEnemies)
            return;

        GameObject selectedPrefab = GetWeightedRandomPrefab();
        if (selectedPrefab == null)
            return;

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject spawnedUnit = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        activeEnemyCount++;

        // inject waypoints at runtime
        TroopWaypointInjector injector = spawnedUnit.GetComponent<TroopWaypointInjector>();
        if (injector == null)
            injector = spawnedUnit.AddComponent<TroopWaypointInjector>();
        injector.Inject(laneWaypoints);

        // attach death reporter
        EnemyDeathReporter reporter = spawnedUnit.AddComponent<EnemyDeathReporter>();
        reporter.Initialize(this);
    }

    /// <summary>
    /// Picks a random enemy prefab based on relative weights.
    /// Higher weight = higher chance of being selected.
    /// </summary>
    private GameObject GetWeightedRandomPrefab()
    {
        float totalWeight = 0f;
        foreach (EnemySpawnEntry entry in enemyTypes)
            totalWeight += entry.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (EnemySpawnEntry entry in enemyTypes)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry.prefab;
        }

        // fallback to first entry
        return enemyTypes[0].prefab;
    }

    public void OnEnemyUnitDied()
    {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    // -------------------------------------------------------------------------
    // Health / damage
    // -------------------------------------------------------------------------

    public void TakeDamage(float amount)
    {
        if (isTowerDestroyed)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);

        if (healthUI != null)
            healthUI.ShowAndUpdate(currentHealth, maxHealth);

        if (GameUI.Instance != null)
            GameUI.Instance.GetEnemyHealthBar()?.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
            HandleTowerDestroyed();
    }

    private void HandleTowerDestroyed()
    {
        isTowerDestroyed = true;
        Debug.Log("EnemyTower: destroyed — player wins!");

        if (healthUI != null)
            healthUI.Hide();

        if (winLoseScreen != null)
            winLoseScreen.ShowWin();
        else if (GameUI.Instance != null)
            GameUI.Instance.DamageEnemyBase(maxHealth);
    }

    // -------------------------------------------------------------------------
    // Public accessors
    // -------------------------------------------------------------------------

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDestroyed() => isTowerDestroyed;

    // -------------------------------------------------------------------------
    // Editor helpers
    // -------------------------------------------------------------------------

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (laneWaypoints != null && laneWaypoints.Length > 1)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < laneWaypoints.Length - 1; i++)
            {
                if (laneWaypoints[i] == null || laneWaypoints[i + 1] == null) continue;
                Gizmos.DrawLine(laneWaypoints[i].position, laneWaypoints[i + 1].position);
            }
        }

        if (spawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
        }
    }
#endif
}