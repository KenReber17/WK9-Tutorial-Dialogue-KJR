using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CharacterDialogue
{
    [TextArea(1, 10)]
    public string name;
    public TMP_Text dialogueText;
    public Animator animator;
    public Dialogue dialogue;
    [Header("Audio Settings")]
    [Tooltip("Sound played when exiting this character's dialogue")]
    public AudioSource exitSound;
    [Tooltip("Optional voice clips for each dialogue sentence (e.g., MP3 files)")]
    public AudioClip[] voiceClips;
}

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Add characters and their dialogue settings here")]
    private CharacterDialogue[] characterDialogues;

    [Header("Typing Settings")]
    [Tooltip("Speed of typing in characters per second")]
    public float typingSpeed = 50f;
    [Tooltip("Delay in seconds after animation before typing starts for computer trigger")]
    public float computerTypingDelay = 1f;
    [Tooltip("Delay in seconds after animation before typing starts for phone trigger")]
    public float phoneTypingDelay = 0.2f;

    [Header("Audio Settings")]
    [Tooltip("Optional AudioSource for playing voice clips")]
    public AudioSource voiceAudioSource;

    private Queue<string> sentences;
    private CharacterDialogue currentCharacter;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool isWaitingForFirstSentence = false;
    private bool isComputerMode = false;
    private int currentSentenceIndex = 0;

    void Start()
    {
        sentences = new Queue<string>();
        foreach (var charDialogue in characterDialogues)
        {
            if (charDialogue.dialogueText != null)
                charDialogue.dialogueText.text = "";
            else
                Debug.LogWarning($"DialogueManager: dialogueText for {charDialogue.name} is not assigned!");
            if (charDialogue.animator == null)
                Debug.LogWarning($"DialogueManager: animator for {charDialogue.name} is not assigned!");
            if (charDialogue.dialogue == null || charDialogue.dialogue.sentences == null)
                Debug.LogWarning($"DialogueManager: dialogue or sentences for {charDialogue.name} is null!");
            if (charDialogue.exitSound != null && charDialogue.exitSound.isPlaying)
                charDialogue.exitSound.Stop();
            if (charDialogue.animator != null)
                charDialogue.animator.gameObject.SetActive(false);
        }
        if (voiceAudioSource != null && voiceAudioSource.isPlaying)
            voiceAudioSource.Stop();
        else if (voiceAudioSource == null)
            Debug.LogWarning("DialogueManager: voiceAudioSource is not assigned—voice clips won’t play!");
    }

    public void TriggerDialogue(string characterName, bool isComputerTrigger = false)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning($"DialogueManager: Cannot start dialogue for '{characterName}', another dialogue is active!");
            return;
        }

        currentCharacter = null;
        foreach (var charDialogue in characterDialogues)
        {
            if (charDialogue.name == characterName)
            {
                currentCharacter = charDialogue;
                break;
            }
        }

        if (currentCharacter == null)
        {
            Debug.LogError($"DialogueManager: No character dialogue found for name '{characterName}'!");
            return;
        }

        isComputerMode = isComputerTrigger;
        StartCharacterDialogue();
    }

    private void StartCharacterDialogue()
    {
        isDialogueActive = true;
        isWaitingForFirstSentence = !isComputerMode;
        currentSentenceIndex = 0;
        Debug.Log($"DialogueManager: Starting dialogue for: {currentCharacter.name}, isComputerMode: {isComputerMode}");
        if (currentCharacter.animator != null)
        {
            foreach (var charDialogue in characterDialogues)
            {
                if (charDialogue != currentCharacter && charDialogue.animator != null)
                    charDialogue.animator.gameObject.SetActive(false);
            }

            currentCharacter.animator.gameObject.SetActive(true);
            currentCharacter.animator.SetBool("IsOpen", true);
            Debug.Log($"DialogueManager: Set Animator IsOpen to true for {currentCharacter.name}");

            if (isComputerMode)
            {
                StartCoroutine(StartTypingAfterAnimation());
            }
        }
        else
        {
            Debug.LogError($"DialogueManager: Animator is null for {currentCharacter.name}!");
            return;
        }

        sentences.Clear();

        if (currentCharacter.dialogue != null && currentCharacter.dialogue.sentences != null)
        {
            Debug.Log($"DialogueManager: Enqueuing {currentCharacter.dialogue.sentences.Length} sentences for {currentCharacter.name}");
            foreach (string sentence in currentCharacter.dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            if (currentCharacter.voiceClips != null && currentCharacter.voiceClips.Length != currentCharacter.dialogue.sentences.Length)
            {
                Debug.LogWarning($"DialogueManager: Voice clips count ({currentCharacter.voiceClips.Length}) does not match sentences count ({currentCharacter.dialogue.sentences.Length}) for {currentCharacter.name}");
            }
        }
        else
        {
            Debug.LogError($"DialogueManager: dialogue or sentences is null for {currentCharacter.name}!");
            isDialogueActive = false;
            isWaitingForFirstSentence = false;
            return;
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            Debug.Log($"DialogueManager: All sentences displayed for {currentCharacter.name}, waiting for end button");
            return;
        }

        if (isTyping)
        {
            StopAllCoroutines();
            if (voiceAudioSource != null && voiceAudioSource.isPlaying)
            {
                voiceAudioSource.Stop();
                Debug.Log($"DialogueManager: Stopped voice clip for {currentCharacter.name} due to advance");
            }
            currentCharacter.dialogueText.text = sentences.Peek();
            isTyping = false;
            return;
        }

        string sentence = sentences.Dequeue();
        Debug.Log($"DialogueManager: Starting to type sentence for {currentCharacter.name}: {sentence}");
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        if (currentCharacter.dialogueText != null)
        {
            currentCharacter.dialogueText.text = "";
            string currentText = "";
            int charIndex = 0;

            if (voiceAudioSource != null && currentCharacter.voiceClips != null && 
                currentSentenceIndex < currentCharacter.voiceClips.Length && 
                currentCharacter.voiceClips[currentSentenceIndex] != null)
            {
                voiceAudioSource.clip = currentCharacter.voiceClips[currentSentenceIndex];
                voiceAudioSource.Play();
                Debug.Log($"DialogueManager: Playing voice clip for {currentCharacter.name}, sentence {currentSentenceIndex}");
            }
            currentSentenceIndex++;

            while (charIndex < sentence.Length)
            {
                currentText += sentence[charIndex];
                charIndex++;
                if (charIndex % 15 == 0 || charIndex == sentence.Length)
                {
                    currentCharacter.dialogueText.text = currentText;
                    yield return new WaitForSeconds(1f / typingSpeed * 15);
                }
            }
        }
        else
        {
            Debug.LogError($"DialogueManager: dialogueText is null for {currentCharacter.name}, cannot type sentence!");
        }
        isTyping = false;
    }

    public void ContinueConversation()
    {
        if (isDialogueActive)
        {
            Debug.Log($"DialogueManager: 'Continue' clicked, advancing dialogue for {currentCharacter.name}. Sentences remaining: {sentences.Count}");
            DisplayNextSentence();
        }
    }

    public void EndDialogueButton()
    {
        if (isDialogueActive)
        {
            Debug.Log($"DialogueManager: 'End' clicked for {currentCharacter.name}. Sentences remaining: {sentences.Count}");
            if (sentences.Count > 0)
            {
                sentences.Clear();
                if (currentCharacter.dialogueText != null)
                    currentCharacter.dialogueText.text = "";
                Debug.Log($"DialogueManager: Cleared remaining sentences for {currentCharacter.name}");
            }
            if (voiceAudioSource != null && voiceAudioSource.isPlaying)
            {
                voiceAudioSource.Stop();
            }
            if (currentCharacter.exitSound != null)
            {
                currentCharacter.exitSound.Play();
                Debug.Log($"DialogueManager: Playing exit sound for {currentCharacter.name}");
            }
            EndDialogue();
        }
        else
        {
            Debug.LogWarning("DialogueManager: 'End' clicked, but no active dialogue!");
        }
    }

    private void EndDialogue()
    {
        Debug.Log($"DialogueManager: End of conversation for {currentCharacter.name}");
        if (currentCharacter.animator != null)
        {
            currentCharacter.animator.SetBool("IsOpen", false);
            Debug.Log($"DialogueManager: Set Animator IsOpen to false for {currentCharacter.name}");
        }
        else
        {
            Debug.LogError($"DialogueManager: Animator is null for {currentCharacter.name}, cannot close dialogue box!");
        }

        if (currentCharacter.dialogueText != null)
            currentCharacter.dialogueText.text = "";
        isDialogueActive = false;
        isWaitingForFirstSentence = false;
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            if (isWaitingForFirstSentence)
            {
                Debug.Log($"DialogueManager: 'E' pressed to answer call for {currentCharacter.name}, starting dialogue after animation");
                StartCoroutine(StartTypingAfterAnimation());
                isWaitingForFirstSentence = false;
            }
            else
            {
                Debug.Log($"DialogueManager: 'E' pressed, advancing dialogue for {currentCharacter.name}. Sentences remaining: {sentences.Count}");
                // Match "Continue" behavior: skip typing if active, otherwise display next sentence
                if (isTyping)
                {
                    StopAllCoroutines();
                    if (voiceAudioSource != null && voiceAudioSource.isPlaying)
                    {
                        voiceAudioSource.Stop();
                        Debug.Log($"DialogueManager: Stopped voice clip for {currentCharacter.name} due to 'E' advance");
                    }
                    currentCharacter.dialogueText.text = sentences.Peek();
                    isTyping = false;
                }
                else
                {
                    DisplayNextSentence();
                }
            }
        }
    }

    IEnumerator StartTypingAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        float delay = isComputerMode ? computerTypingDelay : phoneTypingDelay;
        yield return new WaitForSeconds(delay);
        DisplayNextSentence();
    }

    public void TriggerDialogue(string characterName)
    {
        TriggerDialogue(characterName, false);
    }
}