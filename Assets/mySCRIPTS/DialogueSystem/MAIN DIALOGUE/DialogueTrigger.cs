using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [Tooltip("The name of the character whose dialogue will be triggered")]
    public string characterName;

    [Header("Trigger Settings")]
    [Tooltip("If true, requires 'E' press within collider; if false, 'E' works from anywhere (e.g., for phone calls)")]
    public bool ButtonPress = true;

    [Header("Optional Computer Trigger Settings")]
    [Tooltip("Optional: TextMeshPro for displaying prompt and confirmation messages")]
    public TMP_Text messageText;
    [Tooltip("Message displayed when player enters the trigger (e.g., 'ID REQUIRED')")]
    public string promptMessage = "ID REQUIRED";
    [Tooltip("Message displayed when 'E' is pressed (e.g., 'ACCESS GRANTED')")]
    public string confirmMessage = "ACCESS GRANTED";
    private bool playerInRange = false;
    private bool accessGranted = false;

    [Header("Optional Audio Settings")]
    [Tooltip("Optional: AudioSource for a phone ring sound")]
    public AudioSource phoneRing;
    [Tooltip("Delay between phone ring loops in seconds (e.g., 1 for a slower ring)")]
    public float phoneRingInterval = 1f;
    [Tooltip("Optional: AudioSource for sound when 'E' is pressed to enter conversation")]
    public AudioSource enterSound;
    [Tooltip("Optional: Additional sound for custom use")]
    public AudioSource optionalSound;
    private bool phoneTriggered = false;

    private bool hasTriggered = false;
    private Coroutine phoneRingCoroutine;
    private bool dialogueActivated = false; // New flag to prevent retriggering

    void Start()
    {
        if (messageText != null)
            messageText.text = "";
        if (phoneRing != null && !phoneRing.isPlaying)
            phoneRing.Stop();
        if (enterSound != null && enterSound.isPlaying)
            enterSound.Stop();
        if (optionalSound != null && optionalSound.isPlaying)
            optionalSound.Stop();

        if (string.IsNullOrEmpty(characterName))
            Debug.LogWarning("DialogueTrigger: characterName is not set on " + gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("DialogueTrigger: Trigger entered by: " + other.gameObject.name);
        if (other.CompareTag("Player") && !dialogueActivated) // Only if not already triggered
        {
            playerInRange = true;
            hasTriggered = false;
            if (messageText != null && !accessGranted) // Computer trigger mode
            {
                messageText.text = promptMessage;
                Debug.Log($"DialogueTrigger: Displaying '{promptMessage}'");
            }
            else if (messageText == null && phoneRing != null && !phoneTriggered) // Phone trigger mode
            {
                Debug.Log("DialogueTrigger: No messageText, starting phone ring");
                phoneTriggered = true;
                phoneRingCoroutine = StartCoroutine(PhoneRingLoop());
            }
            else if (messageText == null && phoneRing == null) // Face-to-face mode
            {
                Debug.Log("DialogueTrigger: No messageText or phoneRing, waiting for 'E' to trigger dialogue (face-to-face)");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (messageText != null)
            {
                messageText.text = "";
                Debug.Log("DialogueTrigger: Cleared message text on exit");
            }
            // Phone ring continues until "E" is pressed, no stop on exit
        }
    }

    public void TriggerPhoneRing()
    {
        if (phoneRing != null && !phoneTriggered && !dialogueActivated)
        {
            Debug.Log("DialogueTrigger: Phone ring triggered manually");
            phoneTriggered = true;
            phoneRingCoroutine = StartCoroutine(PhoneRingLoop());
        }
        else if (phoneRing == null && !dialogueActivated)
        {
            Debug.Log("DialogueTrigger: No phoneRing, triggering dialogue directly");
            TriggerDialogue(false); // Phone mode
            dialogueActivated = true; // Mark as triggered
        }
    }

    void Update()
    {
        if (dialogueActivated) return; // Prevent any further triggers

        // Computer mode: Requires player in range if ButtonPress is true
        if ((!ButtonPress || playerInRange) && !accessGranted && Input.GetKeyDown(KeyCode.E) && messageText != null)
        {
            Debug.Log("DialogueTrigger: 'E' pressed, starting FlashAccessGranted");
            if (enterSound != null)
            {
                enterSound.Play();
                Debug.Log("DialogueTrigger: Playing enterSound for computer trigger");
            }
            StartCoroutine(FlashAccessGranted());
        }

        // Phone mode: Rings until "E" is pressed, works from anywhere
        if (phoneRing != null && phoneTriggered && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("DialogueTrigger: 'E' pressed, accepting phone call");
            if (phoneRingCoroutine != null)
            {
                StopCoroutine(phoneRingCoroutine);
                phoneRingCoroutine = null;
            }
            if (phoneRing.isPlaying)
                phoneRing.Stop();
            if (enterSound != null)
            {
                enterSound.Play();
                Debug.Log("DialogueTrigger: Playing enterSound for phone trigger");
            }
            TriggerDialogue(false); // Phone mode
            phoneTriggered = false;
            dialogueActivated = true; // Mark as triggered
        }

        // Face-to-face mode: Requires player in range if ButtonPress is true
        if ((!ButtonPress || playerInRange) && !hasTriggered && Input.GetKeyDown(KeyCode.E) && messageText == null && phoneRing == null)
        {
            Debug.Log("DialogueTrigger: 'E' pressed, triggering dialogue directly (face-to-face)");
            if (enterSound != null)
            {
                enterSound.Play();
                Debug.Log("DialogueTrigger: Playing enterSound for face-to-face trigger");
            }
            hasTriggered = true;
            StartCoroutine(TriggerFaceToFaceDialogue());
        }
    }

    IEnumerator FlashAccessGranted()
    {
        accessGranted = true;
        if (messageText != null)
        {
            Debug.Log($"DialogueTrigger: Starting '{confirmMessage}' flash sequence");
            messageText.text = confirmMessage;
            yield return new WaitForSeconds(0.25f);
            messageText.text = "";
            yield return new WaitForSeconds(0.25f);
            messageText.text = confirmMessage;
            yield return new WaitForSeconds(0.25f);
            messageText.text = "";
            yield return new WaitForSeconds(0.25f);

            TriggerDialogue(true); // Computer mode
            dialogueActivated = true; // Mark as triggered
        }
    }

    IEnumerator TriggerFaceToFaceDialogue()
    {
        yield return new WaitForSeconds(0.1f);
        TriggerDialogue(false); // Face-to-face mode
        dialogueActivated = true; // Mark as triggered
    }

    private void TriggerDialogue(bool isComputerTrigger)
    {
        DialogueManager manager = FindObjectOfType<DialogueManager>();
        if (manager != null)
        {
            Debug.Log($"DialogueTrigger: Triggering dialogue for character: {characterName}, isComputerTrigger: {isComputerTrigger}");
            manager.TriggerDialogue(characterName, isComputerTrigger);
        }
        else
        {
            Debug.LogError("DialogueTrigger: DialogueManager not found in scene!");
        }
    }

    IEnumerator PhoneRingLoop()
    {
        while (phoneTriggered && phoneRing != null)
        {
            phoneRing.Play();
            Debug.Log("DialogueTrigger: Phone ring playing");
            yield return new WaitForSeconds(phoneRing.clip.length);
            yield return new WaitForSeconds(phoneRingInterval);
        }
    }
}