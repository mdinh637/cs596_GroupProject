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

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource; //audio source attached to tank
    [SerializeField] private AudioClip attackSFX; //heavy atk swing sound
    [SerializeField] private AudioClip walkSFX; //heavy armor footsteps

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
            PlayWalkingSFX(); //play looping heavy footsteps while moving
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
            audioSource.PlayOneShot(attackSFX); //play heavy sword swing sound
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

    private void PlayWalkingSFX()
    {
        //don't replay footsteps if already playing or missing audio setup
        if (audioSource == null || walkSFX == null || isPlayingWalkSFX)
            return;

        audioSource.clip = walkSFX; //set current audio clip to heavy footsteps
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
}