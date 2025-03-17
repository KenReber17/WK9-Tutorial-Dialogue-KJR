using System.Collections;
using UnityEngine;

public class DroneController2D : MonoBehaviour
{
    public float maxSpeed = 4f; 
    public float accelerationRate = 5f; 
    public float decelerationRate = 2f; 

    private PlayerMovement playerMovement; 
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 currentVelocity = Vector2.zero; 
    private bool isFacingRight = true; 
    private bool slowingDown = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement script not found in the scene!");
        }
    }

    void Update()
    {
        if (playerMovement != null)
        {
            if (playerMovement.controllingDrone)
            {
                HandleDroneMovement();
                
                // Check for exit input (Q key to start slowing down)
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    playerMovement.controllingDrone = false;
                    slowingDown = true;
                }
            }
            else if (slowingDown) // If slowing down after 'Q' was pressed
            {
                SlowDownAndReset();
            }
            // No need to check for 'E' here as it's handled in PlayerMovement
        }
        else 
        {
            rb.linearVelocity = Vector2.zero;
            currentVelocity = Vector2.zero;
        }

        transform.rotation = Quaternion.identity;
    }

    private void HandleDroneMovement()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        if (moveInput != Vector2.zero)
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, moveInput.normalized * maxSpeed, accelerationRate * Time.deltaTime);
            Flip(moveInput.x); 
        }
        else
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, decelerationRate * Time.deltaTime);
        }

        rb.linearVelocity = currentVelocity;
    }

    private void Flip(float horizontalInput)
    {
        if (horizontalInput != 0 && ((isFacingRight && horizontalInput < 0) || (!isFacingRight && horizontalInput > 0)))
        {
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    private void SlowDownAndReset()
    {
        currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, decelerationRate * Time.deltaTime);
        rb.linearVelocity = currentVelocity;

        if (currentVelocity.magnitude <= 0.1f) // Using a small threshold for 'stopped'
        {
            currentVelocity = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            slowingDown = false; // Reset the slowing down flag
        }
    }
}