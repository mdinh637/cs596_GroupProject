using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Damage")]
    [SerializeField] private float damage = 5f; //dmg dealt to enemies only
    [SerializeField] private LayerMask enemyLayers; //to correctly set targeting to targets with enemy layer
    [SerializeField] private string lavaLayerName = "Lava"; //getting name of lava layer to handle proper interactions with it

    private void Start()
    {
        Destroy(gameObject, 5); //cleanup in case the projectile doesn't hit anything
    }

    private void OnCollisionEnter(Collision collision)
    {
        //only dmg enemies on enemy layer
        if (((1 << collision.gameObject.layer) & enemyLayers) == 0)
            return;

        Troop enemyTroop = collision.gameObject.GetComponentInParent<Troop>(); //get troop hit by projectile

        if (enemyTroop == null)
            return;

        enemyTroop.TakeDamage(damage); //deal dmg to enemy troop

        Destroy(gameObject); //destroy projectile after hitting enemy
    }

    private void OnTriggerEnter(Collider other)
    {
        //destroy projectile if it touches lava trigger
        if (other.gameObject.layer == LayerMask.NameToLayer(lavaLayerName))
        {
            Destroy(gameObject);
        }
    }
}
