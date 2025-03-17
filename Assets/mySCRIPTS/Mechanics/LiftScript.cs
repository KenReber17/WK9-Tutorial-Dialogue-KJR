using UnityEngine;

public class LiftScript : MonoBehaviour
{
    public float maxHeight = 82f;
    public float minHeight = 45f;

    public float speedUp = 0.5f;
    public float speedDown = 0.3f;

    public bool movingUp = false;

    private Vector2 velocity = Vector2.zero; // Used for SmoothDamp
    public float smoothTime = 3f; // Time it takes to reach target speed

    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public Vector2 GetPosition()
    {
        return rb.position;
    }

    void FixedUpdate()
    {
        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = currentPosition;

        if (movingUp && currentPosition.y < maxHeight)
        {
            targetPosition.y = maxHeight;
        }
        else if (!movingUp && currentPosition.y > minHeight)
        {
            targetPosition.y = minHeight;
        }

        // Use Vector2.SmoothDamp for smooth movement
        Vector2 newPosition = Vector2.SmoothDamp(currentPosition, targetPosition, ref velocity, smoothTime, movingUp ? speedUp : speedDown);

        // Ensure the elevator doesn't exceed its limits
        newPosition.y = Mathf.Clamp(newPosition.y, minHeight, maxHeight);

        rb.MovePosition(newPosition);
    }

    // Method to toggle the lift direction
    public void ToggleLift()
    {
        movingUp = !movingUp;
    }

    public bool IsCurrentlyMoving()
    {
        return velocity.sqrMagnitude > 0.01f || 
               (movingUp && rb.position.y < maxHeight - 0.1f) || 
               (!movingUp && rb.position.y > minHeight + 0.1f);
    }
}