using UnityEngine;
using TMPro;

public class ElevatorSwitch : MonoBehaviour
{
    public string displayMessage = "PRESS E to Activate";
    public LiftScript lift;
    public SpriteRenderer switchSprite; // Reference to the sprite that changes color

    private bool playerInRange = false;

    // Reference to UI Text element where the message will be displayed
    public TMPro.TextMeshProUGUI messageText;

    void Start()
    {
        messageText.gameObject.SetActive(false); // This hides the text at the start
        if (switchSprite == null)
        {
            switchSprite = GetComponent<SpriteRenderer>(); // If not assigned in editor, try to get from this object
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            messageText.gameObject.SetActive(true); // This shows text
            ShowMessage();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideMessage();
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
        }
    }

    void Update()
    {
        // Check for 'E' key press
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLift();
        }

        // Update the color of the switch based on the elevator's movement
        if (lift != null && switchSprite != null)
        {
            if (lift.IsCurrentlyMoving())
            {
                switchSprite.color = Color.yellow; // Yellow when moving
            }
            else
            {
                switchSprite.color = Color.green; // Green when stationary
            }
        }
    }

    // Method to toggle or stop the lift
    public void ToggleLift()
    {
        if (lift != null)
        {
            lift.movingUp = !lift.movingUp; // Toggle the direction of the lift
        }
    }
}