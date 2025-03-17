using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    public Transform firePoint;
    public int damage = 30;
    public GameObject impactEffect;
    public LineRenderer lineRenderer;

    //private float fireRate = 15f;

    public AudioSource gun_shotLong;
    public Transform flashStart;
    public GameObject flashPoint;

    //private float nextTimeToFire = 0f;


     void Start ()
    {
        gun_shotLong = GetComponent<AudioSource>();
    }

    void Update ()
    {
        //if (Input.GetButtonDown("Fire1"))
        if (Input.GetButtonDown("Fire1")) //&& Time.time >= nextTimeToFire)
        {
            StartCoroutine(Shoot());
            gun_shotLong.Play();
        }
    }
    
    IEnumerator Shoot()
    {
        Instantiate(flashPoint, flashStart.position, flashStart.rotation);

        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right);

        if (hitInfo)
        {
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Instantiate(impactEffect, hitInfo.point, Quaternion.identity);

            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hitInfo.point);
        } else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 50);
        }

        lineRenderer.enabled = true;

        // wait one frame
        //yield return 0;
        // maybe better to use: 
        yield return new WaitForSeconds(0.02f);

        lineRenderer.enabled = false;
    }
}
