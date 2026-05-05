using UnityEngine;

public class Rogue : Troop
{
    [Header("Rogue Stats")]
    [SerializeField] private float damage = 2f; //rogue normal atk stat
    [SerializeField] private float critDamage = 5f; //rogue first atk after dash dmg
    [SerializeField] private float dashForce = 3f; //force for rogue dash
    [SerializeField] private float dashUpForce = 5f; //upward force for rogue dash
    [SerializeField] private float behindTargetDistance = 2f; //distance rogue tries to land behind enemy
    [SerializeField] private float dashRecoverTime = 0.2f; //time before rogue resumes movement
    [SerializeField] private float critRecoverTime = 3f; //pause after rogue crit atk

    [Header("Animations")]
    [SerializeField] private Animator animator; //animator attached to rogue
    [SerializeField] private string attackTrigger = "Attack"; //normal atk trigger in rogue anim
    [SerializeField] private string critAttackTrigger = "CritAttack"; //crit atk trigger in rogue anim
    [SerializeField] private string dashTrigger = "Dash"; //dash/jump trigger in rogue anim
    [SerializeField] private string movingBool = "Moving"; //bool for movement anim

    [Header("Visuals")]
    [SerializeField] private float transparentAlpha = 0.5f; //transparent effect for untargetable state

    private bool isDashing; //whether rogue is currently dashing
    private bool hasDashCrit; //whether next atk should crit
    private bool isRecoveringFromCrit; //whether rogue is waiting after crit

    private Renderer[] renderers; //all renderers on rogue
    private Color[] originalColors; //store original colors

    protected override void Awake()
    {
        base.Awake();

        isTargetable = false; //rogue starts untargetable

        renderers = GetComponentsInChildren<Renderer>(); //get all renderers on rogue
        originalColors = new Color[renderers.Length]; //store original colors

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_BaseColor"))
            {
                originalColors[i] = renderers[i].material.GetColor("_BaseColor"); //save original urp color
            }
            else if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.GetColor("_Color"); //fallback for older shaders
            }
        }

        SetTransparency(true); //start transparent while untargetable
    }

    protected override void Update()
    {
        if (isRecoveringFromCrit)
        {
            if (animator != null)
                animator.SetBool(movingBool, false); //idle during crit recovery

            return; //skip base update so rogue cannot atk during crit pause
        }

        //call base update (handles movement, targeting, atking)
        base.Update();

        //if rogue has no target and is not dashing, make it untargetable again
        if (currentEnemy == null && isDashing == false)
        {
            isTargetable = false; //go intangible again when no target
            SetTransparency(true); //make rogue transparent again
            hasDashCrit = false; //reset crit state when no target
        }

        //dash once when rogue first finds a target while untargetable
        if (currentEnemy != null && isDashing == false && hasDashCrit == false && isTargetable == false)
        {
            DashBehindTarget(); //jump behind target before first atk
        }

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

    private void DashBehindTarget()
    {
        if (currentEnemy == null || rb == null)
            return;

        isDashing = true; //prevents repeated dash checks
        hasDashCrit = true; //next atk will crit

        if (animator != null)
        {
            animator.SetTrigger(dashTrigger); //play dash/jump animation
        }

        Vector3 behindPosition = currentEnemy.transform.position - currentEnemy.transform.forward * behindTargetDistance; //point behind enemy
        Vector3 dashDirection = behindPosition - transform.position; //direction to point behind enemy
        dashDirection.y = 0;

        if (dashDirection != Vector3.zero)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); //clear horizontal movement first

            Vector3 dashForceDirection = dashDirection.normalized * dashForce; //horizontal dash force
            dashForceDirection.y = dashUpForce; //add upward force for arc

            rb.AddForce(dashForceDirection, ForceMode.Impulse); //launch rogue towards a position behind the enemy
        }

        Invoke(nameof(RecoverFromDash), dashRecoverTime); //resume movement after dash
    }

    private void RecoverFromDash()
    {
        isDashing = false; //allow normal movement again
    }

    protected override void Attack()
    {
        base.Attack(); //updates atk cd timer and log attacks

        isTargetable = true; //rogue becomes targetable after attacking
        SetTransparency(false); //show rogue when targetable

        if (currentEnemy == null)
            return;

        if (hasDashCrit)
        {
            if (animator != null)
            {
                animator.SetTrigger(critAttackTrigger); //play crit atk animation
            }

            currentEnemy.TakeDamage(critDamage); //deal crit dmg after dash
            hasDashCrit = false; //crit only happens once after dash
            StartCoroutine(CritRecover()); //pause before regular atks
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger(attackTrigger); //play normal atk animation
            }

            currentEnemy.TakeDamage(damage); //deal normal dmg
        }
    }

    private void SetTransparency(bool transparent)
    {
        if (renderers == null || originalColors == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            Material mat = renderers[i].material; //get renderer material
            Color color = originalColors[i]; //use original rogue colors

            if (transparent)
            {
                color.a = transparentAlpha; //make rogue transparent
            }
            else
            {
                color.a = 1f; //make rogue fully visible again
            }

            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", color); //apply updated urp color
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", color); //fallback for older shaders
            }
        }
    }

    private System.Collections.IEnumerator CritRecover()
    {
        isRecoveringFromCrit = true; //start crit recovery pause

        yield return new WaitForSeconds(critRecoverTime);

        isRecoveringFromCrit = false; //allow regular atks again

        if (currentEnemy == null)
        {
            isTargetable = false; //go intangible again if no target after pause
            SetTransparency(true); //make rogue transparent again
        }
    }
}