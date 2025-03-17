using UnityEngine;
using System.Collections; // Added this for IEnumerator

public class AllPurposeDoorOpen : MonoBehaviour
{
    public enum DoorType
    {
        VerticalLiftDoor,
        DoubleDoor,
        SlidingDoor
    }

    [Header("Door Settings")]
    [Tooltip("Select the type of door this script controls")]
    public DoorType doorType;

    [Tooltip("Distance the VerticalLiftDoor moves up on the Y axis")]
    public float verticalLiftDistance = 10f;

    [Tooltip("Distance each DoubleDoor moves apart on the X axis (left and right)")]
    public float doubleDoorDistance = 3.5f;

    [Tooltip("Distance the SlidingDoor moves left on the X axis")]
    public float slidingDoorDistance = 5f;

    [Tooltip("Speed of door movement in units per second")]
    public float moveSpeed = 2f;

    [Tooltip("If true, door closes automatically after a delay")]
    public bool autoClose = false;

    [Tooltip("Delay in seconds before auto-closing (if enabled)")]
    public float autoCloseDelay = 5f;

    [Tooltip("Second door object for DoubleDoor type (moves right)")]
    public GameObject secondDoor; // Only used for DoubleDoor

    private bool isPowered = false;
    private bool isOpen = false;
    private Vector3 closedPosition; // Starting position of the door
    private Vector3 openPosition;   // Target position when open
    private Vector3 secondClosedPosition; // For DoubleDoor second door
    private Vector3 secondOpenPosition;   // For DoubleDoor second door

    void Start()
    {
        // Store initial positions
        closedPosition = transform.position;
        CalculateOpenPosition();

        // Disable script until powered
        enabled = false;

        // Validate DoubleDoor setup
        if (doorType == DoorType.DoubleDoor && secondDoor == null)
        {
            Debug.LogWarning($"{gameObject.name}: DoubleDoor selected but secondDoor not assigned!");
        }
    }

    // Called by PowerGeneratorTrigger when power is turned on
    public void EnableDoor()
    {
        isPowered = true;
        enabled = true;
        Debug.Log($"{gameObject.name}: Door powered and enabled.");
    }

    // Triggered by player interaction (e.g., pressing "E" near the door)
    public void ToggleDoor()
    {
        if (!isPowered)
        {
            Debug.Log($"{gameObject.name}: Cannot open door - no power!");
            return;
        }

        if (isOpen)
        {
            StartCoroutine(CloseDoor());
        }
        else
        {
            StartCoroutine(OpenDoor());
        }
    }

    private void CalculateOpenPosition()
    {
        switch (doorType)
        {
            case DoorType.VerticalLiftDoor:
                openPosition = closedPosition + new Vector3(0f, verticalLiftDistance, 0f);
                break;
            case DoorType.DoubleDoor:
                openPosition = closedPosition + new Vector3(-doubleDoorDistance, 0f, 0f); // Left door
                if (secondDoor != null)
                {
                    secondClosedPosition = secondDoor.transform.position;
                    secondOpenPosition = secondClosedPosition + new Vector3(doubleDoorDistance, 0f, 0f); // Right door
                }
                break;
            case DoorType.SlidingDoor:
                openPosition = closedPosition + new Vector3(-slidingDoorDistance, 0f, 0f); // Left slide
                break;
        }
    }

    private IEnumerator OpenDoor()
    {
        Debug.Log($"{gameObject.name}: Opening door...");
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = openPosition;

        if (doorType == DoorType.DoubleDoor && secondDoor != null)
        {
            Vector3 secondStartPos = secondDoor.transform.position;
            Vector3 secondTargetPos = secondOpenPosition;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
                secondDoor.transform.position = Vector3.Lerp(secondStartPos, secondTargetPos, elapsedTime);
                yield return null;
            }

            transform.position = targetPos;
            secondDoor.transform.position = secondTargetPos;
        }
        else
        {
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
                yield return null;
            }
            transform.position = targetPos;
        }

        isOpen = true;
        Debug.Log($"{gameObject.name}: Door opened.");

        if (autoClose)
        {
            yield return new WaitForSeconds(autoCloseDelay);
            StartCoroutine(CloseDoor());
        }
    }

    private IEnumerator CloseDoor()
    {
        Debug.Log($"{gameObject.name}: Closing door...");
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = closedPosition;

        if (doorType == DoorType.DoubleDoor && secondDoor != null)
        {
            Vector3 secondStartPos = secondDoor.transform.position;
            Vector3 secondTargetPos = secondClosedPosition;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
                secondDoor.transform.position = Vector3.Lerp(secondStartPos, secondTargetPos, elapsedTime);
                yield return null;
            }

            transform.position = targetPos;
            secondDoor.transform.position = secondTargetPos;
        }
        else
        {
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
                yield return null;
            }
            transform.position = targetPos;
        }

        isOpen = false;
        Debug.Log($"{gameObject.name}: Door closed.");
    }
}