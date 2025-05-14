using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.UI;

public class EarplugGesture : MonoBehaviour
{
    [SerializeField] private float proximityThreshold = 0.2f;
    private XRHandSubsystem handSubsystem;
    private Camera mainCamera;
    private bool earplugsOn = false;
    private bool gesturePreviouslyDetected = false;

    [SerializeField] private Toggle earplugToggle;

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
         mainCamera = Camera.main;

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

        if (earplugsOn)
        {
            AudioListener.volume = 0.15f;
            earplugToggle.isOn = true;
        }
        else
        {
            AudioListener.volume = 1f;
            earplugToggle.isOn = false;
        }
    }

     private bool IsHandNearHead(XRHand hand)
{
    if (!hand.isTracked) return false;

    // Use wrist joint instead of index tip
    if (hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out Pose jointPose))
    {
        // Convert joint position to local space relative to camera
        Vector3 headPosition = mainCamera.transform.position;
        Vector3 headForward = mainCamera.transform.forward;
        
        // Calculate distance between hand joint and head
        float distance = Vector3.Distance(jointPose.position, headPosition);
        
        // Optional: You might also want to check if the hand is in front of the head
        // Vector3 directionToHand = (jointPose.position - headPosition).normalized;
        // float dotProduct = Vector3.Dot(headForward, directionToHand);
        // return distance < proximityThreshold && dotProduct > 0.5f; // Only detect when hand is somewhat in front
        
        return distance < proximityThreshold;
    }

    return false;
}
}
