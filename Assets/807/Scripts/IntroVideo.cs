using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class IntroVideo : MonoBehaviour
{

    public Canvas introCanvas; // Reference to the intro video canvas  
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component

    public Canvas endVideoCanvas; // Reference to the end video canvas
    public float delay; // Delay before showing the end video canvas

    public MonoBehaviour moveScript;     

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Kode der låser bevægelse.
        moveScript.enabled = false; // Disable the movement script at the start

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartVideo()
    {
        introCanvas.gameObject.SetActive(false); // Hide the intro canvas
        videoPlayer.Play();
    }

    public IEnumerator EndOfVideo(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        endVideoCanvas.gameObject.SetActive(true); // Show the end video canvas      
    }

    public void OnVideoEnd()
    {
        StartCoroutine(EndOfVideo(delay)); // Start the coroutine to show the end video canvas after the delay
    }

    public void ActivateMovement()
    {
        endVideoCanvas.gameObject.SetActive(false); // Hide the end video canvas
        // Kode der låser bevægelse op.
        moveScript.enabled = true; // Enable the movement script
    }
}
