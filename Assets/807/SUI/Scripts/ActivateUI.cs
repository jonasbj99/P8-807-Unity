using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateUI : MonoBehaviour
{
    [SerializeField] private GameObject uiElement; // The UI element to activate/deactivate
    [SerializeField] private InputActionProperty activateButton; // Reference to the button action
    
    [Tooltip("Optional - If toggle mode is false, UI will only show while button is held")]
    [SerializeField] private bool toggleMode = true;
    
    private bool isUIActive = false;

    void Start()
    {
        // Ensure UI is inactive at start
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }

    void OnEnable()
    {
        // Subscribe to button press events when component is enabled
        activateButton.action.performed += OnButtonPressed;
        activateButton.action.canceled += OnButtonReleased;
    }

    void OnDisable()
    {
        // Unsubscribe from button press events when component is disabled
        activateButton.action.performed -= OnButtonPressed;
        activateButton.action.canceled -= OnButtonReleased;
    }

    private void OnButtonPressed(InputAction.CallbackContext context)
    {
        if (uiElement == null) return;

        if (toggleMode)
        {
            // Toggle UI visibility
            isUIActive = !isUIActive;
            uiElement.SetActive(isUIActive);
        }
        else
        {
            // Show UI while button is held
            uiElement.SetActive(true);
        }
    }

    private void OnButtonReleased(InputAction.CallbackContext context)
    {
        if (uiElement == null || toggleMode) return;

        // Hide UI when button is released (only in non-toggle mode)
        uiElement.SetActive(false);
    }
}