using System.Collections;
using TMPro;
using UnityEngine;

public class PopUpInfo : MonoBehaviour
{
    public static PopUpInfo Instance;

    [Header("Popup Settings")]
    [SerializeField] GameObject popupPrefab;
    [SerializeField] float verticalOffset = 0.1f;
    [SerializeField] float fadeSpeed = 5.0f;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Call this to show a popup above a target object with a message.
    /// </summary>
    public GameObject ShowPopup(Transform target, string message, Transform vrHeadTransform)
    {
        if (popupPrefab == null || target == null)
            return null;

        //Debug.LogWarning("Popup prefab or target is null!");  // Debugging line


        Vector3 popupPosition = GetPopupPosition(target);
        //Debug.Log($"Popup Position: {popupPosition}");


        GameObject popup = Instantiate(popupPrefab, popupPosition, Quaternion.identity);
        //Debug.Log("Popup instantiated: " + popup);  // Debugging line


        TMP_Text textComponent = popup.GetComponentInChildren<TMP_Text>();
        CanvasGroup canvasGroup = popup.GetComponentInChildren<CanvasGroup>();

        if (textComponent != null)
            textComponent.text = message;

        FaceCanvasToCamera(popup, vrHeadTransform);
        // Ensure CanvasGroup exists or add one
        if (canvasGroup == null)
        {
            canvasGroup = popup.AddComponent<CanvasGroup>();
        }

        // Start fade coroutine
        StartCoroutine(FadePopup(canvasGroup, popup));
        return popup; // Return the instantiated popup
    }


    private void FaceCanvasToCamera(GameObject popup, Transform vrHeadTransform)
    {
        popup.transform.rotation = Quaternion.Euler(vrHeadTransform.eulerAngles.x, vrHeadTransform.eulerAngles.y, vrHeadTransform.eulerAngles.z);

    }
    private Vector3 GetPopupPosition(Transform target)
    {
        Renderer renderer = target.GetComponentInChildren<Renderer>();
        float offset = verticalOffset;

        if (renderer != null)
            offset += renderer.bounds.extents.y;
        else
            offset += 0.5f; // Add fallback offset if no renderer is found

        return target.position + Vector3.up * offset;
    }

    private IEnumerator FadePopup(CanvasGroup group, GameObject popup)
    {
        // Fade in
        while (group.alpha < 1.0f)
        {
            group.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Wait until the popup is explicitly told to fade out
        while (popup.activeSelf)
        {
            yield return null;
        }

        // Fade out
        while (group.alpha > 0.0f)
        {
            group.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        Destroy(popup);
    }
}
