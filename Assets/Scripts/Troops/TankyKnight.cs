using UnityEngine;

public class TankyKnight : Troop
{
    [Header("Tanky Knight Stats")]
    [SerializeField] private float damage = 5f; //high dmg, low atk spd
    [SerializeField] private float knockbackForce = 8f; //knockback force on troops that aren't heavy
    [SerializeField] private float knockbackUpForce = 3f; //upward knockback force on said troops ^

    [Header("Animations")]
    [SerializeField] private Animator animator; //animator attached to tanky knight
    [SerializeField] private string attackTrigger = "Attack"; //trigger in tanky knight anim
    [SerializeField] private string movingBool = "Moving"; //bool for movement anim

    protected override void Update()
    {
        //call base update (handles movement, targeting, atking)
        base.Update();

        if (animator == null)
            return;

        //if we have a target and in atk range, stay stationary
        if (currentEnemy != null && Vector3.Distance(transform.position, currentEnemy.transform.position) <= attackRange)
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

        if (animator != null)
        {
            animator.SetTrigger(attackTrigger); //play atk animation
        }

        if (currentEnemy == null)
            return;

        currentEnemy.TakeDamage(damage); //deal damage to current enemy

        //only knockback troops that aren't heavy typing
        if (currentEnemy.IsHeavy() == false)
        {
            Rigidbody enemyRb = currentEnemy.GetComponent<Rigidbody>(); //get enemy rb for knockback

            if (enemyRb != null)
            {
                Vector3 knockbackDirection = currentEnemy.transform.position - transform.position; //direction away from tank
                knockbackDirection.y = 0; //keep horizontal direction flat

                if (knockbackDirection != Vector3.zero)
                {
                    Vector3 knockbackForceDirection = knockbackDirection.normalized * knockbackForce; //horizontal knockback force
                    knockbackForceDirection.y = knockbackUpForce; //add upward force for arc

                    currentEnemy.ApplyKnockback(knockbackForceDirection); //apply knockback through troop logic
                }
            }
        }
    }
}