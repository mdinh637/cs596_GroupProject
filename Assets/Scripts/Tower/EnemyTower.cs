using UnityEngine;

/// Passive enemy tower that periodically spawns enemy units down the lane.
/// Now extends Troop so ally units can detect and target it as a valid enemy.
/// Supports multiple enemy types with individual spawn weights and spawn counts.
public class EnemyTower : Troop
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public GameObject prefab;           //enemy prefab to spawn
        [Range(0f, 10f)]
        public float weight = 1f;           //relative spawn chance
        [Range(1, 10)]
        public int spawnCount = 1;          //how many to spawn at once
        public float spawnSpread = 1.5f;    //random position offset between grouped units
    }

    [Header("Spawning")]
    [SerializeField] private EnemySpawnEntry[] enemyTypes;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private int maxActiveEnemies = 10;
    private float spawnTimer;
    private int activeEnemyCount;

    [Header("Lane Waypoints")]
    [SerializeField] private Transform[] laneWaypoints;

    [Header("References")]
    [SerializeField] private EnemyTowerHealthUI healthUI;
    [SerializeField] private WinLoseScreen winLoseScreen;

    private bool isTowerDestroyed = false;

    protected override void Awake()
    {
        base.Awake(); //initialize Troop base (health, rb, etc.)
        maxHealth = 500f; //set tower health via Troop field
        currentHealth = maxHealth;
    }

    private void Start()
    {
        spawnTimer = spawnInterval;

        if (healthUI != null)
            healthUI.Hide();
    }

    protected override void Update()
    {
        //override Troop.Update() to skip targeting/attack logic
        //only handle spawning for EnemyTower
        if (isTowerDestroyed)
            return;

        HandleSpawnTimer();
    }

    protected override void FixedUpdate()
    {
        //override Troop.FixedUpdate() to skip movement logic
        //towers don't move
    }

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

        EnemySpawnEntry selectedEntry = GetWeightedRandomEntry();
        if (selectedEntry == null || selectedEntry.prefab == null)
            return;

        //spawn the full group
        int countToSpawn = Mathf.Min(selectedEntry.spawnCount, maxActiveEnemies - activeEnemyCount);

        for (int i = 0; i < countToSpawn; i++)
        {
            //spread units slightly so they don't all stack on the same point
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
            spawnPosition += new Vector3(
                Random.Range(-selectedEntry.spawnSpread, selectedEntry.spawnSpread),
                0f,
                Random.Range(-selectedEntry.spawnSpread, selectedEntry.spawnSpread)
            );

            GameObject spawnedUnit = Instantiate(selectedEntry.prefab, spawnPosition, Quaternion.identity);
            activeEnemyCount++;

            //inject waypoints
            TroopWaypointInjector injector = spawnedUnit.GetComponent<TroopWaypointInjector>();
            if (injector == null)
                injector = spawnedUnit.AddComponent<TroopWaypointInjector>();
            injector.Inject(laneWaypoints);

            //attach death reporter
            EnemyDeathReporter reporter = spawnedUnit.AddComponent<EnemyDeathReporter>();
            reporter.Initialize(this);
        }
    }

    private EnemySpawnEntry GetWeightedRandomEntry()
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
                return entry;
        }

        return enemyTypes[0];
    }

    public void OnEnemyUnitDied()
    {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }

    public override void TakeDamage(float amount)
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

    protected override void Attack()
    {
        //override to do nothing—towers don't attack
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

    public bool IsDestroyed() => isTowerDestroyed;

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