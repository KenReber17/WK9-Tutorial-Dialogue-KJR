using UnityEngine;

public class GlobalInputHandler : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public ControlSwitch controlSwitch;

    void Awake()
    {
        // Try to find the components if they are not set in the inspector
        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement == null) Debug.LogError("PlayerMovement not found in scene!");
        }

        if (controlSwitch == null)
        {
            controlSwitch = FindObjectOfType<ControlSwitch>();
            if (controlSwitch == null) Debug.LogError("ControlSwitch not found in scene!");
        }
    }

    void Update()
    {
        if (playerMovement == null || controlSwitch == null)
        {
            Debug.LogError("PlayerMovement or ControlSwitch reference not set in GlobalInputHandler!");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q) && playerMovement.controllingDrone)
        {
            playerMovement.controllingDrone = false;
            playerMovement.enabled = true;
            if (controlSwitch != null)
            {
                controlSwitch.SetDroneControlled(false); // Turn the light back to red
            }
            Debug.Log("Exited drone control via global handler.");
        }
        // 'E' key should be handled in PlayerMovement to prevent game from quitting
    }
}