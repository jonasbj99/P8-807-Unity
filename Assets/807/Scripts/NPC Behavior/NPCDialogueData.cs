using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// This class manages the dialogue data for NPCs.
/// It includes multiple dialogue sets, audio settings, and interaction feedback.
/// <summary>

public class NPCDialogueData : MonoBehaviour
{
    //System.Serializable attribute allows this class to be serialized in the inspector
    //This is useful for creating custom data structures that can be edited in the Unity Inspector.
    [System.Serializable]

    /// This class represents a set of dialogue entries for an NPC.
    /// It includes an optional name for the set and a list of dialogue entries.
    public class DialogueSet
    {
        public string setName; // Optional name for this dialogue set, can be deleted if not needed
        public List<NPCDialogueManager.DialogueEntry> dialogueEntries; //The list of dialogue entries for this set.
        //This is a list of DialogueEntry objects, which are defined in the NPCDialogueManager class.
    }

    public string npcName; // The name of the NPC, used for display in dialogue

    [Header("Dialogue")] //Used to display a header in the inspector for better organization.
    public List<DialogueSet> dialogueSets = new List<DialogueSet>(); // The new List<DialogueSet> ensures that the list is initialized. And that the list is not null.
                                                                     //This takes the form of a list of DialogueSet objects, which are defined above.
                                                                     //This allows you to have multiple sets of dialogue for the same NPC, which can be useful for branching dialogue or different stages of a quest.

    [Header("Audio Settings")]
    public AudioSource npcAudioSource; // Attach audio source as each NPC has its own.

    [Header("Interaction Settings")]
    public Transform playerHead; // Reference to the player's head transform, used for distance checking
    public float triggerDistance = 2f; // Distance at which the player can interact with the NPC

    [Header("Interaction Feedback")]
    public GameObject interactionPrompt; // Assign a UI prompt or icon GameObject
    public float promptHeightOffset = 1.40f; // How high above the NPC to place the prompt
    public bool promptFaceCamera = true; // Whether the prompt should rotate to face the camera
    public Button promptButton; // Reference to the button on the prompt canvas
    private bool isPlayerInRange = false; // Track if the player is in range for interaction
    private Transform mainCamera; // Reference to the main camera transform for positioning the prompt

    [Header("VR Input")]
    public InputActionReference leftGripAction; // Reference to the left grip input action
    public InputActionReference rightGripAction; // Reference to the right grip input action
    private bool wasLeftGripPressed = false; // Track left grip press state
    private bool wasRightGripPressed = false; // Track right grip press state

    [Header("Prompt Messages")]
    public string defaultPromptMessage = "Hej, hvor er Amandas kop?"; // Default message for the prompt when the NPC is active
    public TextMeshProUGUI promptText; // Reference to text component on prompt
    private bool isDialogueCompleted = false;



    /// <summary>
    /// Unity's Start method is called before the first frame update.
    /// It initializes the interaction prompt and sets up the button click handler.
    /// It also sets up the audio source if not already assigned. For the dialogue interaction.
    /// </summary>
    void Start()
    {
        // Initialize the interaction prompt if assigned
        if (interactionPrompt != null)
        {
            // Set the prompt to inactive at start
            interactionPrompt.SetActive(false);
            // Set up button click handler if promptButton is assigned
            if (promptButton != null)
            {
                // Remove all existing listeners to avoid duplicates
                // Add a new listener to the button click event
                // This button is assigned in the inspector
                promptButton.onClick.RemoveAllListeners();
                // The new listener will call the TriggerDialogue method when clicked.
                promptButton.onClick.AddListener(TriggerDialogue);
            }
            else
            {
                // This is a fallback in case the button is not assigned in the inspector.
                // Try to find button in the prompt if not manually assigned
                Button[] buttons = interactionPrompt.GetComponentsInChildren<Button>();
                // Find the first button in the prompt's children - in our case there is only one.
                // This is useful if you have multiple buttons in the prompt and want to assign the first one.
                if (buttons.Length > 0)
                {
                    promptButton = buttons[0];
                    promptButton.onClick.RemoveAllListeners();
                    promptButton.onClick.AddListener(TriggerDialogue);
                }
            }
        }
        // Check if the player head is assigned, if not, try to find it in the scene
        // This is used to determine the distance from the player to the NPC.
        // And used to make the UI elements face the player.
        if (Camera.main != null)
            mainCamera = Camera.main.transform;
        // If no audio source is assigned, add one
        if (npcAudioSource == null)
        {
            npcAudioSource = gameObject.AddComponent<AudioSource>();
            // Configure basic spatial audio settings
            // This can be done in the inspector as well, but this is a good default.
            npcAudioSource.spatialBlend = 1.0f; // Full 3D audio
            npcAudioSource.rolloffMode = AudioRolloffMode.Linear;
            npcAudioSource.minDistance = 1.0f;
            npcAudioSource.maxDistance = 15.0f;
        }

        // Enable grip input actions
        if (leftGripAction != null)
        {
            leftGripAction.action.Enable();
        }

        if (rightGripAction != null)
        {
            rightGripAction.action.Enable();
        }
    }

    /// <summary>
    /// Unity's OnDestroy method is called when the object is destroyed.
    /// It removes the button click listener to prevent memory leaks.
    /// It properly unsubscribes the TriggerDialogue method from the 
    /// prompt button's click event. This prevents memory leaks by ensuring that 
    /// event references don't persist after the object is destroyed
    /// a common source of subtle bugs in Unity applications where destroyed objects 
    /// might still be referenced by event listeners.
    /// </summary>
    void OnDestroy()
    {
        if (promptButton != null)
            promptButton.onClick.RemoveListener(TriggerDialogue);

        if (leftGripAction != null)
        {
            leftGripAction.action.Disable();
        }

        if (rightGripAction != null)
        {
            rightGripAction.action.Disable();
        }
    }


    /// <summary>
    /// Unity's Update method is called once per frame.
    /// This handles the continuous checking of the player's distance to the NPC.
    /// It updates the visual feedback for the interaction prompt based on the player's distance.
    /// This is based on the predefined trigger distance.
    /// </summary>
    void Update()
    {
        // If dialogue is completed, ensure prompt stays disabled
        if (isDialogueCompleted)
        {
            if (interactionPrompt != null && interactionPrompt.activeSelf)
                interactionPrompt.SetActive(false);
            return;
        }
        // Check if player is in range of the NPC and the trigger distance defined.
        bool playerInRange = Vector3.Distance(playerHead.position, transform.position) < triggerDistance;

        // If the player is in range and the prompt is not already active, show it.
        // If the player is not in range and the prompt is active, hide it.
        // This compares the current player position to previously stored position.
        // If there is a state change, it updates the prompt visibility.
        if (playerInRange != isPlayerInRange)
        {
            isPlayerInRange = playerInRange;
            UpdateVisualFeedback(isPlayerInRange);
        }
        // Update the position of the prompt if it's active
        // This is used to see if the prompt is referenced in the inspector.
        // and if it is found that it is active in the scene. Then it updates the position.
        if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            UpdatePromptPosition();
        }

        // Check for VR controller input if player is in range
        if (isPlayerInRange)
        {
            // Check left grip
            if (leftGripAction != null)
            {
                bool isLeftGripPressed = leftGripAction.action.ReadValue<float>() > 0.5f;

                // Detect button press (transition from not pressed to pressed)
                if (isLeftGripPressed && !wasLeftGripPressed)
                {
                    TriggerDialogue();
                }

                wasLeftGripPressed = isLeftGripPressed;
            }

            // Check right grip
            if (rightGripAction != null)
            {
                bool isRightGripPressed = rightGripAction.action.ReadValue<float>() > 0.5f;

                // Detect button press (transition from not pressed to pressed)
                if (isRightGripPressed && !wasRightGripPressed)
                {
                    TriggerDialogue();
                }

                wasRightGripPressed = isRightGripPressed;
            }
        }
        else
        {
            // Reset when out of range
            wasLeftGripPressed = false;
            wasRightGripPressed = false;
        }

    }

    /// <summary>
    /// This method updates the visual feedback for the interaction prompt.
    /// It shows or hides the prompt based on the player's distance to the NPC.
    /// It takes a boolean parameter to determine whether to show or hide the prompt.
    /// It also checks if the NPC is active in the dialogue flow and updates the prompt text accordingly.
    /// /// </summary>
    private void UpdateVisualFeedback(bool showFeedback)
    {
        // Show/hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(showFeedback);
            // Set initial position when showing the prompt
            if (showFeedback)
            {
                // Position the prompt above the NPC and face the camera
                UpdatePromptPosition();

                // Thorugh the NPCDialogueFlow instance.
                // This is used to check if the NPC is active in the dialogue flow.
                // of if the NPC is locked.
                if (NPCDialogueFlow.Instance != null)
                {
                    // "This" is used to see current instance of the NPCDialogueData class.
                    // This allows the system to see which NPC is being interacted with.
                    if (NPCDialogueFlow.Instance.IsNPCActive(this))
                    {
                        // NPC is active, show default prompt
                        if (promptText != null)
                            promptText.text = defaultPromptMessage;
                        // Enable button interaction if the NPC is active to trigger dialogue
                        if (promptButton != null)
                        {
                            promptButton.gameObject.SetActive(true);
                            promptButton.interactable = true;
                        }
                    }
                    else
                    {
                        // NPC is locked, show custom message from the dialogue flow script.
                        // Which is created in the inspector.
                        string customMessage = NPCDialogueFlow.Instance.GetCustomPromptForNPC(this);
                        // If the custom message is not null or empty, use it.
                        // Otherwise, use the default prompt message.
                        if (!string.IsNullOrEmpty(customMessage) && promptText != null)
                            promptText.text = customMessage;
                        // Disable button interaction and hide the button if the NPC is locked
                        if (promptButton != null)
                            promptButton.interactable = false;
                        promptButton.gameObject.SetActive(false); // Hide the button if locked
                    }
                }
            }
        }
    }

    /// <summary>
    /// This method updates the position of the interaction prompt above the NPC.
    /// It sets the prompt's position based on the NPC's position and the specified height offset.
    /// It also makes the prompt face the camera if specified.
    /// /// </summary>
    private void UpdatePromptPosition()
    {
        // Position the prompt above the NPC.
        // It calculates the position based on the NPC's position and the height offset.
        Vector3 promptPosition = transform.position + Vector3.up * promptHeightOffset;
        interactionPrompt.transform.position = promptPosition;
        // Make the prompt face the camera if specified.
        // It uses Unitys Quaternion.LookRotation to set the rotation of the prompt.
        // so it faces the camera at all times.
        if (promptFaceCamera && mainCamera != null)
        {
            interactionPrompt.transform.rotation = Quaternion.LookRotation(
                interactionPrompt.transform.position - mainCamera.position
            );
        }
    }

    /// <summary>
    /// This method is called when the player interacts with the NPC.
    /// It triggers the dialogue flow for the NPC.
    /// It checks if the player is in range and if the NPC is active in the dialogue flow.
    /// It also handles the interaction prompt visibility and starts the dialogue.
    /// </summary>
    public void TriggerDialogue()
    {
        // Check if the player is in range to interact with the NPC
        if (isPlayerInRange)
        {
            // Check if this NPC is active in the dialogue flow - ensure it is not locked.
            if (NPCDialogueFlow.Instance != null && !NPCDialogueFlow.Instance.IsNPCActive(this))
                return; // NPC is locked, don't trigger dialogue

            // Hide the interaction prompt immediately when button to interact is pushed
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            // Get the appropriate dialogue set based on current stage from the NPCDialogueFlow.
            // This is used to get the index of the dialogue set for this NPC.
            int dialogueSetIndex = 0;
            if (NPCDialogueFlow.Instance != null)
            {
                dialogueSetIndex = NPCDialogueFlow.Instance.GetDialogueSetForNPC(this);
            }


            /// <summary>
            /// The code verifies that the index is non-negative and falls within 
            /// the valid range of available dialogue sets for this NPC. 
            /// When these conditions are met, the system calls the 
            /// StartDialogue method on the dialogue manager singleton, 
            /// passing five critical pieces of information: 
            /// the specific dialogue entries from the selected set, 
            /// the NPC's name for display purposes, 
            /// the NPC's audio source for voice playback, 
            /// a reference to the NPC itself (allowing callbacks), 
            /// and the dialogue set index (enabling the dialogue manager to track which conversation set is active).
            /// </summary>

            // Make sure we have a valid dialogue set to play
            if (dialogueSetIndex >= 0 && dialogueSetIndex < dialogueSets.Count)
            {
                NPCDialogueManager.Instance.StartDialogue(
                    dialogueSets[dialogueSetIndex].dialogueEntries,
                    npcName,
                    npcAudioSource,
                    this,  // Pass the NPC reference
                    dialogueSetIndex  // Pass the dialogue set index
                );
            }
            else if (dialogueSets.Count > 0)
            {
                // Fallback to first set if index is invalid
                NPCDialogueManager.Instance.StartDialogue(
                    dialogueSets[0].dialogueEntries,
                    npcName,
                    npcAudioSource,
                    this,
                    0
                );
            }
        }
    }

    /// <summary>
    /// This method is called when the NPC has reached the end of all dialogue stages.
    /// It will permanently disable the interaction prompt for this NPC.
    /// </summary>
    public void DisableInteractionPromptPermanently()
    {
        isDialogueCompleted = true;

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Remove button click listener to prevent any further interactions
        if (promptButton != null)
        {
            promptButton.onClick.RemoveListener(TriggerDialogue);
            promptButton.interactable = false;
        }

        Debug.Log($"Interaction prompt permanently disabled for NPC: {npcName}");
    }


}
