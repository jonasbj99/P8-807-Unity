using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class manages the flow of NPC dialogues in a game. 
/// It allows for multiple stages of dialogue, where each stage can have specific NPCs that are active or locked based on the player's progress.
/// It tracks which dialogues have been completed and provides custom prompts for locked NPCs.
/// </summary>
public class NPCDialogueFlow : MonoBehaviour
{
    public static NPCDialogueFlow Instance;

    //Systm.Serializable is used to allow this class to be serialized in Unity's Inspector.
    [System.Serializable]

    // This inner class represents a stage in the dialogue flow.
    // Each stage can have a specific active NPC, a dialogue set index, and a list of locked NPCs.
    // It also includes a message that can be displayed when the player tries to interact with a locked NPC.
    public class DialogueStage
    {
        public string stageName;
        public NPCDialogueData activeNPC;
        public int dialogueSetIndex = 0;
        [Tooltip("If true, this stage requires completion of a specific dialogue set to advance")]
        public bool requireSpecificDialogueCompletion = true;
        public List<NPCDialogueData> lockedNPCs;
        public string lockedMessage = "You need to talk to {0} first.";
    }

    // List of all dialogue stages in the game
    // So they can be set up in the Inspector.
    public List<DialogueStage> dialogueStages;

    // The current index of the active stage in the dialogue flow
    public int currentStageIndex = 0;

    // Track completed dialogues by NPC and set index
    // This dictionary maps NPCs to a set of completed dialogue set indices.
    // The HashSet is used to ensure that each set index is unique for each NPC. and which dialogue sets have been completed.
    private Dictionary<NPCDialogueData, HashSet<int>> completedDialogueSets = new Dictionary<NPCDialogueData, HashSet<int>>();

    // This dictionary is used to store custom prompts for locked NPCs.
    private Dictionary<NPCDialogueData, string> customPrompts = new Dictionary<NPCDialogueData, string>();


    //SLET EFTER
    [Header("Debug")]
    public bool showDebugLogs = true;


    // Singleton pattern to ensure only one instance of this class exists.
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // This method is called when the script instance is being loaded.
    // It checks if any dialogue stages are set up and initializes the first stage.
    private void Start()
    {
        // Initialize the first stage
        if (dialogueStages.Count > 0)
            SetupCurrentStage();
    }

    // This method checks if a specific NPC is active in the current stage.
    // It returns true if the NPC is either the active NPC for this stage or not locked by any other NPCs.
    // If all stages are completed, all NPCs are available.
    // This is useful for determining if an NPC can be interacted with at any given time.
    public bool IsNPCActive(NPCDialogueData npc)
    {
        // It starts with a edge case check for the current stage index.
        // If the current stage index is out of bounds, it means all stages are completed.
        if (currentStageIndex >= dialogueStages.Count)
            return true; // All stages completed, all NPCs available

        // Get the current stage based on the current index
        // and check if the NPC is the active one for this stage.
        DialogueStage currentStage = dialogueStages[currentStageIndex];

        // After the new stage is set up, we check if the NPC is the active one for this stage.
        // The designated active NPC for this stage is always available.
        if (npc == currentStage.activeNPC)
            return true;

        // NPCs in the locked list are not available
        return !currentStage.lockedNPCs.Contains(npc);
    }

    // This method retrieves a custom prompt for a specific NPC.
    // It checks if the NPC has a custom prompt set up in the customPrompts dictionary.
    // If a custom prompt is found, it returns that prompt; otherwise, it returns null.
    public string GetCustomPromptForNPC(NPCDialogueData npc)
    {
        if (customPrompts.TryGetValue(npc, out string customPrompt))
            return customPrompt;

        return null; // No custom prompt, use default
    }


    // This method is called when a dialogue is completed to track the completion of a specific dialogue set for an NPC.
    // It adds the completed dialogue set to the completedDialogueSets dictionary for that NPC.
    public void CompleteDialogue(NPCDialogueData npc, int dialogueSetIndex)
    {
        // This updates the completed dialogue sets for the NPC.
        // If the NPC is not already in the dictionary, add it with an empty HashSet.
        // Then add the completed dialogue set index to that NPC's HashSet.
        if (!completedDialogueSets.ContainsKey(npc))
        {
            completedDialogueSets[npc] = new HashSet<int>();
        }
        completedDialogueSets[npc].Add(dialogueSetIndex);

        // SLET SENERE
        if (showDebugLogs)
            Debug.Log($"Completed dialogue set {dialogueSetIndex} for NPC {npc.npcName}");

        // Check if there are more stages to advance to.
        // If the current stage index is within bounds of the dialogue stages list, 
        // we check if the completed dialogue was with the active NPC and the correct set.
        // If so, we advance to the next stage.
        if (currentStageIndex < dialogueStages.Count)
        {
            DialogueStage currentStage = dialogueStages[currentStageIndex];

            // Only advance if completed dialogue was with the active NPC and correct set.
            // This is useful for stages that require specific dialogue completion to advance.
            if (npc == currentStage.activeNPC &&
                dialogueSetIndex == currentStage.dialogueSetIndex &&
                currentStage.requireSpecificDialogueCompletion)
            {
                AdvanceToNextStage();
            }
        }
    }

    // This method is called to advance to the next stage in the dialogue flow.
    // It increments the current stage index and sets up the new stage.
public void AdvanceToNextStage()
{
    //SLET SENERE
    if (showDebugLogs)
        Debug.Log($"Advancing from stage {currentStageIndex} ({dialogueStages[currentStageIndex].stageName}) to next stage");

    // Store reference to the current active NPC before advancing to next stage
    NPCDialogueData currentActiveNPC = null;
    if (currentStageIndex < dialogueStages.Count) {
        currentActiveNPC = dialogueStages[currentStageIndex].activeNPC;
    }

    // Increment the current stage index and check if it's within bounds of the dialogue stages list.
    // If it is, we set up the new stage.
    currentStageIndex++;
    if (currentStageIndex < dialogueStages.Count)
    {
        SetupCurrentStage();
        //SLET SENERE
        if (showDebugLogs)
            Debug.Log($"Advanced to stage {currentStageIndex}: {dialogueStages[currentStageIndex].stageName}");
    }
    else
    {
        //SLET SENERE
        Debug.Log("Reached the end of all dialogue stages");
        
        // Disable interaction prompts for ALL NPCs in the game
        NPCDialogueData[] allNPCs = FindObjectsOfType<NPCDialogueData>();
        foreach (NPCDialogueData npc in allNPCs)
        {
            npc.DisableInteractionPromptPermanently();
            if (showDebugLogs)
                Debug.Log($"Disabled interaction prompt for NPC: {npc.npcName}");
        }
        
        if (showDebugLogs)
            Debug.Log("Reached the end of all dialogue stages");
    }
}


    /// <summary>
    /// This method retrieves the dialogue set index for a specific NPC based on the current stage.
    /// It determins es which dialogue set the NPC should use based on the current stage index and the NPC's completed dialogues.
    /// </summary>
    public int GetDialogueSetForNPC(NPCDialogueData npc)
    {
        // Check if the current stage index is within bounds of the dialogue stages list.
        // If it is, we get the current stage and check if the NPC is the active one for this stage.
        if (currentStageIndex >= dialogueStages.Count)
            return 0; // Default to first dialogue set when all stages are complete

        // Get the current stage based on the current index
        // and check if the NPC is the active one for this stage.
        DialogueStage currentStage = dialogueStages[currentStageIndex];

        // If this is the active NPC for this stage, return the specific dialogue set
        if (npc == currentStage.activeNPC)
            return currentStage.dialogueSetIndex;

        // For other NPCs, check if they have completed dialogues and return appropriate set
        if (completedDialogueSets.ContainsKey(npc))
        {
            // If NPC has completed dialogues, return the highest completed set + 1 (if available)
            int highestCompletedSet = -1;
            foreach (int setIndex in completedDialogueSets[npc])
            {
                if (setIndex > highestCompletedSet)
                    highestCompletedSet = setIndex;
            }

            // Check if NPC has next dialogue set available
            if (highestCompletedSet + 1 < npc.dialogueSets.Count)
                return highestCompletedSet + 1;
        }

        // Default to first dialogue set
        return 0;
    }

    /// <summary>
    /// This method sets up the current stage by clearing any previous custom prompts and initializing the new stage.
    /// It also sets up custom prompts for locked NPCs based on the current stage's configuration.
    /// It is called when the dialogue flow advances to a new stage.
    /// </summary>
    private void SetupCurrentStage()
    {
        // Clear any previous custom prompts
        // This is done to ensure that the custom prompts are fresh for the new stage.
        customPrompts.Clear();

        // This checks if the current stage index is within bounds of the dialogue stages list.
        // Then access the current stage based on the current index.
        DialogueStage currentStage = dialogueStages[currentStageIndex];

        // Set up custom prompts for locked NPCs
        foreach (var npc in currentStage.lockedNPCs)
        {
            // If the NPC is locked, we set up a custom prompt for them.
            // The prompt is formatted with the name of the active NPC for this stage.
            // If there is no active NPC, we use a default message.
            // This is useful for providing hints to the player about who they need to talk to next.
            string message = string.Format(currentStage.lockedMessage,
                currentStage.activeNPC ? currentStage.activeNPC.npcName : "HELP HEHEH");
            customPrompts[npc] = message;
        }
    }

    // For debugging - shows which dialogue sets have been completed
    // SLET SENERE
    public void PrintCompletedDialogues()
    {
        Debug.Log("=== Completed Dialogue Sets ===");
        foreach (var npc in completedDialogueSets.Keys)
        {
            string completedSets = string.Join(", ", completedDialogueSets[npc]);
            Debug.Log($"NPC: {npc.npcName} - Sets: {completedSets}");
        }
    }
}