using UnityEngine;
using UnityEngine.Video;

public class IntroVideo : MonoBehaviour
{

    public Canvas introCanvas; // Reference to the intro video canvas  
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //videoPlayer = Object.FindFirstObjectByType<VideoPlayer>();
        //introCanvas = Object.FindFirstObjectByType<Canvas>();

        
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
}
