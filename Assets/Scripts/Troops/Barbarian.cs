using UnityEngine;

public class Barbarian : Troop
{
    [Header("Barbarian Stats")]
    [SerializeField] private float damage = 5f; //barbarian atk dmg
    [SerializeField] private float upgradeTime = 12f; //time before barbarian upgrades
    [SerializeField] private float upgradedMoveSpeedMultiplier = 3f; //move spd boost after upgrade
    [SerializeField] private float aoeRadius = 3f; //aoe atk range after upgrade

    [Header("Animations")]
    [SerializeField] private Animator animator; //animator attached to barbarian
    [SerializeField] private string attackTrigger = "Attack"; //normal atk trigger
    [SerializeField] private string upgradedAttackTrigger = "UpgradedAttack"; //upgraded aoe atk trigger
    [SerializeField] private string movingBool = "Moving"; //bool for movement anim
    [SerializeField] private string upgradedBool = "Upgraded"; //bool for upgraded run anim

    private bool isUpgraded; //whether barbarian has upgraded
    private float originalMoveSpeed; //starting move spd before upgrade

    protected override void Awake()
    {
        base.Awake();

        isHeavy = true; //barbarian is heavy and resists kb
        originalMoveSpeed = moveSpeed; //save starting move spd

        Invoke(nameof(UpgradeBarbarian), upgradeTime); //upgrade after set time
    }

    protected override void Update()
    {
        //call base update (handles movement, targeting, atking)
        base.Update();

        if (animator == null)
            return;

        animator.SetBool(upgradedBool, isUpgraded); //checks if barbarian upgraded

        //if we have a target and in atk range, stay stationary
        if (currentEnemy != null && Vector3.Distance(transform.position, currentEnemy.transform.position) <= attackRange)
        {
            animator.SetBool(movingBool, false); //idle while attacking
        }
        else
        {
            animator.SetBool(movingBool, true); //walk/run when moving
        }
    }

    private void UpgradeBarbarian()
    {
        if (isUpgraded)
            return;

        isUpgraded = true; //turn on upgraded state
        moveSpeed = originalMoveSpeed * upgradedMoveSpeedMultiplier; //boost move spd

        if (animator != null)
        {
            animator.SetBool(upgradedBool, true); //switch movement anim to upgraded run
        }
    }

    protected override void Attack()
    {
        base.Attack(); //updates atk cd timer and log attacks

        if (currentEnemy == null)
            return;

        if (isUpgraded)
        {
            if (animator != null)
            {
                animator.SetTrigger(upgradedAttackTrigger); //play upgraded aoe atk anim
            }

            UpgradedAttack(); //deal aoe dmg after upgrade
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger(attackTrigger); //play normal atk anim
            }

            currentEnemy.TakeDamage(damage); //deal dmg to current enemy
        }
    }

    private void UpgradedAttack()
    {
        Collider[] enemiesHit = Physics.OverlapSphere(transform.position, aoeRadius, whatIsEnemy); //get enemies in aoe range

        foreach (Collider enemyCollider in enemiesHit)
        {
            Troop enemyTroop = enemyCollider.GetComponentInParent<Troop>(); //get troop from enemy root or parent

            if (enemyTroop == null)
                continue;

            enemyTroop.TakeDamage(damage); //deal aoe dmg to enemy troop
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); //draw normal sight and atk range

        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, aoeRadius); //show upgraded aoe range
    }
}