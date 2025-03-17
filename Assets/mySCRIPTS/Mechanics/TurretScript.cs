using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TurretScript : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask layerMask; // Assume this now includes the player's layer
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private LineRenderer constantLaser; // New LineRenderer for constant laser
    [SerializeField] private Transform laserOriginPoint; // New Transform for laser's start position

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 20f;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float bpsFireRate = 3f;
    public int damage = 10; // Damage the turret deals

    private Transform target;
    private float timeUntilFire;

    // Sounds
    public AudioSource turretFire;

    void Start ()
    {
        turretFire = GetComponent<AudioSource>();
        if (constantLaser != null)
        {
            constantLaser.positionCount = 2; // The line has a start and an end
            constantLaser.enabled = false; // Start with laser off
        }
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
        }

        UpdateConstantLaser();
        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            target = null;
        } 
        else
        {
            timeUntilFire += Time.deltaTime;
            if (timeUntilFire >= 1f / bpsFireRate)
            {
                turretFire.Play();
                Shoot();
                timeUntilFire = 0f;
            }
        }
    }

    private void Shoot()
    {
        // Instantiate bullet only once
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
        TurretBullet bullet = bulletObj.GetComponent<TurretBullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target);
            bullet.SetDamage(damage); // Set the damage on the bullet
        }
        else
        {
            Debug.LogWarning("Bullet instantiated but TurretBullet component not found.");
        }
    }

    private void FindTarget()
    {
        // Use OverlapCircleAll for performance, then filter results with raycast
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, targetingRange, layerMask);
        
        if (hits.Length > 0)
        {
            // Sort by distance, then by angle for a more strategic targeting
            System.Array.Sort(hits, (a, b) => 
            {
                float distA = Vector2.Distance(transform.position, a.transform.position);
                float distB = Vector2.Distance(transform.position, b.transform.position);
                
                if (Mathf.Approximately(distA, distB))
                {
                    // If distances are similar, target by angle to avoid rapid switching
                    float angleA = Vector2.SignedAngle(Vector2.up, a.transform.position - transform.position);
                    float angleB = Vector2.SignedAngle(Vector2.up, b.transform.position - transform.position);
                    return angleA.CompareTo(angleB);
                }
                return distA.CompareTo(distB);
            });

            // Check if there's a clear line of sight to the target
            for (int i = 0; i < hits.Length; i++)
            {
                Vector2 direction = hits[i].transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, targetingRange, layerMask);
                if (hit.collider != null && hit.collider.gameObject == hits[i].gameObject)
                {
                    target = hits[i].transform;
                    return;
                }
            }
            target = null; // No valid target found
        }
        else
        {
            target = null;
        }
    }

    private bool CheckTargetIsInRange()
    {
        if (target == null) return false;
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
    }

    private void RotateTowardsTarget()
    {
        if (target == null) return;

        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg - 91f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void UpdateConstantLaser()
    {
        if (constantLaser != null && laserOriginPoint != null)
        {
            constantLaser.enabled = target != null && CheckTargetIsInRange(); // Only show laser when target in range
            if (constantLaser.enabled)
            {
                constantLaser.SetPosition(0, laserOriginPoint.position); // Start of the constant laser from new point
                Vector2 endPoint = (Vector2)laserOriginPoint.position + (Vector2)(laserOriginPoint.right * targetingRange); // Extend forward from new point
                constantLaser.SetPosition(1, endPoint);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
}