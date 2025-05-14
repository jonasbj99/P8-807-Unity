using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.UI;

public class EarplugGesture : MonoBehaviour
{
    [SerializeField] private float proximityThreshold = 0.2f;
    [SerializeField] private Toggle earplugToggle;

    private XRHandSubsystem handSubsystem;
    private bool earplugsOn = false;
    private bool gesturePreviouslyDetected = false;

    void Start()
    {
        AudioListener.volume = 1.0f;

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

        if (earplugToggle != null)
        {
            earplugToggle.isOn = earplugsOn;
            earplugToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    void LateUpdate()
    {
        if (handSubsystem == null) return;

        // Get head position from XRNode.Head
        if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            Debug.LogWarning("Unable to get XRNode.Head position.");
            return;
        }

        XRHand leftHand = handSubsystem.leftHand;
        XRHand rightHand = handSubsystem.rightHand;

        bool gestureNowDetected = IsHandNearHead(leftHand, headPosition) && IsHandNearHead(rightHand, headPosition);

        if (gestureNowDetected && !gesturePreviouslyDetected)
        {
            earplugsOn = !earplugsOn;
            Debug.Log("Earplug gesture toggled: " + (earplugsOn ? "ON" : "OFF"));

            if (earplugToggle != null)
                earplugToggle.isOn = earplugsOn;

            UpdateVolume();
        }

        gesturePreviouslyDetected = gestureNowDetected;
    }

    private void OnToggleChanged(bool isOn)
    {
        earplugsOn = isOn;
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        AudioListener.volume = earplugsOn ? 0.15f : 1f;
    }

    private bool IsHandNearHead(XRHand hand, Vector3 headPosition)
    {
        if (!hand.isTracked) return false;

        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose jointPose))
        {
            float distance = Vector3.Distance(jointPose.position, headPosition);
            return distance < proximityThreshold;
        }

        return false;
    }
}

