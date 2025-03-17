using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LimitProjectile : MonoBehaviour
{
    public TMP_Text AmmoText;

    public GameObject projectile;
    public Transform shootPoint;

    [SerializeField] private int roundsToAdd = 20;
    [SerializeField] private int maxRounds = 99;

    private int currentAmmo; // This will track the actual ammo count

    // Sounds
    public AudioSource cannonFire;
    public AudioSource ammoPickup;
    public AudioSource ammoShell;
    public AudioSource miniGunFire;

    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 3)
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

        // Initialize current ammo
        currentAmmo = 0;
        UpdateAmmoText();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (projectile == null)
            {
                Debug.Log("No projectile Set");
            }
            else
            {
                if (currentAmmo > 0)
                {
                    cannonFire.Play();
                    ammoShell.Play();
                    Instantiate(projectile, shootPoint.position, transform.rotation);
                    currentAmmo--;
                    UpdateAmmoText();
                    StartCoroutine(PlayShellCasingSoundWithDelay()); // Start the coroutine
                }
            }
        }
    }

    private IEnumerator PlayShellCasingSoundWithDelay()
    {
        // Wait for half a second
        yield return new WaitForSeconds(0.5f);
        // Play the shell casing sound
        ammoShell.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectible"))
        {
            ammoPickup.Play();
            
            // Increase ammo, but do not exceed maxRounds
            currentAmmo = Mathf.Min(currentAmmo + roundsToAdd, maxRounds);
            Debug.Log("Ammo picked up! Current rounds: " + currentAmmo);
            UpdateAmmoText();
            Destroy(collision.gameObject);
        }
    }

    private void UpdateAmmoText()
    {
        AmmoText.text = "Rounds: " + currentAmmo.ToString();
    }
}