using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponMissile : MonoBehaviour
{
    public Transform launchPoint;
    public Transform launchFlash;
    public GameObject missilePrefab;
    public GameObject launchIndicator;

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Shoot();
        }
    }

    void Shoot ()
    {
        // shooting logic (Prefab Shooting)
        Instantiate(missilePrefab, launchPoint.position, launchPoint.rotation);
        Instantiate(launchIndicator, launchFlash.position, launchFlash.rotation);
    }

}
