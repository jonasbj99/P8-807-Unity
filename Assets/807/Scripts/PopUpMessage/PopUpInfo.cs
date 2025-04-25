using System.Collections;
using TMPro;
using UnityEngine;

//Bliver ikke rigtig brugt.

public class PopUpInfo : MonoBehaviour
{
    // Singleton instance to ensure only one PopUpInfo exists in the scene
    public static PopUpInfo Instance { get; private set; }

    [Header("Popup Settings")]
    [SerializeField] GameObject popupPrefab; // Prefab for the popup UI
    [SerializeField] float verticalOffset = 0.1f; // Vertical offset for popup positioning
    [SerializeField, Range(0.1f, 5.0f)] float fadeDuration = 0.5f; // Duration for fade-in/out animations

    private void Awake()
    {
        // Ensure only one instance of PopUpInfo exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
        Instance = this; // Set the singleton instance
    }

    // Method to display a popup near a target object
    public GameObject ShowPopup(Transform target, string message, Transform vrHeadTransform)
    {
        // Check if the popup prefab or target is missing
        if (popupPrefab == null || target == null)
            return null;

        // Calculate the position for the popup
        Vector3 popupPosition = CalculatePopupPosition(target);
        Debug.Log($"Popup Position: {popupPosition}"); // Log the calculated position for debugging

        // Instantiate the popup prefab at the calculated position
        GameObject popup = Instantiate(popupPrefab, popupPosition, Quaternion.identity);

        // Set the popup's text and orientation
        SetupPopupContent(popup, message, vrHeadTransform);

        // Add a fade-in effect to the popup
        CanvasGroup canvasGroup = GetOrAddCanvasGroup(popup);
        StartCoroutine(HandleFading(canvasGroup, popup));

        return popup; // Return the created popup
    }

    // Calculate the position for the popup based on the target's position
    public Vector3 CalculatePopupPosition(Transform target)
    {
        float offset = verticalOffset; // Start with the default vertical offset
        Renderer renderer = target.GetComponentInChildren<Renderer>(); // Get the renderer of the target

        // If the target has a renderer, adjust the offset based on its size
        if (renderer != null)
            offset += renderer.bounds.extents.y;
        else
            offset += 0.5f; // Use a default offset if no renderer is found

        return target.position + Vector3.up * offset; // Return the calculated position
    }

    // Set up the popup's content, including text and orientation
    private void SetupPopupContent(GameObject popup, string message, Transform vrHeadTransform)
    {
        // Find the text component in the popup and set its text
        TMP_Text textComponent = popup.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
            textComponent.text = message;

        // Align the popup to face the VR headset
        AlignPopupToVRHead(popup, vrHeadTransform);
    }

    // Align the popup to face the user's VR headset
    private void AlignPopupToVRHead(GameObject popup, Transform vrHeadTransform)
    {
        // Calculate direction from popup to VR head
        Vector3 directionToUser = vrHeadTransform.position - popup.transform.position;

        // If you want the popup to maintain an upright orientation (usually better for readability)
        directionToUser.y = 0; // Remove vertical component to keep popup upright

        // Create a rotation that faces the popup toward the user
        if (directionToUser != Vector3.zero)
        {
            popup.transform.rotation = Quaternion.LookRotation(directionToUser);
        }
    }

    // Get or add a CanvasGroup component to the popup for controlling transparency
    private CanvasGroup GetOrAddCanvasGroup(GameObject popup)
    {
        CanvasGroup canvasGroup = popup.GetComponentInChildren<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = popup.AddComponent<CanvasGroup>(); // Add CanvasGroup if missing

        canvasGroup.alpha = 0f; // Start with the popup fully transparent
        return canvasGroup;
    }

    // Handle the fade-in and fade-out animations for the popup
    private IEnumerator HandleFading(CanvasGroup canvasGroup, GameObject popup)
    {
        // Fade in the popup
        yield return FadeTo(canvasGroup, 1f);

        // Wait until the popup is deactivated
        while (popup.activeSelf)
            yield return null;

        // Reactivate the popup to ensure the coroutine continues
        popup.SetActive(true);

        // Fade out the popup
        yield return FadeTo(canvasGroup, 0f);

        // Destroy the popup after fading out
        Destroy(popup);
    }

    // Smoothly fade the popup's transparency to a target alpha value
    private IEnumerator FadeTo(CanvasGroup canvasGroup, float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha; // Current transparency
        float elapsedTime = 0f; // Time elapsed since the fade started

        // Gradually change the alpha value over the fade duration
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime; // Increment elapsed time
            float t = elapsedTime / fadeDuration; // Calculate progress (0 to 1)
            //t is used to interpolate between the start and target alpha values.

            // Use SmoothStep for a smooth easing effect
            // SmoothSteo is a function that eases the transition between two values
            // It takes three parameters: the start value, the end value, and the interpolation factor (t)
            canvasGroup.alpha = Mathf.SmoothStep(startAlpha, targetAlpha, t);
            yield return null; // Wait for the next frame
        }

        // Ensure the alpha value is exactly the target value
        canvasGroup.alpha = targetAlpha;
    }
}