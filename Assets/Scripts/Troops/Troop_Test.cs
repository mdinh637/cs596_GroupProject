using UnityEngine;

public class Troop_Test : Troop
{
    [Header("Testing Troop Logic or sum")]
    [SerializeField] private float damage = 2f; //damage default troop deals

    protected override void Attack()
    {
        base.Attack(); //call base attack to update last attack time

        if (currentEnemy == null)
            return;

        currentEnemy.TakeDamage(damage); //deal damage to current enemy
    }
}
