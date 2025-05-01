using UnityEngine;
using UnityEngine.Video;

public class VideoEndPopup : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public SceneIntroduction sceneIntroduction; // Reference to your popup logic script

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (sceneIntroduction != null)
        {
            sceneIntroduction.ShowPopup(); // Call the VR-aware popup logic
        }
    }
}

