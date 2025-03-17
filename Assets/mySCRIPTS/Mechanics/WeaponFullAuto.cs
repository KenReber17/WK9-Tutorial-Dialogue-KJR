using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFullAuto : MonoBehaviour
{
    public Transform firePoint;
    public int damage = 10;
    public GameObject impactEffect;
    public LineRenderer lineRenderer;

    public Transform flashStart;
    public GameObject flashPoint;
    public float impactForce = 50f;

    private float fireRate = 9f;
    private float nextTimeToFire = 0f;

    // Sounds
    public AudioSource cannonFire;
    public AudioSource ammoPickup;
    public AudioSource ammoShell;
    public AudioSource miniGunFire;

    // Layer mask to control what the raycast can hit
    public LayerMask hitLayers; // Layers you want to hit

    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        hitLayers = LayerMask.GetMask("Enemy", "Ground", "Hit Layers");
        if (audioSources.Length >= 4)
        {
            cannonFire = audioSources[0];
            ammoPickup = audioSources[1];
            ammoShell = audioSources[2];
            miniGunFire = audioSources[3];
        }
        else
        {
            Debug.LogError("Not enough audio sources attached!");
        }
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            miniGunFire.Play();
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }   
    }

    void Shoot()
    {
        Instantiate(flashPoint, flashStart.position, flashStart.rotation);

        // Use LayerMask to control what the raycast can hit
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right, Mathf.Infinity, hitLayers);

        if (hitInfo)
        {
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            if (hitInfo.rigidbody != null)
            {
                hitInfo.rigidbody.AddForce(-hitInfo.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hitInfo.point, Quaternion.identity);
            Destroy(impactGO, 2f);
        }
    }
}