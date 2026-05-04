using UnityEngine;

public class Troop : MonoBehaviour
{
    protected bool troopActive = true; //whether troop is active or not

    protected Rigidbody rb; //rigidbody setup for physics and funny clash royale crowding moments

    [Header("Troop Setup")]
    //default stats, subject to change based on troop type and what we feel like is good balancing
    //rlly bs scaling below headsup did not account for actual map size outside of testing map we had
    [SerializeField] protected float maxHealth = 10f; //max hp for troop
    [SerializeField] protected float currentHealth; //current hp for troop
    [SerializeField] protected float moveSpeed = 10f; //movement speed
    [SerializeField] protected float sightRange = 30f; //range troop can notice enemies in
    [SerializeField] protected float attackRange = 15f; //range troop can attack in
    [SerializeField] protected float attackCooldown = 1f; //atk cd in seconds
    protected float lastTimeAttacked; //time when troop last attacked

    [Header("Troop Type")]
    [SerializeField] protected bool isHeavy = false; //whether troop resists knockback, exclusive trait for tanks

    [SerializeField] protected float knockbackRecoverTime = 0.5f; //time delay before resuming movement, fixes weird head stack glitch
    protected bool isKnockedBack; //whether troop is currently knocked back

    [Header("Targeting")]
    [SerializeField] protected LayerMask whatIsEnemy; //layer mask for what is considered an enemy
    [SerializeField] protected Transform targetPoint; //point used for distance checks if needed later
    protected Troop currentEnemy; //current enemy troop target

    [Header("Pathing")]
    [SerializeField] protected Transform[] waypoints; //points troop follows when no enemy is nearby, leads to bases
    [SerializeField] protected float waypointArrivalDistance = 0.1f; //how close troop needs to be to reach waypoint
    protected int currentWaypointIndex; //setting up waypoint index for traversal between one point and the next

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>(); //get rigidbody on troop root
        currentHealth = maxHealth; //start troop at full hp
        lastTimeAttacked = -attackCooldown; //allows units with high cd atk to attack immediately on first atk
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

        HandleMovement(); //calls movement logic
    }

    public bool IsHeavy()
    {
        return isHeavy; //return if troop can resist kb effect
    }

    protected virtual void HandleMovement()
    {
        if (rb == null)
            return;

        Vector3 moveDirection; //direction troop should move in

        if (isKnockedBack)
            return; //skip movement control during kb

        //if no enemy in sight, follow waypoint path
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
                directionToEnemy.y = 0;

                if (directionToEnemy != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy.normalized); //rotation facing enemy
                    rb.MoveRotation(targetRotation); //rotate using physics
                }

                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); //keeping gravity to implement knockback effect later, otherwise stops horizontal movement
                return; //stop movement once in atk range
            }

            //if enemy is only in sight range, keep moving towards them
            moveDirection = currentEnemy.transform.position - transform.position; //direction towards enemy
            moveDirection.y = 0;
            moveDirection = moveDirection.normalized; //keeping speed consistent
        }

        //stop if there is no movement direction
        if (moveDirection == Vector3.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        //move and face movement direction
        Quaternion moveRotation = Quaternion.LookRotation(moveDirection);
        rb.MoveRotation(moveRotation);

        Vector3 nextPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime; //sets next position
        rb.MovePosition(nextPosition);
    }

    protected virtual Vector3 GetWaypointDirection()
    {
        //if there are no waypoints, keep moving forward
        if (waypoints == null || waypoints.Length == 0)
            return transform.forward;

        //if troop reaches second waypoint in index, stop movement
        if (currentWaypointIndex >= waypoints.Length)
            return Vector3.zero;

        Transform currentWaypoint = waypoints[currentWaypointIndex]; //current point troop moves forward from and to next

        //if no waypoint, move to next one
        if (currentWaypoint == null)
        {
            currentWaypointIndex++;
            return Vector3.zero;
        }

        Vector3 directionToWaypoint = currentWaypoint.position - transform.position; //direction to waypoint
        directionToWaypoint.y = 0;

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

    public virtual void ApplyKnockback(Vector3 force)
    {
        if (isHeavy)
            return;

        isKnockedBack = true; //pause normal movement while launched to prevent weird head stack spasms

        if (rb != null)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); //clear horizontal movement first
            rb.AddForce(force, ForceMode.Impulse); //apply knockback force
        }

        Invoke(nameof(RecoverFromKnockback), knockbackRecoverTime); //resume movement after delay
    }

    protected virtual void RecoverFromKnockback()
    {
        isKnockedBack = false; //allow troop to move again
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage; //reduce health by damage taken

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject); //destroy troop when hp hits 0, clean up heh
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange); //show sight range in scene

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); //show atk range in scene
    }
}