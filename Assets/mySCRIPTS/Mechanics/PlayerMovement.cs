using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    public float speed = 6f;
    private float jumpingPower = 25f;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Drone Operation
    public DroneController2D drone;
    public ControlSwitch controlSwitch; // Reference to the control switch

    [SerializeField] 
    public bool controllingDrone = false; // Changed to public for DroneController2D access

    private bool isNearDroneControl = false;

    private void Awake()
    {
        // Ensure components are set up if they weren't in the inspector
        if (rb == null) 
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) Debug.LogError("Rigidbody2D component not found on PlayerMovement!");
        }

        if (groundCheck == null) 
        {
            groundCheck = transform.Find("GroundCheck"); // Assuming 'GroundCheck' is a child of this game object
            if (groundCheck == null) Debug.LogError("GroundCheck Transform not found!");
        }

        if (groundLayer == 0) 
        {
            groundLayer = LayerMask.GetMask("Ground"); // Make sure you have a layer called 'Ground'
            if (groundLayer == 0) Debug.LogError("Ground Layer not set!");
        }

        if (controlSwitch == null) 
        {
            controlSwitch = FindObjectOfType<ControlSwitch>(includeInactive: true);
            if (controlSwitch == null) Debug.LogError("ControlSwitch not found in the scene!");
        }

        // Similar for drone if it's not always required or might be instantiated later
        if (drone == null) 
        {
            drone = FindObjectOfType<DroneController2D>(includeInactive: true);
            if (drone == null) Debug.LogError("DroneController2D not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for null references at the start of Update
        if (rb == null) 
        {
            Debug.LogError("Rigidbody2D is null!");
            return;  // Exit if there's an error to prevent further null reference exceptions
        }
        if (groundCheck == null) 
        {
            Debug.LogError("GroundCheck is null!");
            return;
        }
        if (controlSwitch == null) 
        {
            Debug.LogError("ControlSwitch is null!");
            return;
        }

        // Check for entering drone control with 'E'
        if (Input.GetKeyDown(KeyCode.E) && isNearDroneControl)
        {
            Debug.Log("E key pressed. isNearDroneControl: " + isNearDroneControl);
            controllingDrone = true;
            this.enabled = false;
            if (controlSwitch != null) // Additional check to ensure controlSwitch is not null
            {
                controlSwitch.SetDroneControlled(true); // Set the control switch to green
            }
            // Stop player movement when taking control of drone
            StopPlayerMovement();
            Debug.Log("Now controlling the drone.");
        }

        // Check for exiting drone control with 'Q'
        if (Input.GetKeyDown(KeyCode.Q) && controllingDrone)
        {
            Debug.Log("Q key pressed. controllingDrone: " + controllingDrone);
            controllingDrone = false;
            this.enabled = true;
            if (controlSwitch != null) // Additional check to ensure controlSwitch is not null
            {
                controlSwitch.SetDroneControlled(false); // Reset to red when control is relinquished
            }
            Debug.Log("Back to controlling player.");
        }

        // Normal Player Controls
        if (!controllingDrone)
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpingPower, ForceMode2D.Impulse);
            }

            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }

            Flip();
        }
    }

    private void FixedUpdate()
    {
        // Changed to velocity for consistency with Unity's 2D physics
        if (!controllingDrone && rb != null)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck != null)
            return Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);
        else
        {
            Debug.LogError("GroundCheck is null in IsGrounded!");
            return false;
        }
    }

    private void Flip()
    {
        if (horizontal != 0 && ((isFacingRight && horizontal < 0) || (!isFacingRight && horizontal > 0)))
        {
            isFacingRight = !isFacingRight;
            
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void StopPlayerMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DroneControl"))
        {
            isNearDroneControl = true;
            Debug.Log("Player entered drone control area. isNearDroneControl: " + isNearDroneControl);
        }
        else if (collision.CompareTag("Level0Load"))
        {
            SceneManager.LoadScene(0);
        }
        else if (collision.CompareTag("Level1Load"))
        {
            SceneManager.LoadScene(1);
        }
        else if (collision.CompareTag("Level2Load"))
        {
            SceneManager.LoadScene(2);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DroneControl"))
        {
            isNearDroneControl = false;
            Debug.Log("Player exited drone control area. isNearDroneControl: " + isNearDroneControl);
        }
    }
}