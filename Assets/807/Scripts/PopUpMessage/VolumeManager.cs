using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using TMPro;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;
    public AudioSource audioSource;
    public TextMeshProUGUI volumeText;

   void Start()
{
    // Start with audio stopped and volume muted
    audioSource.Stop();
    
    // Add listener for volume changes
    volumeSlider.onValueChanged.AddListener(SetVolume);
    
    // Add listeners for drag events
    EventTrigger trigger = volumeSlider.gameObject.AddComponent<EventTrigger>();
    
    // Pointer Down event
    EventTrigger.Entry pointerDown = new EventTrigger.Entry();
    pointerDown.eventID = EventTriggerType.PointerDown;
    pointerDown.callback.AddListener((data) => { OnStartDrag(); });
    trigger.triggers.Add(pointerDown);
    
    // Start at zero
    volumeSlider.value = 0f;
    SetVolume(0f);
    
    // Make sure the text shows 0
    volumeText.text = "0";
    
    // Set the mixer to silent
    audioMixer.SetFloat("MasterVolume", -80f);
}

    public void OnStartDrag()
    {
        // Play sound as soon as user touches the slider
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

   public void SetVolume(float sliderValue)
{
    // Treat very small values as zero
    if (sliderValue < 0.001f)
    {
        sliderValue = 0f;
        volumeText.text = "0";
        audioMixer.SetFloat("MasterVolume", -80f);
        return;
    }

    // Convert linear to decibel for audio mixer
    float dB = sliderValue > 0 ? Mathf.Log10(sliderValue) * 20f : -80f;
    audioMixer.SetFloat("MasterVolume", dB);

    // Create a custom scale for displaying values (0 to 110)
    // When slider is halfway (0.5), display value will be 55
    int displayValue = Mathf.RoundToInt(sliderValue * 110);
    volumeText.text = displayValue.ToString();
}

    // Stop audio when this object gets disabled
    private void OnDisable()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Stop audio when this object gets destroyed
    private void OnDestroy()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}