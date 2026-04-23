using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public Rigidbody projectile;
    public Transform launchPoint;
    public float speed = 1f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (projectile == null)
            {
                Debug.LogError("FireProjectile: Projectile prefab is not assigned.");
                return;
            }

            Transform spawn = launchPoint != null ? launchPoint : transform;

            Rigidbody p = Instantiate(projectile, spawn.position, spawn.rotation);
            p.linearVelocity = spawn.forward * speed;

            Debug.Log($"Fired projectile from {spawn.name} at speed {speed}");
        }
    }
}
