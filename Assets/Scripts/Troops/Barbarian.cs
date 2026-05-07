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

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource; //audio source attached to barbarian
    [SerializeField] private AudioClip attackSFX; //normal atk sound
    [SerializeField] private AudioClip upgradedAttackSFX; //upgraded aoe atk sound
    [SerializeField] private AudioClip walkSFX; //normal barbarian footsteps
    [SerializeField] private AudioClip runSFX; //upgraded barbarian running footsteps

    private bool isUpgraded; //whether barbarian has upgraded
    private float originalMoveSpeed; //starting move spd before upgrade
    private bool isPlayingMovementSFX; //prevents movement audio from constantly restarting every frame

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
            StopMovementSFX(); //stop movement sounds while attacking
        }
        else
        {
            animator.SetBool(movingBool, true); //walk/run when moving
            PlayMovementSFX(); //play movement sounds while moving
        }
    }

    private void UpgradeBarbarian()
    {
        if (isUpgraded)
            return;

        isUpgraded = true; //turn on upgraded state
        moveSpeed = originalMoveSpeed * upgradedMoveSpeedMultiplier; //boost move spd

        //switch from walking footsteps to upgraded running footsteps
        StopMovementSFX();

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

            if (audioSource != null && upgradedAttackSFX != null)
            {
                audioSource.PlayOneShot(upgradedAttackSFX); //play upgraded aoe atk sound
            }

            UpgradedAttack(); //deal aoe dmg after upgrade
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger(attackTrigger); //play normal atk anim
            }

            if (audioSource != null && attackSFX != null)
            {
                audioSource.PlayOneShot(attackSFX); //play normal atk sound
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

    private void PlayMovementSFX()
    {
        AudioClip currentMovementClip = isUpgraded ? runSFX : walkSFX; //swap between walk and run sounds

        //don't replay movement sounds if already playing or missing audio setup
        if (audioSource == null || currentMovementClip == null || isPlayingMovementSFX)
            return;

        audioSource.clip = currentMovementClip; //set current movement sound
        audioSource.loop = true; //keep movement sounds looping while moving
        audioSource.Play(); //start playing movement sound

        isPlayingMovementSFX = true; //track that movement sounds are currently playing
    }

    private void StopMovementSFX()
    {
        //stop if audio source missing or movement sounds already stopped
        if (audioSource == null || isPlayingMovementSFX == false)
            return;

        audioSource.Stop(); //stop looping movement sounds
        isPlayingMovementSFX = false; //track that movement sounds are no longer playing
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); //draw normal sight and atk range

        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, aoeRadius); //show upgraded aoe range
    }
}