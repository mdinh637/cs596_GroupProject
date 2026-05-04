using UnityEngine;

public class BasicKnight : Troop
{
    [Header("Basic Knight Stats")]
    [SerializeField] private float damage = 2f; //basic knight atk stat

    [Header("Animations")]
    [SerializeField] private Animator animator; //animator attached to knight
    [SerializeField] private string attackTrigger = "Attack"; //trigger in knight anim
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

        currentEnemy.TakeDamage(damage); //deal damage to enemy troop
    }
}