using UnityEngine;

public class BaseTower : Troop
{
    private EnemyTower enemyTower; //if this base also has an EnemyTower, forward damage to it

    protected override void Awake()
    {
        base.Awake();
        enemyTower = GetComponent<EnemyTower>(); //check if this base has an EnemyTower on it
    }

    protected override void Update()
    {
        //base doesn't target/atk, projectiles are a diff story
    }

    protected override void FixedUpdate()
    {
        //base doesn't move
    }

    public override void TakeDamage(float damage)
    {
        //if this base has an EnemyTower, forward damage there so one health pool is used
        if (enemyTower != null)
        {
            enemyTower.TakeDamage(damage);
            return;
        }

        //otherwise handle damage normally through Troop
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        Debug.Log(gameObject.name + " base destroyed");
        Destroy(gameObject); //GameEndChecker detects the null and triggers win/lose
    }
}