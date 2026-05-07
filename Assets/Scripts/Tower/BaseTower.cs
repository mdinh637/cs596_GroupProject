using UnityEngine;

public class BaseTower : Troop
{
    //cheesing the crap outa the tower logic for ally base and enemy base
    //didn't make a shared dmg interface so gonna rip it from my troop script
    protected override void Update()
    {
        //base doesn't target/atk, projectiles are a diff story
    }

    protected override void FixedUpdate()
    {
        //base doesn't move
    }

    protected override void Die()
    {
        Debug.Log(gameObject.name + " base destroyed");
        Destroy(gameObject); //GameEndChecker detects the null reference and triggers win/lose
    }
}