using UnityEngine;

public class PoolBubbleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] bubbleParticleSystems; // Array for all six particle systems
    [SerializeField] private Light poolLight;                        // The pool light
    [SerializeField] private float interactionDistance = 2f;        // How close the player must be to toggle
    [SerializeField] private Transform playerTransform;             // Reference to the player's transform
    [SerializeField] private Renderer switchRenderer;               // The renderer for the switch object
    [SerializeField] private float switchTravelDistance = 1f;       // Distance the switch moves along X-axis

    private bool isBubblesActive = false; // Tracks if bubbles and light are on
    private Vector3 offPosition;          // Original position (off state)
    private Vector3 onPosition;           // Position when turned on (variable distance right)

    void Start()
    {
        // Store initial positions
        offPosition = transform.position;                    // Starting position (off)
        onPosition = offPosition + Vector3.right * switchTravelDistance; // Calculate on position based on distance

        // Ensure all particle systems and light start off
        if (bubbleParticleSystems != null && bubbleParticleSystems.Length > 0)
        {
            foreach (ParticleSystem ps in bubbleParticleSystems)
            {
                if (ps != null)
                    ps.Stop();
            }
        }
        if (poolLight != null)
            poolLight.enabled = false; // Light starts off

        // Set initial switch color to dark grey (off)
        if (switchRenderer != null)
            switchRenderer.material.color = Color.grey; // Dark grey when off
    }

    void Update()
    {
        // Check if player is within interaction distance
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= interactionDistance)
        {
            // Activate with "E" key
            if (Input.GetKeyDown(KeyCode.E) && !isBubblesActive)
            {
                ToggleBubbles(true);
            }
            // Deactivate with "Q" key
            else if (Input.GetKeyDown(KeyCode.Q) && isBubblesActive)
            {
                ToggleBubbles(false);
            }
        }
    }

    private void ToggleBubbles(bool activate)
    {
        isBubblesActive = activate;

        // Toggle all bubble particle systems
        if (bubbleParticleSystems != null && bubbleParticleSystems.Length > 0)
        {
            foreach (ParticleSystem ps in bubbleParticleSystems)
            {
                if (ps != null)
                {
                    if (activate)
                        ps.Play();
                    else
                        ps.Stop();
                }
            }
        }

        // Toggle pool light
        if (poolLight != null)
        {
            poolLight.enabled = activate; // Turn light on or off
            if (activate)
                poolLight.color = Color.green; // Set to green when turned on
        }

        // Move the switch and change its color
        if (activate)
        {
            transform.position = onPosition; // Move to the on position
            if (switchRenderer != null)
                switchRenderer.material.color = Color.green; // Green when on
        }
        else
        {
            transform.position = offPosition; // Move back to original position
            if (switchRenderer != null)
                switchRenderer.material.color = Color.grey; // Dark grey when off
        }
    }

    // Visualize interaction range in the Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}