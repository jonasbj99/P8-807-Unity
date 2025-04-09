using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandController : MonoBehaviour
{
    public Animator handAnimator;         // Reference to the Animator
    public InputAction gripAction;        // The grip action
    private void Update()
    {
        // Get the grip value (should be between 0 and 1)
        float gripValue = gripAction.ReadValue<float>();

        // Pass the grip value to the Animator to control the "Grip" parameter
        handAnimator.SetFloat("Grip", gripValue);
    }
}
