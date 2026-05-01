using UnityEngine;

public class BasicKnight : Troop
{
    [Header("Basic Knight Stats")]
    [SerializeField] private float damage = 2f; //balanced dmg for first troop

    protected override void Attack()
    {
        base.Attack(); //updates atk cd timer and log attacks

        if (currentEnemy == null)
            return;

        currentEnemy.TakeDamage(damage); //deal damage to enemy troop
    }
}