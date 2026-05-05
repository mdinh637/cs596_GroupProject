using UnityEngine;

public class Archer : Troop
{
    [Header("Archer Stats")]
    [SerializeField] private float damage = 2f; //atk dmg per arrow shot

    [Header("Projectile")]
    [SerializeField] private GameObject arrowPrefab; //arrow to spawn
    [SerializeField] private Transform arrowSpawnPoint; //where arrow comes out from
    [SerializeField] private float arrowSpeed = 25f; //arrow projectile spd

    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private string movingBool = "Moving";

    protected override void Update()
    {
        //call base update (handles movement, targeting, atking)
        base.Update();

        if (animator == null)
            return;

        //if we have a target and in atk range, stay stationary
        if (currentEnemy != null &&
            Vector3.Distance(transform.position, currentEnemy.transform.position) <= attackRange)
        {
            animator.SetBool(movingBool, false); //idle while attacking
        }
        else
        {
            animator.SetBool(movingBool, true); //walking when moving
        }
    }

    protected override void Attack()
    {
        base.Attack(); //updates atk cd timer and log attacks

        if (currentEnemy == null || arrowPrefab == null || arrowSpawnPoint == null)
            return;

        if (animator != null)
        {
            animator.SetTrigger(attackTrigger);
        }

        //spawn arrow
        GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        ArrowProjectile arrow = arrowObj.GetComponent<ArrowProjectile>();

        if (arrow != null)
        {
            arrow.SetTarget(currentEnemy, damage, arrowSpeed);
        }
    }
}