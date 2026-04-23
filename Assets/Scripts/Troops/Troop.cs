using UnityEngine;

public class Troop : MonoBehaviour
{
    protected bool troopActive = true; //whether troop is active or not

    [Header("Troop Setup")]
    [SerializeField] protected float maxHealth = 10f; //max hp for troop
    [SerializeField] protected float currentHealth; //current hp for troop
    [SerializeField] protected float moveSpeed = 3f; //movement speed
    [SerializeField] protected float sightRange = 4f; //range troop can notice enemies in
    [SerializeField] protected float attackRange = 1.5f; //range troop can attack in
    [SerializeField] protected float attackCooldown = 1f; //atk cd in seconds
    protected float lastTimeAttacked; //time when troop last attacked

    [Header("Targeting")]
    [SerializeField] protected LayerMask whatIsEnemy; //layer mask for what is considered an enemy
    [SerializeField] protected Transform targetPoint; //point used for distance checks if needed later
    protected Troop currentEnemy; //current enemy troop target

    protected virtual void Awake()
    {
        currentHealth = maxHealth; //start troop at full hp
    }

    protected virtual void Update()
    {
        if (troopActive == false)
            return;

        UpdateTarget(); //check for target in sight range first
        HandleMovement(); //handle movement after target check

        if (CanAttack())
            Attack(); //attack if able
    }

    protected virtual void HandleMovement()
    {
        //if there is no enemy at all, keep moving forward
        if (currentEnemy == null)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            return;
        }

        float distanceToEnemy = Vector3.Distance(transform.position, currentEnemy.transform.position); //distance from this troop to enemy

        //if enemy is in atk range, stop moving and face enemy
        if (distanceToEnemy <= attackRange)
        {
            Vector3 directionToEnemy = currentEnemy.transform.position - transform.position; //direction towards enemy
            directionToEnemy.y = 0; //keep rotation flat

            if (directionToEnemy != Vector3.zero)
            {
                transform.forward = directionToEnemy.normalized; //face enemy while attacking
            }

            return; //stop movement in attack range
        }

        //if enemy is only in sight range, move toward enemy
        Vector3 moveDirection = currentEnemy.transform.position - transform.position; //direction towards enemy
        moveDirection.y = 0; //keep movement flat

        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection.normalized; //face enemy while moving toward them
        }

        transform.position += transform.forward * moveSpeed * Time.deltaTime; //move toward enemy
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
            currentEnemy = enemiesAround[0].GetComponent<Troop>(); //for now just take the first troop found
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