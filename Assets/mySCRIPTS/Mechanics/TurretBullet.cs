using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 50f;

    private Vector2 firingDirection;
    private Transform target;
    private int damage;

    void Start()
    {
        // Get the direction based on the turret's current rotation
        if (rb != null)
        {
            firingDirection = transform.right; // Assuming 'right' is forward for your turret setup
            rb.linearVelocity = firingDirection * bulletSpeed;
        }

        StartCoroutine(SelfDestruct());
    }

    public void SetTarget(Transform _target)
    {
        // Store the target or perform any setup needed with the target
        target = _target;
    }

    public void SetDamage(int _damage)
    {
        damage = _damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        
        Destroy(gameObject); // Destroy bullet on trigger
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}