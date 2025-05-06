using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using TMPro; // Required for TextMeshPro elements

public class ScoreTrigger : MonoBehaviour
{
    [SerializeField] private GameObject UIScore; // Reference to the UI element you want to show
    [SerializeField] private string targetTag = "Puha"; // Tag of the object that triggers the UI (optional)
    [SerializeField] private TextMeshProUGUI scoreText; // Reference to the TextMeshPro component for the score
    [SerializeField] private string scorePrefix = "Score: "; // Text to display before the score
    [SerializeField] private bool showTotalResponses = true; // Whether to show total responses alongside correct ones
    [SerializeField] private int fixedTotalResponses = 4; // Fixed number for total responses
    [SerializeField] private TextMeshProUGUI descriptionText; // Reference to the TextMeshPro component for the description
    [SerializeField] private string scoreDescription = "Your final responses:"; // Descriptive text to display above the score
    [SerializeField] private GameObject targetObject; // The GameObject to modify when triggered
    [SerializeField] private Vector3 newPosition; // New position for the target object
    [SerializeField] private bool visibleAtStart = false; // Whether the object should be visible from the start

    [Header("NPC Dialogue Connection")]
    [SerializeField] private NPCDialogueData npcToWaitFor; // The NPC whose dialogue must complete
    [SerializeField] private int requiredDialogueSetIndex = -1; // Specific dialogue set index, -1 means any

    private bool showUI = false;
    private bool dialogueCompleted = false;

    public Transform targetPosition; // The position to move the instantiated copy to
    public GameObject foodObjects; // The prefab to instantiate

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure UI is hidden at start
        if (UIScore != null)
        {
            UIScore.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UI Element not assigned in ScoreTrigger!");
        }

        // Check if scoreText is assigned
        if (scoreText == null && UIScore != null)
        {
            // Try to find a TextMeshProUGUI component in UIElement or its children
            scoreText = UIScore.GetComponentInChildren<TextMeshProUGUI>();
            if (scoreText == null)
            {
                Debug.LogWarning("Score Text not assigned in ScoreTrigger and none found in UI Element!");
            }
        }

        // Set description text if it's assigned
        if (descriptionText != null)
        {
            descriptionText.text = scoreDescription;
        }

        // If we have a target object, and it shouldn't be visible at start, hide it
        if (targetObject != null && !visibleAtStart && npcToWaitFor != null)
        {
            targetObject.SetActive(false);
        }
        // If visibleAtStart is true or no NPC to wait for, mark dialogueCompleted as true
        else if (visibleAtStart || npcToWaitFor == null)
        {
            dialogueCompleted = true;
        }

        // Register to dialogue completion events
        if (NPCDialogueFlow.Instance != null && npcToWaitFor != null)
        {
            // Method to subscribe to dialogue completion events will be added in NPCDialogueFlow
            NPCDialogueFlow.Instance.OnDialogueCompleted += HandleDialogueCompleted;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (NPCDialogueFlow.Instance != null)
        {
            NPCDialogueFlow.Instance.OnDialogueCompleted -= HandleDialogueCompleted;
        }
    }

    // Called when any NPC dialogue is completed
    private void HandleDialogueCompleted(NPCDialogueData npc, int dialogueSetIndex)
    {
        // Check if this is the NPC we're waiting for
        if (npc == npcToWaitFor)
        {
            // If we don't care about the specific dialogue set, or it matches our required set
            if (requiredDialogueSetIndex == -1 || dialogueSetIndex == requiredDialogueSetIndex)
            {
                dialogueCompleted = true;

                // Make the target object visible now that dialogue is completed
                if (targetObject != null)
                {
                    targetObject.SetActive(true);
                }
            }
        }
    }

    // Called when this object's collider enters another collider
    private void OnTriggerEnter(Collider other)
    {
        // Only process trigger if dialogue is completed or no NPC was specified to wait for
        if (!dialogueCompleted)
            return;

        // Check if we hit the target object (or any object if targetTag is empty)
        if (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))
        {
            CloneFoodPos(); // Call the function to instantiate and move the object

            // Find the GameObject named "FoodObjects(Clone)" and enable it
            GameObject clonedFoodObject = GameObject.Find("FoodObjects(Clone)");
            if (clonedFoodObject != null)
            {
                clonedFoodObject.SetActive(true);
            }

            // Find the GameObject named "FoodObjects" and disable it
            GameObject originalFoodObject = GameObject.Find("FoodObjects");
            if (originalFoodObject != null)
            {
                originalFoodObject.SetActive(false);
            }

            // Show the UI
            if (UIScore != null)
            {
                UIScore.SetActive(true);
                showUI = true;

                // Set description text if it's assigned
                if (descriptionText != null)
                {
                    descriptionText.text = scoreDescription;
                }

                if (scoreText != null)
                {
                    // Get the score from NPCDialogueManager
                    int correctAnswers = 0;
                    if (NPCDialogueManager.Instance != null)
                    {
                        correctAnswers = NPCDialogueManager.Instance.correctResponsesCount;
                        Debug.Log($"ScoreTrigger: Retrieved correctResponsesCount = {correctAnswers}");
                    }
                    else
                    {
                        Debug.LogWarning("NPCDialogueManager instance not found!");
                    }

                    if (showTotalResponses)
                    {
                        scoreText.text = $"{scorePrefix}{correctAnswers}/{fixedTotalResponses}";
                    }
                    else
                    {
                        scoreText.text = $"{scorePrefix}{correctAnswers}";
                    }
                }
            }

            // Modify the transform of the target object
            if (targetObject != null)
            {
                targetObject.transform.position = newPosition;

                // Make the object static if it has a Rigidbody
                var rigidbody = targetObject.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = true; // Make the object static to prevent physics interactions
                }
            }
        }
    }

    // Optional: hide UI when exiting the trigger
    private void OnTriggerExit(Collider other)
    {
        // Check if we're leaving the target object
        if ((string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) && showUI)
        {
            // Hide the UI
            if (UIScore != null)
            {
                UIScore.SetActive(false);
                showUI = false;
            }
        }
    }

    public void CloneFoodPos()
    {
        // Instantiate a copy of this object (including children) at the same position and rotation
        GameObject foodCopy = Instantiate(foodObjects, transform.position, transform.rotation);

        foodCopy.SetActive(true);

        // Move the instantiated copy to the target position
        foodCopy.transform.position = targetPosition.position;
    }
}