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

    private bool showUI = false;

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
    }

    // Called when this object's collider enters another collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit the target object (or any object if targetTag is empty)
        if (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))
        {
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

                // Update the score text if available
                if (scoreText != null)
                {
                    // Get the score from NPCDialogueManager
                    if (NPCDialogueManager.Instance != null)
                    {
                        int correctAnswers = NPCDialogueManager.Instance.correctResponsesCount;

                        if (showTotalResponses)
                        {
                            scoreText.text = $"{scorePrefix}{correctAnswers}/{fixedTotalResponses}";
                        }
                        else
                        {
                            scoreText.text = $"{scorePrefix}{correctAnswers}";
                        }
                    }
                    else
                    {
                        scoreText.text = $"{scorePrefix}0/{fixedTotalResponses}";
                        Debug.LogWarning("NPCDialogueManager instance not found!");
                    }
                }
            }

            // Modify the transform of the target object
            if (targetObject != null)
            {
                targetObject.transform.position = newPosition;
            }
            else
            {
                Debug.LogWarning("Target object not assigned in ScoreTrigger!");
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
}