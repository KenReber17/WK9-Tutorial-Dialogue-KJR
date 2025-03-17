using UnityEngine;
using System.Collections;

public class PowerGeneratorTrigger : MonoBehaviour
{
    [Header("Power Generator Settings")]
    [Tooltip("The particle system to activate when power is turned on")]
    public ParticleSystem powerParticles;
    [Tooltip("The 2D sprite circle to turn green when power is activated")]
    public SpriteRenderer powerCircle;
    [Tooltip("Delay in seconds before phone ring starts after power activation")]
    public float ringDelay = 10f;

    [Header("Dialogue Settings")]
    [Tooltip("The name of the character to trigger dialogue (set in Inspector)")]
    public string characterName;

    [Header("Interactable Object Settings")]
    [Tooltip("GameObject (e.g., door) to enable when power is turned on")]
    public GameObject interactableObject; // References any interactable object

    [Header("Audio Settings")]
    [Tooltip("AudioSource for the phone ring sound")]
    public AudioSource phoneRing;
    [Tooltip("Delay between phone ring loops in seconds (e.g., 1 for a slower ring)")]
    public float phoneRingInterval = 1f;
    [Tooltip("Optional additional sound for custom use (plays when answering phone call)")]
    public AudioSource optionalSound;
    [Tooltip("AudioSource for generator startup sound 1")]
    public AudioSource generatorSound1;
    [Tooltip("AudioSource for generator startup sound 2")]
    public AudioSource generatorSound2;

    private bool isPoweredOn = false;
    private bool playerInRange = false;
    private bool phoneTriggered = false;
    private bool isRinging = false;
    private Coroutine phoneRingCoroutine;
    private BoxCollider2D boxCollider;
    private const string POWER_STATE_KEY = "PowerGeneratorState";

    void Start()
    {
        PlayerPrefs.DeleteKey(POWER_STATE_KEY);
        isPoweredOn = PlayerPrefs.GetInt(POWER_STATE_KEY, 0) == 1;
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError($"PowerGeneratorTrigger: BoxCollider2D missing on {gameObject.name}!");
        }
        else
        {
            Debug.Log($"PowerGeneratorTrigger: BoxCollider2D found on {gameObject.name}, IsTrigger: {boxCollider.isTrigger}, Enabled: {boxCollider.enabled}");
        }

        if (powerParticles != null)
        {
            if (isPoweredOn)
                powerParticles.Play();
            else
                powerParticles.Stop();
            Debug.Log($"PowerGeneratorTrigger: Initial state loaded for {gameObject.name} - Powered On: {isPoweredOn}, Particles Playing: {powerParticles.isPlaying}");
        }
        else
        {
            Debug.LogWarning($"PowerGeneratorTrigger: Particle System not assigned for {gameObject.name}!");
        }

        if (powerCircle != null)
        {
            powerCircle.color = isPoweredOn ? Color.green : new Color(1f, 1f, 1f, 0f);
            Debug.Log($"PowerGeneratorTrigger: Power circle initial color set for {gameObject.name} - Powered On: {isPoweredOn}");
        }
        else
        {
            Debug.LogWarning($"PowerGeneratorTrigger: Power circle sprite not assigned for {gameObject.name}!");
        }

        if (phoneRing != null && phoneRing.isPlaying)
            phoneRing.Stop();
        if (optionalSound != null && optionalSound.isPlaying)
            optionalSound.Stop();
        if (generatorSound1 != null && generatorSound1.isPlaying)
            generatorSound1.Stop();
        if (generatorSound2 != null && generatorSound2.isPlaying)
            generatorSound2.Stop();

        if (string.IsNullOrEmpty(characterName))
            Debug.LogWarning($"PowerGeneratorTrigger: characterName is not set on {gameObject.name}");

        // Initialize door state based on power
        if (interactableObject != null)
        {
            AllPurposeDoorOpen door = interactableObject.GetComponent<AllPurposeDoorOpen>();
            if (door != null && isPoweredOn)
            {
                door.EnableDoor();
                Debug.Log($"PowerGeneratorTrigger: Initialized {interactableObject.name} with AllPurposeDoorOpen - Powered: {isPoweredOn}");
            }
        }
        else
        {
            Debug.LogWarning($"PowerGeneratorTrigger: interactableObject (e.g., door) not assigned on {gameObject.name}!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"PowerGeneratorTrigger: OnTriggerEnter2D called on {gameObject.name} with {other.gameObject.name}");
        Debug.Log($"PowerGeneratorTrigger: Collision detected with {other.gameObject.name}, Tag: {other.tag}, IsPoweredOn: {isPoweredOn}, Collider Enabled: {boxCollider.enabled}");
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log($"PowerGeneratorTrigger: Player entered trigger area of {gameObject.name} - Press 'E' to activate (Powered On: {isPoweredOn})");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log($"PowerGeneratorTrigger: Player exited trigger area of {gameObject.name}");
        }
    }

    void Update()
    {
        if (playerInRange && !isPoweredOn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"PowerGeneratorTrigger: 'E' pressed, activating power generator {gameObject.name}");
                ActivatePowerGenerator();
            }
        }
        else if (isPoweredOn && isRinging)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"PowerGeneratorTrigger: 'E' pressed again, stopping ring and starting dialogue for {gameObject.name}");

                if (phoneTriggered && phoneRingCoroutine != null)
                {
                    StopCoroutine(phoneRingCoroutine);
                    phoneRingCoroutine = null;
                    if (phoneRing != null && phoneRing.isPlaying)
                        phoneRing.Stop();
                    phoneTriggered = false;
                    isRinging = false;
                    Debug.Log($"PowerGeneratorTrigger: Stopped phone ring for {gameObject.name}");
                }

                // Play optionalSound when "E" is pressed to answer the phone
                if (optionalSound != null)
                {
                    optionalSound.Play();
                    Debug.Log($"PowerGeneratorTrigger: Optional sound playing for {gameObject.name} as phone call is answered");
                }

                if (boxCollider != null)
                {
                    boxCollider.enabled = false;
                    Debug.Log($"PowerGeneratorTrigger: Collider disabled for {gameObject.name}");
                }

                StartCoroutine(TriggerDialogueWithDelay());
            }
        }
    }

    private IEnumerator TriggerDialogueWithDelay()
    {
        yield return new WaitForSeconds(0.1f);

        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        
        if (dialogueManager != null)
        {
            Debug.Log($"PowerGeneratorTrigger: Triggering dialogue for {characterName} after 0.1f delay (phone mode)");
            dialogueManager.TriggerDialogue(characterName, false);
            Debug.Log($"PowerGeneratorTrigger: TriggerDialogue called for {characterName}. Now forcing dialogue start.");
            yield return new WaitForSeconds(0.8f);
            dialogueManager.DisplayNextSentence();
        }
        else
        {
            Debug.LogError("PowerGeneratorTrigger: DialogueManager not found in scene!");
        }
    }

    private void ActivatePowerGenerator()
    {
        isPoweredOn = true;
        PlayerPrefs.SetInt(POWER_STATE_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log($"PowerGeneratorTrigger: Power generator activated, state saved");

        if (powerParticles != null)
        {
            powerParticles.Play();
            Debug.Log($"PowerGeneratorTrigger: Particle system started for {gameObject.name}");
        }
        else
        {
            Debug.LogError($"PowerGeneratorTrigger: Particle system is null for {gameObject.name}!");
        }

        if (powerCircle != null)
        {
            powerCircle.color = Color.green;
            Debug.Log($"PowerGeneratorTrigger: Power circle turned green for {gameObject.name}");
        }
        else
        {
            Debug.LogError($"PowerGeneratorTrigger: Power circle sprite is null for {gameObject.name}!");
        }

        if (generatorSound1 != null)
        {
            generatorSound1.Play();
            Debug.Log($"PowerGeneratorTrigger: Generator sound 1 playing for {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"PowerGeneratorTrigger: GeneratorSound1 not assigned for {gameObject.name}!");
        }

        if (generatorSound2 != null)
        {
            generatorSound2.Play();
            Debug.Log($"PowerGeneratorTrigger: Generator sound 2 playing for {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"PowerGeneratorTrigger: GeneratorSound2 not assigned for {gameObject.name}!");
        }

        if (interactableObject != null)
        {
            AllPurposeDoorOpen door = interactableObject.GetComponent<AllPurposeDoorOpen>();
            if (door != null)
            {
                door.EnableDoor();
                Debug.Log($"PowerGeneratorTrigger: Enabled door functionality for {interactableObject.name}");
            }
            else
            {
                Debug.LogWarning($"PowerGeneratorTrigger: No AllPurposeDoorOpen script found on {interactableObject.name} - Door will not function as expected.");
            }
        }

        StartCoroutine(StartPhoneRingAfterDelay());
    }

    private IEnumerator StartPhoneRingAfterDelay()
    {
        Debug.Log($"PowerGeneratorTrigger: Waiting {ringDelay} seconds to start phone ring for {gameObject.name}");
        yield return new WaitForSeconds(ringDelay);

        if (phoneRing != null)
        {
            phoneTriggered = true;
            isRinging = true;
            phoneRingCoroutine = StartCoroutine(PhoneRingLoop());
            Debug.Log($"PowerGeneratorTrigger: Started phone ring for {gameObject.name}");
        }
    }

    private IEnumerator PhoneRingLoop()
    {
        while (phoneTriggered && phoneRing != null)
        {
            phoneRing.Play();
            Debug.Log($"PowerGeneratorTrigger: Phone ring playing for {gameObject.name}");
            yield return new WaitForSeconds(phoneRing.clip.length);
            yield return new WaitForSeconds(phoneRingInterval);
        }
    }

    public bool IsPowerGeneratorOn()
    {
        return isPoweredOn;
    }
}