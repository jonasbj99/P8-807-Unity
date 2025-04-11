using UnityEngine;
using UnityEngine.InputSystem;  // This is necessary for the Input System

public class PopupInfoTrigger : MonoBehaviour
{
    [SerializeField] GameObject popupPanel; // Reference to the popup panel GameObject
    [SerializeField] Transform vrHeadTransform; // Reference to the VR headset or player's head
    [SerializeField] float triggerDistance = 10.0f;  // Distance within which the popup will appear
    [SerializeField] bool debugMode = true; // Enable or disable debug mode for testing in the editor

    private GameObject currentPopup; // Reference to the currently active popup

    private void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false); // Ensure the popup panel is initially inactive
        }
    }

    private void Update()
    {
        // DEBUG TRIGGER: Press B on the keyboard
        if (debugMode && Keyboard.current.bKey.wasPressedThisFrame)
        {
            // Show popup when 'B' is pressed (for debugging in the editor)
            ShowPopup();
        }

        // DISTANCE TRIGGER (for VR)
        if (vrHeadTransform != null) // Ensure the VR headset reference is assigned
        {
            // Calculate the distance between the VR headset and this object
            float distance = Vector3.Distance(vrHeadTransform.position, transform.position);

            // If the distance is less than the trigger distance, show the popup
            if (distance < triggerDistance)
            {
                if (currentPopup == null)
                {
                    ShowPopup();
                }

                // Make the popup panel face the player
                if (popupPanel != null)
                {
                    popupPanel.transform.LookAt(vrHeadTransform);
                    popupPanel.transform.rotation = Quaternion.Euler(0, popupPanel.transform.rotation.eulerAngles.y, 0); // Lock rotation to Y-axis
                }
            }
            else
            {
                // If the distance is greater than the trigger distance, hide the popup
                if (currentPopup != null)
                {
                    HidePopup();
                }
            }
        }
    }

    private void ShowPopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true); // Activate the popup panel
            currentPopup = popupPanel; // Store the reference to the active popup
        }
    }

    private void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false); // Deactivate the popup panel
            currentPopup = null; // Clear the reference to the popup
        }
    }
}