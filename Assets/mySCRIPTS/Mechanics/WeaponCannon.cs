using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCannon : MonoBehaviour
{
    public Transform firePoint;
    public Transform firePoint2;
    public Transform flashUpper;
    public Transform flashLower;
    public GameObject bulletPrefab;
    public GameObject flashPoint;
    // public int damage = 40;
    // public GameObject impactEffect;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot ()
    {
        // shooting logic (Prefab Shooting)
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Instantiate(bulletPrefab, firePoint2.position, firePoint2.rotation);
        Instantiate(flashPoint, flashUpper.position, flashUpper.rotation);
        Instantiate(flashPoint, flashLower.position, flashLower.rotation);

        // shooting logic (Raycast shooting w/o bullet prefab)
        // RaycastHit2D hitInfo = Physicss2D.Raycast(firePoint.position, firePoint.right);
        // if (hitInfo)
        // {
        //      Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
        //      if (enemy != null)
        //      {
        //          enemy.TakeDamage(damage);
        //      }
        //      Instantiate(impactEffect, hitInfo.point, Quaternion.identity);
        // }
    }
}
