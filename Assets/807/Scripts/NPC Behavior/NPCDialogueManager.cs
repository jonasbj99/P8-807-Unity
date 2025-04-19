using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NPCDialogueManager : MonoBehaviour
{
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
        public string npcName; // Name of the NPC
        public AudioClip npcAudio; // The NPC's spoken line
        public string[] playerResponses; // The player's response options
        public int[] nextDialogueIndices; // Next dialogue entry for each response (-1 to end)
    }

    [Header("Dialogue Data")]
    public List<DialogueEntry> dialogueEntries;

    [Header("UI")]
    public GameObject dialogueCanvas; // World-space canvas
    public GameObject[] responseButtons; // Buttons for player responses
    public TextMeshProUGUI npcNameText; // Text element for the NPC's name

    [Header("Audio")]
    public AudioSource npcAudioSource;

    [Header("VR")]
    public Transform playerHead; // Assign your VR camera/head here
    public float triggerDistance = 2.0f;

    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;

    [Header("XR Input")]
    public InputActionReference dialogueStartAction; // Assign in Inspector

    void Start()
    {
        dialogueCanvas.SetActive(false);
        foreach (var btn in responseButtons)
            btn.SetActive(false);

        if (dialogueStartAction != null)
            dialogueStartAction.action.Enable();
    }

    void OnDestroy()
    {
        if (dialogueStartAction != null)
            dialogueStartAction.action.Disable();
    }


    void Update()
    {
        if (!isDialogueActive
            && Vector3.Distance(playerHead.position, transform.position) < triggerDistance
            && dialogueStartAction != null
            && dialogueStartAction.action.WasPressedThisFrame())
        {
            StartDialogue();
        }
    }



    public void StartDialogue()
    {
        if (dialogueEntries.Count == 0) return;
        isDialogueActive = true;
        currentDialogueIndex = 0;
        dialogueCanvas.SetActive(true);
        PositionCanvas();
        npcNameText.text = dialogueEntries[currentDialogueIndex].npcName;

        PlayCurrentDialogue();
    }

    void PlayCurrentDialogue()
    {
        foreach (var btn in responseButtons)
            btn.SetActive(false);

        var entry = dialogueEntries[currentDialogueIndex];
        if (entry.npcAudio != null)
        {
            npcAudioSource.clip = entry.npcAudio;
            npcAudioSource.Play();
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
        var entry = dialogueEntries[currentDialogueIndex];
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
        var entry = dialogueEntries[currentDialogueIndex];
        if (responseIndex < entry.nextDialogueIndices.Length)
        {
            int nextIdx = entry.nextDialogueIndices[responseIndex];
            if (nextIdx == -1)
            {
                EndDialogue();
                return;
            }
            currentDialogueIndex = nextIdx;
            PlayCurrentDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueCanvas.SetActive(false);
        npcAudioSource.Stop();
          // Detach the canvas from the player's head
    dialogueCanvas.transform.SetParent(null);
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