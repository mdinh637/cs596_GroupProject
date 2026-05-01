using UnityEngine;

public class Troop : MonoBehaviour
{
    protected bool troopActive = true; //whether troop is active or not

    protected Rigidbody rb; //rigidbody used for physics based movement

    [Header("Troop Setup")]
    [SerializeField] protected float maxHealth = 10f; //max hp for troop
    [SerializeField] protected float currentHealth; //current hp for troop
    [SerializeField] protected float moveSpeed = 10f; //movement speed
    [SerializeField] protected float sightRange = 30f; //range troop can notice enemies in
    [SerializeField] protected float attackRange = 15f; //range troop can attack in
    [SerializeField] protected float attackCooldown = 1f; //atk cd in seconds
    protected float lastTimeAttacked; //time when troop last attacked

    [Header("Targeting")]
    [SerializeField] protected LayerMask whatIsEnemy; //layer mask for what is considered an enemy
    [SerializeField] protected Transform targetPoint; //point used for distance checks if needed later
    protected Troop currentEnemy; //current enemy troop target

    [Header("Pathing")]
    [SerializeField] protected Transform[] waypoints; //points troop follows when no enemy is nearby
    [SerializeField] protected float waypointArrivalDistance = 0.2f; //how close troop needs to be to reach waypoint
    protected int currentWaypointIndex; //current waypoint troop is moving toward

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>(); //get rigidbody on troop root
        currentHealth = maxHealth; //start troop at full hp
    }

    protected virtual void Update()
    {
        if (troopActive == false)
            return;

        UpdateTarget(); //check for target in sight range first

        if (CanAttack())
            Attack(); //attack if able
    }

    protected virtual void FixedUpdate()
    {
        if (troopActive == false)
            return;

        HandleMovement(); //physics movement should happen in fixed update
    }

    protected virtual void HandleMovement()
    {
        if (rb == null)
            return;

        Vector3 moveDirection; //direction troop should move in

        //if there is no enemy at all, follow waypoint path
        if (currentEnemy == null)
        {
            moveDirection = GetWaypointDirection(); //follow path when no enemy is found
        }
        else
        {
            float distanceToEnemy = Vector3.Distance(transform.position, currentEnemy.transform.position); //distance from this troop to enemy

            //if enemy is in atk range, stop moving and face enemy
            if (distanceToEnemy <= attackRange)
            {
                Vector3 directionToEnemy = currentEnemy.transform.position - transform.position; //direction towards enemy
                directionToEnemy.y = 0; //keep rotation flat

                if (directionToEnemy != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy.normalized); //rotation facing enemy
                    rb.MoveRotation(targetRotation); //rotate using physics
                }

                rb.linearVelocity = Vector3.zero; //stop sliding while attacking
                return; //stop movement in attack range
            }

            //if enemy is only in sight range, move toward enemy
            moveDirection = currentEnemy.transform.position - transform.position; //direction towards enemy
            moveDirection.y = 0; //keep movement flat
            moveDirection = moveDirection.normalized; //normalize so speed stays consistent
        }

        //stop if there is no movement direction
        if (moveDirection == Vector3.zero)
        {
            rb.linearVelocity = Vector3.zero; //stop moving when path ends or no direction exists
            return;
        }

        //move and face movement direction
        Quaternion moveRotation = Quaternion.LookRotation(moveDirection); //rotation facing movement direction
        rb.MoveRotation(moveRotation); //rotate using physics

        Vector3 nextPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime; //next physics position
        rb.MovePosition(nextPosition); //move using physics
    }

    protected virtual Vector3 GetWaypointDirection()
    {
        //if there are no waypoints, keep moving forward
        if (waypoints == null || waypoints.Length == 0)
            return transform.forward;

        //if troop reached the end of the path, stop moving
        if (currentWaypointIndex >= waypoints.Length)
            return Vector3.zero;

        Transform currentWaypoint = waypoints[currentWaypointIndex]; //current point troop should move toward

        //if waypoint is missing, skip it
        if (currentWaypoint == null)
        {
            currentWaypointIndex++;
            return Vector3.zero;
        }

        Vector3 directionToWaypoint = currentWaypoint.position - transform.position; //direction to waypoint
        directionToWaypoint.y = 0; //keep movement flat

        //if close enough to waypoint, move to next one
        if (directionToWaypoint.magnitude <= waypointArrivalDistance)
        {
            currentWaypointIndex++;
            return Vector3.zero;
        }

        return directionToWaypoint.normalized; //return movement direction
    }

    protected virtual void UpdateTarget()
    {
        //if current enemy exists, make sure it is still in sight range
        if (currentEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, currentEnemy.transform.position);

            if (distanceToEnemy <= sightRange)
                return; //keep current target if still in sight range

            currentEnemy = null; //clear target if it leaves sight range
        }

        Collider[] enemiesAround = Physics.OverlapSphere(transform.position, sightRange, whatIsEnemy); //get enemies within sight range

        if (enemiesAround.Length > 0)
        {
            currentEnemy = enemiesAround[0].GetComponentInParent<Troop>(); //get troop from enemy root or parent
        }
        else
        {
            currentEnemy = null; //no target found
        }
    }

    protected virtual void Attack()
    {
        lastTimeAttacked = Time.time; //update last attack time
        Debug.Log(gameObject.name + " attacked " + currentEnemy.gameObject.name);
    }

    protected bool CanAttack()
    {
        if (currentEnemy == null)
            return false;

        float distanceToEnemy = Vector3.Distance(transform.position, currentEnemy.transform.position); //distance from this troop to enemy

        return distanceToEnemy <= attackRange && Time.time > lastTimeAttacked + attackCooldown; //can atk if target is in atk range and cd is over
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage; //subtract incoming damage

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject); //destroy troop when hp hits 0
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange); //show sight range in scene

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); //show atk range in scene
    }
}