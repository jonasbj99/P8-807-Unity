using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine.XR.Management;

public class EarplugGesture : MonoBehaviour
{
    [SerializeField] private float proximityThreshold = 0.1f; // 10 cm
    private XRHandSubsystem handSubsystem;
    private Camera mainCamera;

    private bool earplugsOn = false;
    private bool gesturePreviouslyDetected = false;

    void Start()
    {
        AudioListener.volume = 1.0f;
        mainCamera = Camera.main;

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
    }

    void Update()
    {
        if (handSubsystem == null) return;

        XRHand leftHand = handSubsystem.leftHand;
        XRHand rightHand = handSubsystem.rightHand;

        bool gestureNowDetected = IsHandNearHead(leftHand) && IsHandNearHead(rightHand);

        // Toggle only on the rising edge (gesture just started)
        if (gestureNowDetected && !gesturePreviouslyDetected)
        {
            earplugsOn = !earplugsOn;
            Debug.Log("Earplug gesture toggled: " + (earplugsOn ? "ON" : "OFF"));
        }

        gesturePreviouslyDetected = gestureNowDetected;

        AudioListener.volume = earplugsOn ? 0.2f : 1f;
    }

    private bool IsHandNearHead(XRHand hand)
    {
        if (!hand.isTracked) return false;

        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose jointPose))
        {
            float distance = Vector3.Distance(jointPose.position, mainCamera.transform.position);
            return distance < proximityThreshold;
        }

        return false;
    }
}

