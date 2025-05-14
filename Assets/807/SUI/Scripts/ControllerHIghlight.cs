using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerHIghlight : MonoBehaviour
{
    public Material[] highlights;
    public InputActionProperty showHighlights;
    public float highlightRevealTime = 3f;

    private bool isHighlightActive = false; // Flag to track if the coroutine is running

    private void Start()
    {
        // Initialize all highlight materials to disabled
        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 0);
        }
    }

    private void Update()
    {
        // Check for button press to activate highlights
        if (showHighlights.action.IsPressed() && !isHighlightActive)
        {
            StartCoroutine(ActivateHighlights());
        }
    }

    public IEnumerator ActivateHighlights()
    {
        isHighlightActive = true; // Set the flag to true

        // Enable all highlights
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

        // Disable all highlights
        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 0);
        }

        isHighlightActive = false; // Reset the flag after the coroutine finishes
    }
}
