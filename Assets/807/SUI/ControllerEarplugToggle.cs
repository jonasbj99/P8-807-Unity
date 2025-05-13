using UnityEngine;
using UnityEngine.UI;

namespace SUI
{
    /// <summary>
    /// Controls audio volume when a toggle button is pressed.
    /// Can be used to mute/unmute audio or switch between different volume levels.
    /// </summary>
    public class ControllerEarplugToggle : MonoBehaviour
    {
        [Header("Toggle Reference")]
        [SerializeField] private Toggle toggle;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource targetAudio; // The audio source to control

        [Header("Volume Levels")]
        [SerializeField] private float onVolume = 1.0f; // Volume when toggle is on
        [SerializeField] private float offVolume = 0.0f; // Volume when toggle is off
        
        private void Awake()
        {
            // Find toggle on this GameObject if not assigned
            if (toggle == null)
                toggle = GetComponent<Toggle>();
            
            if (toggle == null)
            {
                Debug.LogError("Toggle component missing for ControllerEarplugToggle!");
                return;
            }
            
            // Add listener to the toggle
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        
        private void Start()
        {
            // Initialize with current toggle state
            OnToggleValueChanged(toggle.isOn);
        }
        
        private void OnToggleValueChanged(bool isOn)
        {
            if (targetAudio == null)
                return;
                
            // Set volume based on toggle state
            targetAudio.volume = isOn ? onVolume : offVolume;
        }
        
        private void OnDestroy()
        {
            // Remove listener when destroyed
            if (toggle != null)
                toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}