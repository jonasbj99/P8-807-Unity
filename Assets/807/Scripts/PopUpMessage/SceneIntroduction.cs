using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SceneIntroduction : MonoBehaviour
{
    [Header("Popup Settings")]
    [SerializeField] private GameObject popupPanel;
    // [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField, TextArea(3, 10)] private string introMessage = "Welcome to the environment!";
    //[SerializeField] private float displayTime = 5f;
    [SerializeField] private bool fadeEffect = true;
    [SerializeField] private float fadeSpeed = 0.5f;

    [Header("Optional Components")]
    [SerializeField] private Button closeButton;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("VR Settings")]
    [SerializeField] private Transform vrCamera; // Reference to the VR Camera/Head
    [SerializeField] private float distanceFromPlayer = 2f; // How far in front of the player
    [SerializeField] private bool followPlayer = true; // Whether to continuously follow player
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 0, 0); // Offset from camera forward

    private Canvas canvas;
    private bool isVisible = false;

    private void Awake()
    {
        // Ensure we have a CanvasGroup if fadeEffect is enabled
        if (fadeEffect && canvasGroup == null)
            canvasGroup = popupPanel.GetComponent<CanvasGroup>() ?? popupPanel.AddComponent<CanvasGroup>();

        // Get canvas component
        canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        // If no VR camera is set, try to find main camera
        if (vrCamera == null)
            vrCamera = Camera.main?.transform;
    }

    void Start()
    {
        // Hide popup initially
        if (popupPanel != null)
            popupPanel.SetActive(false);

        // Set message text if available
        // if (messageText != null)
        // messageText.text = introMessage;

        // Show popup when scene starts
        ShowPopup();

        // Setup close button if available
        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);

        // Make sure the canvas is set to world space for VR
        if (canvas != null)
            canvas.renderMode = RenderMode.WorldSpace;
    }

    void Update()
    {
        if (isVisible && followPlayer && vrCamera != null)
        {
            PositionCanvasInFrontOfPlayer();
        }
    }

    public void ShowPopup()
    {
        if (popupPanel == null) return;

        // Position in front of player before showing
        if (vrCamera != null)
            PositionCanvasInFrontOfPlayer();

        popupPanel.SetActive(true);
        isVisible = true;

        if (fadeEffect && canvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }

        if (closeButton != null)
            closeButton.interactable = true;

        // Auto-hide after displayTime if greater than 0
        //if (displayTime > 0)
        // Invoke(nameof(HidePopup), displayTime);
    }

    public void HidePopup()
    {
        if (popupPanel == null) return;

        isVisible = false;

        // Disable the close button to prevent multiple clicks
        if (closeButton != null)
            closeButton.interactable = false;


        if (fadeEffect && canvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
        else
        {
            popupPanel.SetActive(false);
        }
    }

    private void PositionCanvasInFrontOfPlayer()
    {
        if (vrCamera == null || canvas == null) return;

        // Position the canvas in front of the player
        transform.position = vrCamera.position + vrCamera.forward * distanceFromPlayer + positionOffset;

        // Rotate canvas to face the player
        transform.rotation = Quaternion.LookRotation(transform.position - vrCamera.position);
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        canvasGroup.alpha = 1f;

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        popupPanel.SetActive(false);
    }
}