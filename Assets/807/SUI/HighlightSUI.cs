using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR;
using System.Collections;

public class HighlightSUI : MonoBehaviour
{
    [SerializeField] private float curlThreshold = 0.04f; // Fingers are curled if tips are within 4cm of palm
    [SerializeField] private float thumbExtendThreshold = 0.06f; // Thumb is extended if far from palm

    private XRHandSubsystem handSubsystem;

    private bool gestureActive = false;
    private bool wasThumbsUpLastFrame = false;

    public Material[] highlights;
    public float highlightRevealTime = 3f;
    bool isHighlightActive = false; // Flag to track if the coroutine is running

    //[SerializeField] private float proximityThreshold = 0.1f; // 10 cm
    //private Camera mainCamera;

    //bool earplugsOn = false;

    void Start()
    {
        //AudioListener.volume = 1.0f;

        //mainCamera = Camera.main;

        List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);

        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
            Debug.Log("XRHandSubsystem found and assigned.");
        }
        else
        {
            Debug.LogError("XRHandSubsystem not found.");
        }

        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 0);
        }
    }

    void Update()
    {
        if (handSubsystem == null) return;

        //XRHand leftHand = handSubsystem.leftHand;
        XRHand rightHand = handSubsystem.rightHand;

        bool isThumbsUp = IsThumbsUp(rightHand);

        if (isThumbsUp && !wasThumbsUpLastFrame)
        {
            gestureActive = !gestureActive;
            Debug.Log("Thumbs Up gesture detected. Toggled: " + gestureActive);

            if (gestureActive)
                OnThumbsUp();
            else
                OnThumbsDown();
        }

        wasThumbsUpLastFrame = isThumbsUp;


        /*
        if (IsHandNearHead(leftHand) && IsHandNearHead(rightHand))
        {
            earplugsOn = !earplugsOn;
        }

        if (earplugsOn)
        {
            AudioListener.volume = 0.2f;
        }
        else
        {
            AudioListener.volume = 1f;
        }
        */
    }

    private bool IsThumbsUp(XRHand hand)
    {
        // Get relevant joint poses
        bool gotThumb = hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose thumbPose);
        bool gotPalm = hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose);
        bool gotIndex = hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexPose);
        bool gotMiddle = hand.GetJoint(XRHandJointID.MiddleTip).TryGetPose(out Pose middlePose);
        bool gotRing = hand.GetJoint(XRHandJointID.RingTip).TryGetPose(out Pose ringPose);
        bool gotLittle = hand.GetJoint(XRHandJointID.LittleTip).TryGetPose(out Pose littlePose);

        if (!(gotThumb && gotPalm && gotIndex && gotMiddle && gotRing && gotLittle))
            return false;

        // Thumb must be extended away from palm
        float thumbDistance = Vector3.Distance(thumbPose.position, palmPose.position);
        if (thumbDistance < thumbExtendThreshold)
            return false;

        // Other fingers must be curled (close to palm)
        bool indexCurled = Vector3.Distance(indexPose.position, palmPose.position) < curlThreshold;
        bool middleCurled = Vector3.Distance(middlePose.position, palmPose.position) < curlThreshold;
        bool ringCurled = Vector3.Distance(ringPose.position, palmPose.position) < curlThreshold;
        bool littleCurled = Vector3.Distance(littlePose.position, palmPose.position) < curlThreshold;

        return indexCurled && middleCurled && ringCurled && littleCurled;
    }

    private void OnThumbsUp()
    {
        Debug.Log("Thumbs Up Gesture Activated");
        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 1);
        }
    }

    private void OnThumbsDown()
    {
        Debug.Log("Thumbs Up Gesture Deactivated");
        foreach (Material material in highlights)
        {
            material.SetFloat("_HighlightEnable", 0);
        }
    }

    /*
    private bool IsHandNearHead(XRHand hand)
    {
        if (!hand.isTracked) return false;

        // Get the joint directly using the joints dictionary and the joint ID
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose jointPose))
        {
            float distance = Vector3.Distance(jointPose.position, mainCamera.transform.position);
            return distance < proximityThreshold;
        }

        return false;
    }
    */
}
