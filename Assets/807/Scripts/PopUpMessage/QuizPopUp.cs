using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming you are using TextMeshPro for UI text

public class QuizPopUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject uiPanel; // The UI panel to show/hide
    [SerializeField] private Transform player; // Reference to the player
    
    [Header("Settings")]
    [SerializeField] private float activationDistance = 5f; // Distance at which UI appears
    [SerializeField] private float deactivationDistance = 7f; // Distance at which UI disappears
    
    private bool isUIActive = false;
    
    void Start()
    {
        // Ensure UI is hidden at start
        if (uiPanel != null)
            uiPanel.SetActive(false);
        
        // Auto-find player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (player == null)
            Debug.LogWarning("No player assigned or found. Please assign a player reference.");
    }

    void Update()
    {
        if (player == null) return;
        
        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Show UI when player gets close
        if (!isUIActive && distanceToPlayer <= activationDistance)
        {
            ShowUI();
        }
        // Hide UI when player moves away
        else if (isUIActive && distanceToPlayer > deactivationDistance)
        {
            HideUI();
        }
    }
    
    private void ShowUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
            isUIActive = true;
            Debug.Log("Quiz UI activated");
        }
    }
    
    private void HideUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            isUIActive = false;
            Debug.Log("Quiz UI deactivated");
        }
    }
}