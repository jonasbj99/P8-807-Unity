using UnityEngine;

public class WristUI : MonoBehaviour
{
    [SerializeField] GameObject wristCanvas;
    [SerializeField] GameObject wristWatch;
    Camera mainCam;

    float wristDot;
    float wristThreshold = 0.6f;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Changes the rotation of the wrist UI
        wristCanvas.transform.LookAt(wristCanvas.transform.position + mainCam.transform.rotation * Vector3.forward, mainCam.transform.rotation * Vector3.up);

        // Activate UI when the watch points upwards
        wristDot =  Vector3.Dot(wristWatch.transform.forward, Vector3.up);
        if (wristDot > wristThreshold)
        {
            wristCanvas.SetActive(true);
        }
        else
        {
            wristCanvas.SetActive(false);
        }
    }
}
