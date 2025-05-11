using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;
using UnityEngine.XR.Management;

public class EarplugGesture : MonoBehaviour
{
    [SerializeField] private float proximityThreshold = 0.1f; // 10 cm
    private XRHandSubsystem handSubsystem;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Get the running XRHandSubsystem
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

        if (IsHandNearHead(leftHand) && IsHandNearHead(rightHand))
        {
            Debug.Log("Earplug gesture detected!");
            // Trigger audio effect or logic here
        }
    }

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
}

