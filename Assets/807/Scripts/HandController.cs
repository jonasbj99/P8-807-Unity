using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandController : MonoBehaviour
{
    [Header("Input Action Reference")]
    [SerializeField] private InputActionReference gripAction;

    [Header("Animator")]
    [SerializeField] private Animator handAnimator;

    private void OnEnable()
    {
        // Subscribe to the grip input actions
        gripAction.action.performed += OnGripPerformed;
        gripAction.action.canceled += OnGripCanceled;

        // Enable the input action
        gripAction.action.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe from the input actions
        gripAction.action.performed -= OnGripPerformed;
        gripAction.action.canceled -= OnGripCanceled;

        // Disable the input action
        gripAction.action.Disable();
    }

    private void OnGripPerformed(InputAction.CallbackContext context)
    {
        // Read the grip value as a float (this is the axis value)
        float gripValue = context.ReadValue<float>();

        // Optionally clamp the value to make sure it's within the expected range (0 to 1)
        gripValue = Mathf.Clamp01(gripValue);

        // Set the "Grip" parameter in the Animator
        handAnimator.SetFloat("Grip", gripValue);
    }

    private void OnGripCanceled(InputAction.CallbackContext context)
    {
        // When grip is canceled, set the "Grip" parameter to 0 (Open Hand)
        handAnimator.SetFloat("Grip", 0f);
    }
}
