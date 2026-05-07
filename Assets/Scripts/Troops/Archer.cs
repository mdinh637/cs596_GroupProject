using UnityEngine;

public class Archer : Troop
{
    [Header("Archer Stats")]
    [SerializeField] private float damage = 1f; //atk dmg per arrow shot

    [Header("Projectile")]
    [SerializeField] private GameObject arrowPrefab; //arrow to spawn
    [SerializeField] private Transform arrowSpawnPoint; //where arrow comes out from
    [SerializeField] private float arrowSpeed = 25f; //arrow projectile spd

    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private string movingBool = "Moving";

    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource; //audio source attached to archer
    [SerializeField] private AudioClip attackSFX; //arrow shot sound
    [SerializeField] private AudioClip walkSFX; //looping archer footstep sound

    private bool isPlayingWalkSFX; //prevents walk audio from constantly restarting every frame

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
            StopWalkingSFX(); //stop footsteps while attacking or standing still
        }
        else
        {
            animator.SetBool(movingBool, true); //walking when moving
            PlayWalkingSFX(); //play looping archer footsteps while moving
        }
    }

    protected override void Attack()
    {
        base.Attack(); //updates atk cd timer and log attacks

        //make sure we have everything needed before firing arrow
        if (currentEnemy == null || arrowPrefab == null || arrowSpawnPoint == null)
            return;

        //play atk animation when firing arrow
        if (animator != null)
        {
            animator.SetTrigger(attackTrigger);
        }

        if (audioSource != null && attackSFX != null)
        {
            audioSource.PlayOneShot(attackSFX); //play arrow shot sound
        }

        GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity); //spawn arrow at bow position

        ArrowProjectile arrow = arrowObj.GetComponent<ArrowProjectile>(); //get arrow script from spawned prefab

        //pass target, dmg, and spd into arrow so it knows what to do
        if (arrow != null)
        {
            arrow.SetTarget(currentEnemy, damage, arrowSpeed);
        }
    }

    private void PlayWalkingSFX()
    {
        //don't replay footsteps if already playing or missing audio setup
        if (audioSource == null || walkSFX == null || isPlayingWalkSFX)
            return;

        audioSource.clip = walkSFX; //set current audio clip to archer footsteps
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