using UnityEngine;

public class BasicKnight : Troop
{
    [Header("Basic Knight Stats")]
    [SerializeField] private float damage = 2f; //basic knight atk stat

    [Header("Animations")]
    [SerializeField] private Animator animator; //animator attached to knight
    [SerializeField] private string attackTrigger = "Attack"; //trigger in knight anim
    [SerializeField] private string movingBool = "Moving"; //bool for movement anim

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource; //audio source attached to knight/skeleton
    [SerializeField] private AudioClip attackSFX; //sound for basic atk
    [SerializeField] private AudioClip walkSFX; //looping walking sound

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
            StopWalkingSFX(); //stop footstep sounds while standing still or attacking
        }
        else
        {
            animator.SetBool(movingBool, true); //walking when moving
            PlayWalkingSFX(); //play looping footstep sounds while moving
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
            audioSource.PlayOneShot(attackSFX); //play sword swing sound
        }

        if (currentEnemy == null)
            return;

        currentEnemy.TakeDamage(damage); //deal dmg to enemy troop
    }

    private void PlayWalkingSFX()
    {
        //don't replay footsteps if already playing or missing audio setup
        if (audioSource == null || walkSFX == null || isPlayingWalkSFX)
            return;

        audioSource.clip = walkSFX; //set current audio clip to footsteps
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