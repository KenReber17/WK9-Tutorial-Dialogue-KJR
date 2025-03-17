using UnityEngine;
using System.Collections;

public class Diamond : MonoBehaviour
{
    public AudioClip diamondGrabClip;  // Assign this in the inspector

    void Start()
    {
        // No need for AudioSource here since we're using AudioManager
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has the "Player" tag
        if (collision.CompareTag("Player"))
        {
            // Play the sound through the AudioManager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(diamondGrabClip);
            }
            else
            {
                Debug.LogError("AudioManager Instance is not found!");
            }

            StartCoroutine(CollectDiamond());
        }
    }

    IEnumerator CollectDiamond()
    {
        // Add points to the score immediately
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnDiamondPickup();
        }

        // Wait for 0.5 seconds before destroying the object
        yield return new WaitForSeconds(0.1f);

        // Destroy the diamond after the wait
        Destroy(gameObject);
    }
}