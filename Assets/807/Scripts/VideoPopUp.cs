using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class VideoPopup : MonoBehaviour
{
    [Header("Video & Popup")]
    public VideoPlayer videoPlayer;
    public SceneIntroduction sceneIntroduction;

    [Header("XR Locomotion Locking")]
    public ActionBasedContinuousMoveProvider moveProvider;
    public ActionBasedContinuousTurnProvider turnProvider;
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider teleportationProvider;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started += OnVideoStarted;
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        // Lock movement at start, assuming video plays immediately
        LockMovement();
    }

    void OnVideoStarted(VideoPlayer vp)
    {
        LockMovement();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        UnlockMovement();

        if (sceneIntroduction != null)
        {
            sceneIntroduction.ShowPopup();
        }
    }

    void LockMovement()
    {
        if (moveProvider) moveProvider.enabled = false;
        if (turnProvider) turnProvider.enabled = false;
        if (teleportationProvider) teleportationProvider.enabled = false;
    }

    void UnlockMovement()
    {
        if (moveProvider) moveProvider.enabled = true;
        if (turnProvider) turnProvider.enabled = true;
        if (teleportationProvider) teleportationProvider.enabled = true;
    }
}


