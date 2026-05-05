using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    private Troop target; //target to hit with arrow
    private float damage;
    private float speed;

    [SerializeField] private float lifeTime = 5f; //destroy projectile if it doesn't hit a target after a set amount of time

    public void SetTarget(Troop newTarget, float newDamage, float newSpeed)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;

        Destroy(gameObject, lifeTime); //cleanup if never hits
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized; //move toward target

        transform.position += direction * speed * Time.deltaTime;

        //rotate arrow to face movement
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Troop hitTroop = other.GetComponentInParent<Troop>();

        if (hitTroop == null)
            return;

        //this is to ensure the arrow only hits the targeted unit, not an ally/enemy troop in front of target
        if (hitTroop != target)
            return;

        hitTroop.TakeDamage(damage);

        Destroy(gameObject);
    }
}
