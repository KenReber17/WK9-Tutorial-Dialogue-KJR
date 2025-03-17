using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Reference")]
    [Tooltip("The GameObject with the AllPurposeDoorOpen script to trigger")]
    public GameObject doorObject;

    [Header("Trigger Settings")]
    [Tooltip("If true, requires 'E' press to open; if false, opens automatically on enter")]
    public bool ButtonPress = true;

    private AllPurposeDoorOpen door;
    private bool playerInRange = false;
    private bool isActive = true;

    void Start()
    {
        if (doorObject == null)
        {
            Debug.LogError($"{gameObject.name}: DoorObject not assigned in DoorTrigger!");
            isActive = false;
            return;
        }

        door = doorObject.GetComponent<AllPurposeDoorOpen>();
        if (door == null)
        {
            Debug.LogError($"{gameObject.name}: AllPurposeDoorOpen component missing on {doorObject.name}!");
            isActive = false;
        }
        else
        {
            Debug.Log($"{gameObject.name}: Successfully linked to door {doorObject.name}. ButtonPress set to {ButtonPress}.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        Debug.Log($"{gameObject.name}: Trigger entered by {other.gameObject.name} (Tag: {other.tag})");
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log($"{gameObject.name}: Player in range of door {doorObject.name}.");
            if (!ButtonPress && door != null) // Open automatically if ButtonPress is false
            {
                Debug.Log($"{gameObject.name}: Auto-opening door {doorObject.name} (ButtonPress = false).");
                door.ToggleDoor();
            }
            else if (ButtonPress)
            {
                Debug.Log($"{gameObject.name}: Press 'E' to toggle door {doorObject.name} (ButtonPress = true).");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log($"{gameObject.name}: Player left range of door {doorObject.name}.");
        }
    }

    void Update()
    {
        if (!isActive || door == null || doorObject == null) return;

        // Only check for "E" press if ButtonPress is true
        if (ButtonPress && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"{gameObject.name}: 'E' pressed while in range - Toggling door {doorObject.name}.");
            door.ToggleDoor();
        }
    }

    void OnDisable()
    {
        isActive = false;
        playerInRange = false;
        Debug.Log($"{gameObject.name}: DoorTrigger disabled or destroyed.");
    }

    void OnDestroy()
    {
        isActive = false;
        playerInRange = false;
        Debug.Log($"{gameObject.name}: DoorTrigger destroyed.");
    }
}