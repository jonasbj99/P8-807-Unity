using UnityEngine;
using UnityEngine.Video;

public class VideoEndPopup : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject popupCanvas;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        popupCanvas.SetActive(true);
    }
}

