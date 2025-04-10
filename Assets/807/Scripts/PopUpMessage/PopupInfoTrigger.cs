using UnityEngine;
using UnityEngine.InputSystem;  // This is necessary for the Input System

public class PopupInfoTrigger : MonoBehaviour
{
    [TextArea]
    [SerializeField] string message = "Hello Bitches!"; // Message to display in the popup
    [SerializeField] Transform vrHeadTransform; // Reference to the VR headset or player's head
    [SerializeField] float triggerDistance = 10.0f;  // Distance within which the popup will appear
    [SerializeField] bool debugMode = true; // Enable or disable debug mode for testing in the editor

    private GameObject currentPopup; // Reference to the currently active popup

    private void Update()
    {
        // DEBUG TRIGGER: Press B on the keyboard
        if (debugMode && Keyboard.current.bKey.wasPressedThisFrame)
        {
            // Show popup when 'B' is pressed (for debugging in the editor)
            PopUpInfo.Instance.ShowPopup(transform, message, vrHeadTransform);
        }

        // DISTANCE TRIGGER (for VR)
        if (vrHeadTransform != null) // Ensure the VR headset reference is assigned
        {
            // Calculate the distance between the VR headset and this object
            float distance = Vector3.Distance(vrHeadTransform.position, transform.position);

            // If the distance is less than the trigger distance, show the popup
            if (distance < triggerDistance)
            {
                // Check if a popup is already active
                if (currentPopup == null)
                {
                    // Show a new popup and store its reference
                    currentPopup = PopUpInfo.Instance.ShowPopup(transform, message, vrHeadTransform);
                }
                Debug.Log("Popup should appear now!");  // Log a message for debugging
            }
            else
            {
                // If the distance is greater than the trigger distance, hide the popup
                if (currentPopup != null)
                {
                    currentPopup.SetActive(false); // Deactivate the popup
                    currentPopup = null; // Clear the reference to the popup
                }
            }
        }
    }
}
