using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandController : MonoBehaviour
{
    public Animator handAnimator;         // Reference to the Animator
    public InputActionProperty gripAction;        // The grip action

    public Material[] highlights;
    public float highlightToggle;

    public InputActionProperty showHighlights;

    public float highlightRevealTime = 3f;

    private bool isHighlightActive = false; // Flag to track if the coroutine is running


    private void Start()
    {
        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 0);
        }

        
    }

    private void Update()
    {
        // Get the grip value (should be between 0 and 1)
        float gripValue = gripAction.action.ReadValue<float>();

        // Pass the grip value to the Animator to control the "Grip" parameter
        handAnimator.SetFloat("Grip", gripValue);

        if (showHighlights.action.IsPressed() && !isHighlightActive)
        {
            StartCoroutine(activateHighlights());
        }
    }

    public IEnumerator activateHighlights()
    {
        isHighlightActive = true; // Set the flag to true

        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 1);
        }

        float elapsedTime = 0f;

        // Keep checking if the button is still pressed
        while (elapsedTime < highlightRevealTime || showHighlights.action.IsPressed())
        {
            if (showHighlights.action.IsPressed())
            {
                // Reset the timer if the button is still pressed
                elapsedTime = 0f;
            }
            else
            {
                // Increment the timer if the button is not pressed
                elapsedTime += Time.deltaTime;
            }

            yield return null; // Wait for the next frame
        }

        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 0);
        }

        isHighlightActive = false; // Reset the flag after the coroutine finishes
    }
}
