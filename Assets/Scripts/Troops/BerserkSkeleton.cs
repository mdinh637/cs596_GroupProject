using UnityEngine;

public class BerserkSkeleton : Troop
{
    [Header("Berserk Skeleton Stats")]
    [SerializeField] private float damage = 4f; //aoe dmg stat
    [SerializeField] private float aoeRadius = 3f; //range around skeleton hit by aoe
    [SerializeField] private float knockbackForce = 5f; //knockback force on troops that aren't heavy
    [SerializeField] private float knockbackUpForce = 5f; //upward knockback force on said troops ^

    [Header("Animations")]
    [SerializeField] private Animator animator; //animator attached to berserk skeleton
    [SerializeField] private string attackTrigger = "Attack"; //trigger in skeleton anim
    [SerializeField] private string movingBool = "Moving"; //bool for movement anim

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource; //audio source attached to berserk skeleton
    [SerializeField] private AudioClip attackSFX; //aoe atk sound
    [SerializeField] private AudioClip walkSFX; //looping skeleton walk sound

    private bool isPlayingWalkSFX; //prevents walk audio from constantly restarting every frame

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
            StopWalkingSFX(); //stop footsteps while attacking or standing still
        }
        else
        {
            animator.SetBool(movingBool, true); //walking when moving
            PlayWalkingSFX(); //play looping skeleton footsteps while moving
        }
    }

    protected override void Attack()
    {
        base.Attack(); //updates atk cd timer and log attacks

        if (animator != null)
        {
            animator.SetTrigger(attackTrigger); //play atk animation
        }

        if (audioSource != null && attackSFX != null)
        {
            audioSource.PlayOneShot(attackSFX); //play aoe atk sound
        }

        Collider[] enemiesHit = Physics.OverlapSphere(transform.position, aoeRadius, whatIsEnemy); //get enemies in aoe range

        foreach (Collider enemyCollider in enemiesHit)
        {
            Troop enemyTroop = enemyCollider.GetComponentInParent<Troop>(); //get troop from enemy root or parent

            if (enemyTroop == null)
                continue;

            enemyTroop.TakeDamage(damage); //deal aoe dmg to enemy troop

            //only knockback troops that aren't heavy type
            if (enemyTroop.IsHeavy() == false)
            {
                Vector3 knockbackDirection = enemyTroop.transform.position - transform.position; //direction away from skeleton
                knockbackDirection.y = 0;

                if (knockbackDirection != Vector3.zero)
                {
                    Vector3 knockbackForceDirection = knockbackDirection.normalized * knockbackForce; //horizontal knockback force
                    knockbackForceDirection.y = knockbackUpForce; //add upward force for arc

                    enemyTroop.ApplyKnockback(knockbackForceDirection); //apply knockback through troop logic
                }
            }
        }
    }

    private void PlayWalkingSFX()
    {
        //don't replay footsteps if already playing or missing audio setup
        if (audioSource == null || walkSFX == null || isPlayingWalkSFX)
            return;

        audioSource.clip = walkSFX; //set current audio clip to skeleton footsteps
        audioSource.loop = true; //keep footsteps looping while moving
        audioSource.Play(); //start playing footsteps

        isPlayingWalkSFX = true; //track that footsteps are currently playing
    }

    private void StopWalkingSFX()
    {
        //stop if audio source missing or footsteps already stopped
        if (audioSource == null || isPlayingWalkSFX == false)
            return;

        audioSource.Stop(); //stop looping footsteps
        isPlayingWalkSFX = false; //track that footsteps are no longer playing
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos(); //draw normal sight and atk range

        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, aoeRadius); //show aoe range in scene
    }
}