using UnityEngine;
using UnityEngine.InputSystem;  // This is necessary for the Input System

public class PopupInfoTrigger : MonoBehaviour
{
    [TextArea]
    [SerializeField] string message = "This is a rule or some info!";
    [SerializeField] Transform vrHeadTransform; // Assign VR camera/player head
    [SerializeField] float triggerDistance = 2.0f;  // Distance to trigger popup
    [SerializeField] bool debugMode = true;

    bool hasShownPopup = false;

    private void Update()
    {
        // === DEBUG TRIGGER: Press B on the keyboard ===
        if (debugMode && Keyboard.current.bKey.wasPressedThisFrame)
        {
            // Show popup when 'B' is pressed (for debugging in editor)
            PopUpInfo.Instance.ShowPopup(transform, message);
        }

        // === DISTANCE TRIGGER (for VR) ===
        if (!hasShownPopup && vrHeadTransform != null)
        {
            float distance = Vector3.Distance(vrHeadTransform.position, transform.position);
            if (distance < triggerDistance)
            {
                // Show popup when the user is within the trigger distance
                PopUpInfo.Instance.ShowPopup(transform, message);
                hasShownPopup = true; // Ensure it only shows once
            }
        }
    }
}
