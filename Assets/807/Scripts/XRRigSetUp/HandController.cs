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

        yield return new WaitForSeconds(highlightRevealTime);

        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 0);
        }

        isHighlightActive = false; // Reset the flag after the coroutine finishes
    }
}
