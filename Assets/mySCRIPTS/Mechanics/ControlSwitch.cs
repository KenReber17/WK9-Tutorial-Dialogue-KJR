using UnityEngine;
using TMPro; // Make sure to include this for TextMeshProUGUI

public class ControlSwitch : MonoBehaviour
{
    public DroneController2D droneController; // Reference to the drone's controller script
    private bool isDroneControlled = false; // Flag to indicate if drone is being controlled
    private SpriteRenderer spriteRenderer;
    public TextMeshProUGUI messageText; // Ensure this is assigned in the inspector
    private bool playerInRange = false; // Added missing variable declaration
    public string displayMessage = "[E] = CONTROL DRONE [Q] = QUIT";

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on ControlSwitch GameObject!");
        }

        // Check if messageText is assigned
        if (messageText == null)
        {
            Debug.LogWarning("messageText is not assigned in Awake. Trying to find it now.");
            messageText = FindObjectOfType<TextMeshProUGUI>();
            if (messageText == null)
            {
                Debug.LogError("Failed to find TextMeshProUGUI in scene at startup!");
            }
            else
            {
                Debug.LogWarning("Found TextMeshProUGUI after initial check. Ensure it's set in the Inspector.");
            }
        }

        // Initially hide the message text
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    // Optional: Visual feedback
    void Update()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isDroneControlled ? Color.green : Color.red;
        }

        // Check for 'E' key press to hide message
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && messageText != null)
        {
            HideMessage();
        }
    }

    public void SetDroneControlled(bool controlled)
    {
        isDroneControlled = controlled;
        // No need to change color here since Update handles it
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered drone control area.");
            playerInRange = true; // Set to true when player enters
            if (messageText != null)
            {
                messageText.gameObject.SetActive(true); // This shows text
                ShowMessage();
            }
            else
            {
                Debug.LogWarning("messageText is not assigned!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player exited drone control area.");
            playerInRange = false; // Reset when player exits
            if (messageText != null)
            {
                HideMessage();
            }
            //else
            //{
                // Add Exit message
                //Debug.LogWarning("messageText is not assigned!");
            //}
        }
    }

    void ShowMessage()
    {
        if (messageText != null)
        {
            messageText.text = displayMessage;
        }
    }

    void HideMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false); // This hides text
        }
    }
}