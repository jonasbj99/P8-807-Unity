using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class NPCDialogueData : MonoBehaviour
{
    public string npcName;
    public List<NPCDialogueManager.DialogueEntry> dialogueEntries;

    [Header("Audio Settings")]
    public AudioSource npcAudioSource; // Each NPC has its own audio source

    [Header("Interaction Settings")]
    public Transform playerHead;
    public float triggerDistance = 2f;
    public InputActionReference dialogueStartAction;

    [Header("Interaction Feedback")]
    public GameObject interactionPrompt; // Assign a UI prompt or icon GameObject
    public float promptHeightOffset = 1.40f; // How high above the NPC to place the prompt
    public bool promptFaceCamera = true; // Whether the prompt should rotate to face the camera
    private Material originalMaterial;
    private bool isPlayerInRange = false;
    private Transform mainCamera;

   void Start()
    {
        if (dialogueStartAction != null)
            dialogueStartAction.action.Enable();
            
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        if (Camera.main != null)
            mainCamera = Camera.main.transform;
            
        // If no audio source is assigned, add one
        if (npcAudioSource == null)
        {
            npcAudioSource = gameObject.AddComponent<AudioSource>();
            // Configure basic spatial audio settings
            npcAudioSource.spatialBlend = 1.0f; // Full 3D audio
            npcAudioSource.rolloffMode = AudioRolloffMode.Linear;
            npcAudioSource.minDistance = 1.0f;
            npcAudioSource.maxDistance = 15.0f;
        }
    }

    void OnDestroy()
    {
        if (dialogueStartAction != null)
            dialogueStartAction.action.Disable();
    }

    void Update()
    {
        // Check if player is in range
        bool playerInRange = Vector3.Distance(playerHead.position, transform.position) < triggerDistance;
        
        // Show/hide interaction feedback
        if (playerInRange != isPlayerInRange)
        {
            isPlayerInRange = playerInRange;
            UpdateVisualFeedback(isPlayerInRange);
        }
        
        // Trigger dialogue if in range and button pressed
        if (isPlayerInRange && dialogueStartAction != null && dialogueStartAction.action.WasPressedThisFrame())
        {
            TriggerDialogue();
        }

          // Update the position of the prompt if it's active
        if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            UpdatePromptPosition();
        }
    }
    
    private void UpdateVisualFeedback(bool showFeedback)
    {
        // Show/hide interaction prompt
        if (interactionPrompt != null)
            interactionPrompt.SetActive(showFeedback);
            
         // Set initial position when showing the prompt
        if (showFeedback && interactionPrompt != null)
        {
            UpdatePromptPosition();
        }
    }

    private void UpdatePromptPosition()
    {
        // Position the prompt above the NPC
        Vector3 promptPosition = transform.position + Vector3.up * promptHeightOffset;
        interactionPrompt.transform.position = promptPosition;
        
        // Make the prompt face the camera if needed
        if (promptFaceCamera && mainCamera != null)
        {
            interactionPrompt.transform.rotation = Quaternion.LookRotation(
                interactionPrompt.transform.position - mainCamera.position
            );
        }
    }
    public void TriggerDialogue()
    {
        NPCDialogueManager.Instance.StartDialogue(dialogueEntries, npcName, npcAudioSource);
    }
}
