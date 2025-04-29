using UnityEngine;
using UnityEngine.UI; // Required for UI elements
using TMPro; // Required for TextMeshPro elements

public class ScoreTrigger : MonoBehaviour
{
    [SerializeField] private GameObject uiElement; // Reference to the UI element you want to show
    [SerializeField] private string targetTag = "Puha"; // Tag of the object that triggers the UI (optional)
    [SerializeField] private TextMeshProUGUI scoreText; // Reference to the TextMeshPro component for the score
    
    [SerializeField] private string scorePrefix = "Score: "; // Text to display before the score
    [SerializeField] private bool showTotalResponses = true; // Whether to show total responses alongside correct ones
    [SerializeField] private int fixedTotalResponses = 4; // Fixed number for total responses
    
    private bool showUI = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure UI is hidden at start
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
        else
        {
            Debug.LogWarning("UI Element not assigned in ScoreTrigger!");
        }
        
        // Check if scoreText is assigned
        if (scoreText == null && uiElement != null)
        {
            // Try to find a TextMeshProUGUI component in uiElement or its children
            scoreText = uiElement.GetComponentInChildren<TextMeshProUGUI>();
            if (scoreText == null)
            {
                Debug.LogWarning("Score Text not assigned in ScoreTrigger and none found in UI Element!");
            }
        }
    }

    // Called when this object's collider enters another collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if we hit the target object (or any object if targetTag is empty)
        if (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))
        {
            // Show the UI
            if (uiElement != null)
            {
                uiElement.SetActive(true);
                showUI = true;
                
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
        }
    }

    // Optional: hide UI when exiting the trigger
    private void OnTriggerExit(Collider other)
    {
        // Check if we're leaving the target object
        if ((string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag)) && showUI)
        {
            // Hide the UI
            if (uiElement != null)
            {
                uiElement.SetActive(false);
                showUI = false;
            }
        }
    }
}