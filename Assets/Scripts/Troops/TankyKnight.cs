using UnityEngine;

public class TankyKnight : Troop
{
    [Header("Tanky Knight Stats")]
    [SerializeField] private float damage = 5f; //high dmg, low atk spd

    protected override void Attack()
    {
        base.Attack(); //updates attack cooldown timer and logs attack

        if (currentEnemy == null)
            return;

        currentEnemy.TakeDamage(damage); //deal damage to current enemy
    }
}


