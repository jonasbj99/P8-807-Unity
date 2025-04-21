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

    //I am using a serializable class to store the dialogue data
    // This class holds the NPC's name, audio clip, player responses, and next dialogue indices

    //I have also used a nested class to store the dialogue data and also because it needs
    // to be only used in for this class and not outside of it.
    // This is a good practice to keep the code clean and organized.
    // This design helps encapsulate the DialogueEntry logic and keeps it scoped to the NPCDialogueManager
    // avoiding unnecessary exposure to other parts of the codebase.


    public class DialogueEntry
    {
        // This nested class holds the NPC name, audio clip, player responses, and next dialogue indices
        // This design helps encapsulate the DialogueEntry logic and keeps it scoped to the NPCDialogueManager.
        public AudioClip npcAudio; // The NPC's spoken line
        public string[] playerResponses; // The player's response options
        public int[] nextDialogueIndices; // Next dialogue entry for each response (-1 to end)
        public bool[] isCorrectResponse; // Which responses are considered "correct"
        public int pointsForCorrectResponse = 1; // Points awarded for correct responses
    }



[Header("Dialogue Data")]
private List<DialogueEntry> currentDialogueEntries;
private string currentNPCName;

[Header("Dialogue Transitions")]
public float dialogueFadeDuration = 0.5f;
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
private int currentDialogueIndex = 0;
private bool isDialogueActive = false;

[Header("Response Tracking")]
public bool trackCorrectResponses = false;
public int correctResponsesCount = 0;
public int totalResponsesGiven = 0;
public TextMeshProUGUI scoreText; // Optional UI element to show score


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

public void StartDialogue(List<DialogueEntry> entries, string npcName, AudioSource npcAudioSource = null)
{
    if (entries == null || entries.Count == 0) return;
    if (isDialogueActive) return;

    currentDialogueEntries = entries;
    currentNPCName = npcName;
    
    // Store the NPC's audio source or use fallback
    currentNPCAudioSource = npcAudioSource != null ? npcAudioSource : fallbackAudioSource;
    
    currentDialogueIndex = 0;
    isDialogueActive = true;

    dialogueCanvas.SetActive(true);
    PositionCanvas();
    npcNameText.text = currentNPCName;

    StartCoroutine(FadeDialogueCanvas(0f, 1f, dialogueFadeDuration));

    PlayCurrentDialogue();
}

void PlayCurrentDialogue()
{
    foreach (var btn in responseButtons)
        btn.SetActive(false);

    // Add bounds check to prevent index out of range
    if (currentDialogueIndex < 0 || currentDialogueIndex >= currentDialogueEntries.Count)
    {
        EndDialogue();
        return;
    }

    var entry = currentDialogueEntries[currentDialogueIndex];
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

IEnumerator ShowResponsesAfterAudio(float delay)
{
    yield return new WaitForSeconds(delay);
    ShowResponseButtons();
}

// This method shows the response buttons based on the current dialogue entry
// It checks if the response is not null or empty before displaying it
// It also sets up the button click listeners to handle player responses
// The method uses a for loop to iterate through the response buttons and set them up accordingly
// This design allows for dynamic response options based on the current dialogue entry.
void ShowResponseButtons()
{
    // This gets the current dialogue entry based on the current index
    // It uses the currentDialogueIndex to access the correct entry in the dialogueEntries list.
    var entry = currentDialogueEntries[currentDialogueIndex];
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

void OnPlayerResponse(int responseIndex)
{
    var entry = currentDialogueEntries[currentDialogueIndex];

    // Track response if tracking is enabled
    if (trackCorrectResponses)
    {
        totalResponsesGiven++;

        // Check if this response is marked as correct
        if (responseIndex < entry.isCorrectResponse.Length && entry.isCorrectResponse[responseIndex])
        {
            correctResponsesCount += entry.pointsForCorrectResponse;
            // Optional visual feedback for correct answer
            ShowCorrectAnswerFeedback(true);
        }
        else
        {
            // Optional visual feedback for incorrect answer
            ShowCorrectAnswerFeedback(false);
        }

        // Update score display if available
        UpdateScoreDisplay();
    }

    // Continue with regular dialogue flow
    if (responseIndex < entry.nextDialogueIndices.Length)
    {
        int nextIdx = entry.nextDialogueIndices[responseIndex];
        if (nextIdx == -1 || nextIdx >= currentDialogueEntries.Count)
        {
            EndDialogue();
            return;
        }
        currentDialogueIndex = nextIdx;
        PlayCurrentDialogue();
    }
}

// Visual feedback methods for correct/incorrect answers
void ShowCorrectAnswerFeedback(bool isCorrect)
{
    // You could implement a visual effect here, like:
    // - Flashing the button green for correct answers
    // - Playing a sound effect
    // - Showing a checkmark or X icon

    if (isCorrect)
    {
        // Example: Play correct answer sound
        // AudioSource.PlayClipAtPoint(correctAnswerSound, Camera.main.transform.position);
    }
    else
    {
        // Example: Play incorrect answer sound
        // AudioSource.PlayClipAtPoint(incorrectAnswerSound, Camera.main.transform.position);
    }
}

void UpdateScoreDisplay()
{
    if (scoreText != null)
    {
        scoreText.text = $"Score: {correctResponsesCount}/{totalResponsesGiven}";
    }
}

public float GetDialogueScore()
{
    if (totalResponsesGiven == 0) return 0f;
    return (float)correctResponsesCount / totalResponsesGiven;
}

// Call this when a dialogue or conversation set is complete
public void ResetResponseTracking()
{
    correctResponsesCount = 0;
    totalResponsesGiven = 0;
    UpdateScoreDisplay();
}

void EndDialogue()
{
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
    }));
}

// Coroutine to fade the canvas group
IEnumerator FadeDialogueCanvas(float startAlpha, float targetAlpha, float duration, System.Action onComplete = null)
{
    // Ensure we have a canvas group to fade
    if (dialogueCanvasGroup == null)
    {
        Debug.LogWarning("No CanvasGroup found for dialogue fading effect!");
        if (onComplete != null) onComplete();
        yield break;
    }

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

    dialogueCanvasGroup.alpha = targetAlpha;

    if (onComplete != null)
        onComplete();
}

void PositionCanvas()
{
    if (playerHead != null)
    {
        // Parent the canvas to the player's head
        dialogueCanvas.transform.SetParent(playerHead);

        // Position the canvas slightly in front of the player's view
        dialogueCanvas.transform.localPosition = new Vector3(0, 0, 1.0f); // 1.5 meters in front
        dialogueCanvas.transform.localRotation = Quaternion.identity; // Reset rotation
    }
}

}
