using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMiniGun : MonoBehaviour
{
    // The origin point for the raycasts
    public Transform raycastOrigin;
    
    // The length of the raycast
    public float raycastDistance = 100f;

    // Layer mask for what the raycast should hit
    public LayerMask hitMask;

    // The angles for the three raycasts
    public float[] angles = new float[] { 0f, 15f, -15f }; // Central, right, and left rays

    void Update()
    {
        // Check if the fire button is held down
        if (Input.GetButton("Fire1"))
        {
            for (int i = 0; i < angles.Length; i++)
            {
                // Calculate the direction for each raycast
                Vector3 direction = Quaternion.Euler(0, angles[i], 0) * transform.forward;

                // Perform the raycast
                RaycastHit hit;
                if (Physics.Raycast(raycastOrigin.position, direction, out hit, raycastDistance, hitMask))
                {
                    // Here you can do something when the ray hits something
                    Debug.DrawLine(raycastOrigin.position, hit.point, Color.red, 0.1f);
                    
                    // Example: Log the name of the object hit
                    Debug.Log("Hit " + hit.collider.gameObject.name);
                }
                else
                {
                    // If nothing was hit, draw a line to the maximum distance
                    Debug.DrawLine(raycastOrigin.position, raycastOrigin.position + direction * raycastDistance, Color.yellow, 0.1f);
                }
            }
        }
    }
}
