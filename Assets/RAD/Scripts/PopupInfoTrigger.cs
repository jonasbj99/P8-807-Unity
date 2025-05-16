using UnityEngine;
using UnityEngine.UI;  // Required for Button component
using System.Collections;
using UnityEngine.InputSystem; // Required for Input System

public class PopupInfoTrigger : MonoBehaviour
{
    [Header("Popup Settings")]
    [SerializeField] GameObject popupPanel; // Reference to the popup panel GameObject
    [SerializeField] Transform vrHeadTransform; // Reference to the VR headset or player's head

    [Header("Button Canvas Settings")]
    [SerializeField] GameObject buttonCanvas; // Canvas containing the trigger button
    [SerializeField] Button triggerButton; // Reference to the UI button that will trigger the popup
    [SerializeField] float canvasAppearDistance = 2.0f; // Distance at which the button canvas appears

    [Header("VR Controller Settings")]
    [SerializeField] bool enableGripInput = true; // Enable grip input for showing/hiding the popup
    [SerializeField] InputActionReference leftGripAction; // Reference to the left grip input action
    [SerializeField] InputActionReference rightGripAction; // Reference to the right grip input action
    private GameObject currentPopup; // Reference to the currently active popup
    private bool canvasDisabledByInteraction = false; // Flag to track if canvas was disabled by interaction
    private bool wasOutOfRange = false; // Flag to track if player has left the trigger area.
    private bool leftGripWasPressed = false; // Track if left grip was already pressed
    private bool rightGripWasPressed = false; // Track if right grip was already pressed

    private void OnEnable()
    {
        // Enable grip input actions if assigned
        if (enableGripInput)
        {
            if (leftGripAction != null)
                leftGripAction.action.Enable();
            if (rightGripAction != null)
                rightGripAction.action.Enable();
        }
    }


private void OnDisable()
{
    // Disable grip input actions when this component is disabled
    if (leftGripAction != null)
        leftGripAction.action.Disable();
    if (rightGripAction != null)
        rightGripAction.action.Disable();
}

private void Start()
{
    // Initialize both popup and button canvas to inactive
    if (popupPanel != null)
    {
        popupPanel.SetActive(false);
    }

    if (buttonCanvas != null)
    {
        buttonCanvas.SetActive(false);
    }

    // Add click listener to the button
    if (triggerButton != null)
    {
        triggerButton.onClick.AddListener(TogglePopup);
    }
    else
    {
        Debug.LogWarning("Trigger button is not assigned in the PopupInfoTrigger component.");
    }
}

private void Update()
{
    // Check distance and update UI elements accordingly
    CheckDistanceAndUpdateUI();

    // Check for grip input when in range
    CheckGripInput();

    // Update popup orientation if it's active
    UpdatePopupOrientation();
}

private void CheckGripInput()
{
    // Skip if grip input is disabled or no actions assigned
    if (!enableGripInput || (leftGripAction == null && rightGripAction == null))
        return;

    // Skip if not in range
    float distance = Vector3.Distance(transform.position, vrHeadTransform.position);
    bool isInRange = distance <= canvasAppearDistance;
    if (!isInRange)
        return;

    // Check left grip
    if (leftGripAction != null)
    {
        float leftGripValue = leftGripAction.action.ReadValue<float>();
        bool isLeftGripPressed = leftGripValue > 0.5f; // Adjust threshold as needed

        // Detect new left grip press
        if (isLeftGripPressed && !leftGripWasPressed)
        {
            leftGripWasPressed = true;
            Debug.Log($"Left grip pressed! Value: {leftGripValue}");
            TogglePopup();
        }
        // Reset when left grip is released
        else if (!isLeftGripPressed && leftGripWasPressed)
        {
            leftGripWasPressed = false;
        }
    }

    // Check right grip
    if (rightGripAction != null)
    {
        float rightGripValue = rightGripAction.action.ReadValue<float>();
        bool isRightGripPressed = rightGripValue > 0.5f; // Adjust threshold as needed

        // Detect new right grip press
        if (isRightGripPressed && !rightGripWasPressed)
        {
            rightGripWasPressed = true;
            Debug.Log($"Right grip pressed! Value: {rightGripValue}");
            TogglePopup();
        }
        // Reset when right grip is released
        else if (!isRightGripPressed && rightGripWasPressed)
        {
            rightGripWasPressed = false;
        }
    }
}

private void CheckDistanceAndUpdateUI()
{
    if (vrHeadTransform == null)
        return;

    // Calculate distance between this object and the VR head
    float distance = Vector3.Distance(transform.position, vrHeadTransform.position);
    bool isInRange = distance <= canvasAppearDistance;

    // Handle when player leaves the range
    if (!isInRange)
    {
        // If player just left the range, log it
        if (!wasOutOfRange)
        {
            wasOutOfRange = true;
            Debug.Log("Player left interaction range");
        }

        // Disable both popup and button canvas when out of range
        if (buttonCanvas != null)
        {
            buttonCanvas.SetActive(false);
        }

        if (currentPopup != null)
        {
            HidePopup();
        }

        return; // Exit early since we're out of range
    }

    // Handle when player enters the range
    if (isInRange && wasOutOfRange)
    {
        // Reset flags when re-entering the range
        canvasDisabledByInteraction = false;
        wasOutOfRange = false;
        Debug.Log("Player re-entered interaction range, resetting flags");
    }

    // Only show button canvas if in range, not disabled by interaction, and no popup visible
    bool shouldShowButtonCanvas = isInRange &&
                                 !canvasDisabledByInteraction &&
                                 (currentPopup == null);

    // Update button canvas visibility
    if (buttonCanvas != null && buttonCanvas.activeSelf != shouldShowButtonCanvas)
    {
        buttonCanvas.SetActive(shouldShowButtonCanvas);

        // Update button canvas orientation immediately when activated
        if (shouldShowButtonCanvas)
        {
            UpdateButtonCanvasOrientation();
            Debug.Log("Button canvas activated");
        }
    }

    // If canvas is active, make sure it faces the user
    if (buttonCanvas != null && buttonCanvas.activeSelf)
    {
        UpdateButtonCanvasOrientation();
    }
}

private void UpdateButtonCanvasOrientation()
{
    if (buttonCanvas != null && vrHeadTransform != null)
    {
        // This makes the canvas face the player correctly
        Vector3 directionVector = buttonCanvas.transform.position - vrHeadTransform.position;
        directionVector.y = 0; // Keep it level (no tilt)

        if (directionVector != Vector3.zero)
        {
            buttonCanvas.transform.rotation = Quaternion.LookRotation(directionVector);
        }
    }
}

private void UpdatePopupOrientation()
{
    if (currentPopup != null && vrHeadTransform != null)
    {
        // Make the popup panel face the player
        Vector3 directionToUser = vrHeadTransform.position - currentPopup.transform.position;
        directionToUser.y = 0; // Keep it level (no tilt)

        if (directionToUser != Vector3.zero)
        {
            currentPopup.transform.rotation = Quaternion.LookRotation(directionToUser);
        }
    }
}

public void TogglePopup()
{
    Debug.Log("TogglePopup called");

    if (currentPopup == null)
    {
        ShowPopup();

        // Hide the button canvas after showing the popup
        if (buttonCanvas != null)
        {
            buttonCanvas.SetActive(false);
            canvasDisabledByInteraction = true; // Set the flag
            Debug.Log("Canvas disabled by interaction");
        }
    }
    else
    {
        HidePopup();

        // When closing the popup, we still don't want to immediately show the canvas
        // It will only show again after leaving and re-entering the range
        canvasDisabledByInteraction = true;
    }
}

// Show the popup
public void ShowPopup()
{
    if (popupPanel != null && currentPopup == null)
    {
        popupPanel.SetActive(true); // Activate the popup panel
        currentPopup = popupPanel; // Store the reference to the active popup
        Debug.Log("Popup panel activated");
    }
}

// Hide the popup
public void HidePopup()
{
    if (popupPanel != null && currentPopup != null)
    {
        popupPanel.SetActive(false); // Deactivate the popup panel
        currentPopup = null; // Clear the reference to the popup
        Debug.Log("Popup panel deactivated");
    }
}

private void OnDestroy()
{
    // Clean up the listener when the object is destroyed
    if (triggerButton != null)
    {
        triggerButton.onClick.RemoveListener(TogglePopup);
    }
}
}