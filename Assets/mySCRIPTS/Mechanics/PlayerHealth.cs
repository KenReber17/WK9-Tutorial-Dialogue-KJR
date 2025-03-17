using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;     // Maximum health of the player
    public int currentHealth;       // Current health of the player
    public Slider healthSlider;     // UI Slider to represent health visually
    public Image damageImage;       // Image that flashes when taking damage
    public float flashSpeed = 5f;   // Speed of the flash effect
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f); // Color to flash when taking damage

    // Private reference to the Animator component for death animation
    private Animator anim;
    // Boolean to check if the player is dead
    private bool isDead;

    void Awake()
    {
        // Get the Animator component
        anim = GetComponent<Animator>();
        // Initialize current health to max health
        currentHealth = maxHealth;
        // Set the health slider to full
        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth;
            healthSlider.maxValue = maxHealth;
        }
        isDead = false;
    }

    // Public method to apply damage to the player
    public void TakeDamage(int amount)
    {
        if (isDead) return; // Don't apply damage if already dead

        currentHealth -= amount;

        // Clamp health to ensure it doesn't go below 0 or above maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the slider's value
        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth;
        }

        // Flash the screen
        if (damageImage != null) 
        {
            damageImage.color = flashColour;
        }

        if (currentHealth <= 0 && !isDead)
        {
            StartCoroutine(DeathSequence());
        }
    }

    // Public method to heal the player
    public void Heal(int amount)
    {
        if (isDead) return; // Don't heal if dead

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the slider's value
        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth;
        }
    }

    // Coroutine for handling the death sequence
    IEnumerator DeathSequence()
    {
        isDead = true;
        // Trigger any death animation or effect
        if (anim != null) 
        {
            anim.SetTrigger("Die");
        }

        // Wait for the animation to finish or for a certain time
        yield return new WaitForSeconds(2f);

        // Game over logic here, e.g., load game over screen
        Debug.Log("Player has died!");
        //SceneManager.LoadScene("GameOver");
    }

    void Update()
    {
        // Gradually fade out the damage image if it's not fully transparent
        if (damageImage != null)
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }
}