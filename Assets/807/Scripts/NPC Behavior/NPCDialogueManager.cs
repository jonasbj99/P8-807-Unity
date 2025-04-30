using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NPCDialogueManager : MonoBehaviour
{
    public static NPCDialogueManager Instance;

    [System.Serializable]


    /// <summary>
    /// This script manages the dialogue system for NPCs in a VR environment.
    /// It handles the display of dialogue entries, player responses, and audio playback.
    /// DialogueEntry class holds the NPC's name, audio clip, player responses, and next dialogue indices.
    /// </summary>
    public class DialogueEntry
    {
        // This nested class holds the NPC name, audio clip, player responses, and next dialogue indices
        // This design helps encapsulate the DialogueEntry logic and keeps it scoped to the NPCDialogueManager.
        public AudioClip npcAudio; // The NPC's spoken line
        public string[] playerResponses; // The player's response options
        public int[] nextDialogueIndices; // Next dialogue entry for each response (-1 to end)
        public bool[] isCorrectResponse; // Which responses are considered "correct"
        public int pointsForCorrectResponse = 1; // Points awarded for correct responses
        public GameObject[] spawnObjects; // Objects to spawn for each response

    }



    [Header("Dialogue Data")]
    private List<DialogueEntry> currentDialogueEntries; // List of current dialogue entries
    private string currentNPCName; // Name of the current NPC
    private NPCDialogueData currentNPC; // Reference to the NPCDialogueData scriptable object of the current NPC
    private int currentDialogueSetIndex; // Index for the current dialogue set

    [Header("Dialogue Transitions")]
    public float dialogueFadeDuration = 0.5f; // Duration for fading in/out the dialogue canvas
    public CanvasGroup dialogueCanvasGroup;  // Assign the CanvasGroup component attached to your dialogueCanvas


    [Header("UI")]
    public GameObject dialogueCanvas; // World-space canvas
    public GameObject[] responseButtons; // Buttons for player responses
    public TextMeshProUGUI npcNameText; // Text element for the NPC's name


    [Header("Audio")]
    public AudioSource fallbackAudioSource; // Renamed from npcAudioSource - used only as fallback
    private AudioSource currentNPCAudioSource; // Reference to the current NPC's audio source


    [Header("VR")]
    public Transform playerHead; // Assign your VR camera/head here
    private int currentDialogueIndex = 0; // Current index of the dialogue entry being displayed
    private bool isDialogueActive = false; // Flag to check if dialogue is active

    [Header("Response Tracking")]
    public bool trackCorrectResponses = false; // Flag to enable tracking of correct responses
    public int correctResponsesCount = 0; // Count of correct responses given by the player
    public int totalResponsesGiven = 0; // Total responses given by the player

    // This method is called when the script instance is being loaded
    // It ensures that only one instance of the NPCDialogueManager exists in the scene
    // It also initializes the dialogueCanvasGroup if it wasn't assigned in the inspector
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        // If dialogueCanvasGroup wasn't assigned, try to get it
        if (dialogueCanvasGroup == null && dialogueCanvas != null)
        {
            dialogueCanvasGroup = dialogueCanvas.GetComponent<CanvasGroup>();
            if (dialogueCanvasGroup == null)
                dialogueCanvasGroup = dialogueCanvas.AddComponent<CanvasGroup>();
        }
    }

    // This method is called to start the dialogue with the specified entries and NPC name
    // It sets up the dialogue entries, NPC name, and audio source
    public void StartDialogue(List<DialogueEntry> entries, string npcName, AudioSource npcAudioSource = null, NPCDialogueData npc = null, int dialogueSetIndex = 0)
    {
        // Check if the entries are null or empty, or if dialogue is already active
        // If so, return early to avoid starting a new dialogue
        //The method begins with validation checks to ensure there's actual dialogue content 
        // to display and that no other dialogue is currently active. 
        if (entries == null || entries.Count == 0) return;
        if (isDialogueActive) return;

        // Next, it stores all the dialogue data needed for the conversation: 
        // the list of dialogue entries, the NPC's name, 
        // a reference to the NPC itself, and which dialogue set is being used. 
        currentDialogueEntries = entries;
        currentNPCName = npcName;
        currentNPC = npc;
        currentDialogueSetIndex = dialogueSetIndex;

        // Store the NPC's audio source or use fallback
        currentNPCAudioSource = npcAudioSource != null ? npcAudioSource : fallbackAudioSource;

        // Set the initial index to 0 and mark dialogue as active
        currentDialogueIndex = 0;
        isDialogueActive = true;

        // Set the NPC name in the UI
        if (npcNameText != null)
            npcNameText.text = currentNPCName;

        //It activates the dialogue canvas, positions it in front of the player,
        // and starts the fade-in effect.
        dialogueCanvas.SetActive(true);
        PositionCanvas();
        npcNameText.text = currentNPCName;

        StartCoroutine(FadeDialogueCanvas(0f, 1f, dialogueFadeDuration));

        // Start the dialogue by playing the first entry
        // It also sets the canvas to be a child of the player's head for proper positioning in VR.
        PlayCurrentDialogue();
    }


    /// <summary>
    /// This method plays the current dialogue entry for the NPC.
    /// It handles the audio playback and shows the response buttons after the audio is done playing.
    /// </summary>
    void PlayCurrentDialogue()
    {
        // It starts by disabling all response buttons to ensure a clean slate for the next dialogue entry.
        // This is important to prevent any leftover buttons from the previous entry from being visible.
        foreach (var btn in responseButtons)
            btn.SetActive(false);

        // Add bounds check to prevent index out of range
        // and to check if the current index is valid
        // If the index is invalid (negative or beyond the array bounds), 
        // the method ends the dialogue sequence rather than crashing.
        if (currentDialogueIndex < 0 || currentDialogueIndex >= currentDialogueEntries.Count)
        {
            EndDialogue();
            return;
        }

        // Get the current dialogue entry based on the current index
        // It uses the currentDialogueIndex to access the correct entry in the dialogueEntries list.
        var entry = currentDialogueEntries[currentDialogueIndex];

        // Check if the current entry has an audio clip assigned
        // If it does, play the audio and wait for it to finish before showing response buttons.
        // If not, show the response buttons immediately.
        if (entry.npcAudio != null)
        {
            currentNPCAudioSource.clip = entry.npcAudio;
            currentNPCAudioSource.Play();
            StartCoroutine(ShowResponsesAfterAudio(entry.npcAudio.length));
        }
        else
        {
            ShowResponseButtons();
        }
    }

    // This coroutine waits for the specified delay (audio length) before showing the response buttons
    // It uses a yield return statement to pause execution for the given time.
    // This is called in the above method.
    IEnumerator ShowResponsesAfterAudio(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowResponseButtons();
    }

    /// <summary>
    /// This method shows the response buttons based on the current dialogue entry.
    /// It checks if the response is not null or empty before displaying it.
    /// It also sets up the button click listeners to handle player responses.
    /// The method uses a for loop to iterate through the response buttons and set them up accordingly.
    /// This design allows for dynamic response options based on the current dialogue entry.
    /// </summary>
    void ShowResponseButtons()
    {
        // This gets the current dialogue entry based on the current index
        // It uses the currentDialogueIndex to access the correct entry in the dialogueEntries list.
        var entry = currentDialogueEntries[currentDialogueIndex];

        // It iterates through the response buttons and checks if there are valid responses to show.
        // If a response is valid (not null or empty), it sets the button active and assigns the response text.
        // It also sets up the button click listeners to handle player responses.
        // If a response is not valid, it hides the button.
        for (int i = 0; i < responseButtons.Length; i++)
        {
            if (i < entry.playerResponses.Length && !string.IsNullOrEmpty(entry.playerResponses[i]))
            {
                responseButtons[i].SetActive(true);
                responseButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = entry.playerResponses[i];
                int idx = i;
                var btn = responseButtons[i].GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnPlayerResponse(idx));
            }
            else
            {
                responseButtons[i].SetActive(false);
            }
        }
    }


    /// <summary>
    /// This method handles the player's response when they click a response button.
    /// It checks if the response is valid and updates the dialogue flow accordingly.
    /// It also tracks the response if tracking is enabled.
    /// The method uses the responseIndex to determine which response was clicked.
    /// It checks if the response is marked as correct and updates the score accordingly.
    /// It also provides optional visual feedback for correct and incorrect answers.
    /// Finally, it continues the dialogue flow based on the next dialogue indices.
    /// </summary>
    void OnPlayerResponse(int responseIndex)
    {
        // This checks if the dialogue is active and if the response index is valid
        // If the dialogue is not active or the response index is invalid, it returns early.
        var entry = currentDialogueEntries[currentDialogueIndex];

        // Track response if tracking is enabled
        if (trackCorrectResponses)
        {
            // Check the response index against the entry's correct response array
            totalResponsesGiven++;

            // Check if this response is marked as correct
            if (responseIndex < entry.isCorrectResponse.Length && entry.isCorrectResponse[responseIndex])
            {
                // Increment the correct responses count
                // This is where the score is updated based on the correct response
                correctResponsesCount += entry.pointsForCorrectResponse;
                // Optional visual feedback for correct answer

            }
        }

         // Activate the object associated with this response
    if (responseIndex < entry.spawnObjects.Length && entry.spawnObjects[responseIndex] != null)
    {
        entry.spawnObjects[responseIndex].SetActive(true);
    }
        
        // Continue with regular dialogue flow
        if (responseIndex < entry.nextDialogueIndices.Length) // Check if the player's chosen response is valid
        {
            int nextIdx = entry.nextDialogueIndices[responseIndex]; // Get the index of the next dialogue entry

            // If the index is -1 or out of bounds, end the dialogue
            if (nextIdx == -1 || nextIdx >= currentDialogueEntries.Count)
            {
                EndDialogue(); // Stop the dialogue sequence
                return;
            }

            currentDialogueIndex = nextIdx; // Update to the new dialogue index
            PlayCurrentDialogue(); // Start playing the next dialogue line
        }

    }

    /// <summary>
    /// This method ends the dialogue sequence.
    /// It stops the current NPC's audio, fades out the dialogue canvas, and resets the state.
    /// It also signals the dialogue flow manager that the dialogue has completed.
    /// The method uses a coroutine to handle the fade-out effect smoothly.
    /// It also detaches the canvas from the player's head to prevent any unwanted behavior.
    /// Finally, it resets the dialogue state and cleans up any references to the current NPC.
    /// </summary>
    void EndDialogue()
    {
        // This checks if the dialogue is active and if the current NPC is valid
        isDialogueActive = false;
        // Stop the current NPC's audio
        if (currentNPCAudioSource != null)
            currentNPCAudioSource.Stop();
            

        // Start fade out, and then disable canvas when done
        StartCoroutine(FadeDialogueCanvas(1f, 0f, dialogueFadeDuration, () =>
        {
            dialogueCanvas.SetActive(false);
            // Detach the canvas from the player's head
            dialogueCanvas.transform.SetParent(null);

            // Signal the dialogue flow manager that this dialogue completed
            if (NPCDialogueFlow.Instance != null && currentNPC != null)
            {
                NPCDialogueFlow.Instance.CompleteDialogue(currentNPC, currentDialogueSetIndex);
            }
        }));
    }

    /// <summary>
    /// This coroutine handles the fading effect for the dialogue canvas.
    /// It takes the start and target alpha values, duration, and an optional callback for when the fade is complete.
    /// The method uses a while loop to gradually change the alpha value of the canvas group over time.
    /// It uses Mathf.SmoothStep for a smoother transition effect.
    /// The coroutine yields null to wait for the next frame before continuing the loop.
    /// Finally, it sets the alpha to the target value and calls the onComplete callback if provided.
    /// </summary>
    IEnumerator FadeDialogueCanvas(float startAlpha, float targetAlpha, float duration, System.Action onComplete = null)
    {
        // Ensure we have a canvas group to fade
        if (dialogueCanvasGroup == null)
        {
            Debug.LogWarning("No CanvasGroup found for dialogue fading effect!");
            if (onComplete != null) onComplete();
            yield break;
        }

        // Start the fade effect
        // It uses a while loop to gradually change the alpha value of the canvas group over time.
        // it uses Time.deltaTime to ensure the fade effect is frame-rate independent.
        // and Mathf.Clamp01 to ensure the value stays between 0 and 1.
        // It also uses Mathf.SmoothStep for a smoother transition effect.
        // Then it uses a lerp function to interpolate between the start and target alpha values.
        // Finally, it sets the alpha to the target value and calls the onComplete callback if provided.
        float elapsedTime = 0f;
        dialogueCanvasGroup.alpha = startAlpha;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / duration);

            // Apply easing for smoother transition
            float easedTime = Mathf.SmoothStep(0f, 1f, normalizedTime);

            dialogueCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, easedTime);
            yield return null;
        }
        // Ensure we set the final alpha value
        dialogueCanvasGroup.alpha = targetAlpha;

        // If the target alpha is 0, disable the canvas group to prevent interaction
        if (onComplete != null)
            onComplete();
    }

    // This method positions the dialogue canvas in front of the player's head
    // It sets the canvas as a child of the player's head for proper positioning in VR.
    void PositionCanvas()
    {
        // Check if the player head is assigned
        if (playerHead != null)
        {
            // Parent the canvas to the player's head
            dialogueCanvas.transform.SetParent(playerHead);

            // Position the canvas slightly in front of the player's view
            dialogueCanvas.transform.localPosition = new Vector3(0, 0, 0.4f); // 1.5 meters in front
            dialogueCanvas.transform.localRotation = Quaternion.identity; // Reset rotation
        }
    }

}
