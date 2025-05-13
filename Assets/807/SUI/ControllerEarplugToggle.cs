using UnityEngine;
using UnityEngine.UI;

public class ControllerEarplugToggle : MonoBehaviour
{
    public Toggle uiToggle;
    private bool earplugsOn = false;

    void Start()
    {
        // Reset audio volume at start exactly like EarplugGesture
        AudioListener.volume = 1.0f;
        
        if (uiToggle != null)
        {
            uiToggle.onValueChanged.AddListener(OnToggleChanged);
            // Set initial UI state
            uiToggle.isOn = earplugsOn;
        }
    }

    void OnToggleChanged(bool isOn)
    {
        // Toggle earplug state when UI toggle changes
        earplugsOn = isOn;
        
        // Apply appropriate volume based on earplug state
        // Using exactly the same volume values as in EarplugGesture
        if (earplugsOn)
        {
            AudioListener.volume = 0.2f;
        }
        else
        {
            AudioListener.volume = 1.0f;
        }
    }

    void OnDestroy()
    {
        if (uiToggle != null)
        {
            uiToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }
}