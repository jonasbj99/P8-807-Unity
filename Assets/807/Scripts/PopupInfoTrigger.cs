using UnityEngine;
using UnityEngine.InputSystem;  // This is necessary for the Input System

public class PopupInfoTrigger : MonoBehaviour
{
    [TextArea]
    [SerializeField] string message = "This is a rule or some info!";
    [SerializeField] Transform vrHeadTransform; // Assign VR camera/player head
    [SerializeField] float triggerDistance = 10.0f;  // Distance to trigger popup
    [SerializeField] bool debugMode = true;

    private GameObject currentPopup; // Reference to the instantiated popup

    private void Update()
    {
        // === DEBUG TRIGGER: Press B on the keyboard ===
        if (debugMode && Keyboard.current.bKey.wasPressedThisFrame)
        {
            // Show popup when 'B' is pressed (for debugging in editor)
            PopUpInfo.Instance.ShowPopup(transform, message);
        }

        // === DISTANCE TRIGGER (for VR) ===
        if (vrHeadTransform != null)
        {
            float distance = Vector3.Distance(vrHeadTransform.position, transform.position);
            Debug.Log($"Distance to popup: {distance}");
            Debug.Log($"VR Head Position: {vrHeadTransform.position}");
            Debug.Log($"Target Position: {transform.position}");

            if (distance < triggerDistance)
            {
                 if (currentPopup == null)
                {
                    currentPopup = PopUpInfo.Instance.ShowPopup(transform, message);
                }
                Debug.Log("Popup should appear now!");  // Debugging when it should show
            }
            else
            {
                // Hide the popup if outside the trigger distance
                if (currentPopup != null)
                {
                    currentPopup.SetActive(false);
                    currentPopup = null;
                }
            } 
        }
    }
}
