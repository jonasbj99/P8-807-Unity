using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WristSUI : MonoBehaviour
{
    // Sound Level Meter
    [SerializeField] TMP_Text soundLevelText;
    [SerializeField] int sampleSize = 1024; // Higher samples might provide smaller jitters at the cost of higher processing (Should be a power of two)
    [SerializeField] float dBRange = 110f;  // Decibel range 0 - dBRange
    [SerializeField] float interval = 0.5f; // Seconds between each reading and display of sound level
    float rmsValue = 0f;
    float dbFS = 0f;
    float dbPositive = 0f;

    // Exsposure Meter
    [SerializeField] Slider exposureSlider;
    [SerializeField] GameObject forwardMarker;
    [SerializeField] GameObject backwardMarker;
    [SerializeField] float exposureUpMultiplier = 5f;    // Multiplier for the speed of moving the exposure level up
    [SerializeField] float exposureDownMultiplier = 5f;  // Multiplier for the speed of moving the exposure level down
    int exposureThreshold = 85; // Decibel threshold for increasing exposure level
    float currentExposure = 0f;
    float targetExposure = 100f;
    float exposureSpeed = 1f;


    // Hearing Health
    [SerializeField] Slider hearingHealthSlider;
    int healthThreshold = 75;   // Exposure threshold for decreasing health bar
    int hearingHealth = 100;
    [SerializeField] int hearingDamage = 5;  // Amount of damage done every tick
    [SerializeField] float damageDelay = 1f;    // Defines the tick delay for health damage
    float earplugEffect = 1f;
    int earplugSubtract = 0;

    // Wrist UI
    [SerializeField] GameObject wristCanvas;
    [SerializeField] GameObject wristWatch;
    Camera mainCam;

    float wristThreshold = 0.6f;

    [SerializeField] bool wristUI = true;


    private void Start()
    {
        mainCam = Camera.main;

        // Exposure Meter
        exposureSlider.value = currentExposure;
        exposureSlider.maxValue = targetExposure;

        // Hearing Health
        hearingHealthSlider.maxValue = hearingHealth;
        hearingHealthSlider.value = hearingHealth;
    }

    void Update()
    {
        if (wristUI)
        {
            // Changes the rotation of the wrist UI
            wristCanvas.transform.LookAt(wristCanvas.transform.position + mainCam.transform.rotation * Vector3.forward, mainCam.transform.rotation * Vector3.up);

            // Activate UI when the watch points upwards
            float wristDot = Vector3.Dot(wristWatch.transform.forward, Vector3.up);
            if (wristDot > wristThreshold)
            {
                wristCanvas.SetActive(true);
            }
            else
            {
                wristCanvas.SetActive(false);
            }
        }

        //Exposure Meter
        if (dbPositive > exposureThreshold)
        {
            exposureSpeed = (((dbPositive - exposureThreshold) / 100) + 1) * exposureUpMultiplier; // Adjust Speed above threshold
            currentExposure = Mathf.MoveTowards(currentExposure, targetExposure, exposureSpeed * Time.deltaTime);
            forwardMarker.SetActive(true);
            backwardMarker.SetActive(false);
        }
        else
        {
            exposureSpeed = 1f * exposureDownMultiplier; // Adjust Speed below threshold
            currentExposure = Mathf.MoveTowards(currentExposure, 0f, exposureSpeed * Time.deltaTime);
            forwardMarker.SetActive(false);
            backwardMarker.SetActive(true);
        }
        exposureSlider.value = currentExposure;

        if (currentExposure <= 0f)
        {
            forwardMarker.SetActive(false);
            backwardMarker.SetActive(false);
        }
    }

    void OnEnable()
    {
        // Sound Level Meter
        StartCoroutine(SoundLevelReader());

        // Hearing Health
        StartCoroutine(HearingHealthDamage());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // Sound Level Meter
    IEnumerator SoundLevelReader()
    {
        while (enabled)
        {
            AudioListener.volume = earplugEffect;

            // Get left and right output data seperately
            float[] left = new float[sampleSize];
            float[] right = new float[sampleSize];
            AudioListener.GetOutputData(left, 0); // Left ear
            AudioListener.GetOutputData(right, 1); // Right ear

            // Average the channels
            float[] mono = new float[sampleSize];
            for (int i = 0; i < sampleSize; i++)
            {
                mono[i] = (left[i] + right[i]) * 0.5f;
            }

            //AudioListener.GetOutputData(samples, 0);

            float sum = 0f;
            for (int i = 0; i < sampleSize; i++)
            {
                sum += mono[i] * mono[i]; // Change mono to samples, if mono is not used
            }

            rmsValue = Mathf.Sqrt(sum / sampleSize);
            dbFS = 20f * Mathf.Log10(rmsValue);

            if (float.IsNegativeInfinity(dbFS))
            {
                dbFS = -dBRange;
            }

            dbPositive = dbFS + dBRange;

            dbPositive -= earplugSubtract;

            if (soundLevelText != null)
            {
                soundLevelText.text = dbPositive.ToString("F1");
            }

            yield return new WaitForSeconds(interval);
        }
    }

    // Hearing Health
    IEnumerator HearingHealthDamage()
    {
        while (enabled)
        {
            if (currentExposure > healthThreshold)
            {
                if (hearingHealth > 0)
                {
                    hearingHealth -= hearingDamage;
                }
                else
                {
                    hearingHealth = 0;
                }

            }
            hearingHealthSlider.value = hearingHealth;

            yield return new WaitForSeconds(damageDelay);
        }
    }
}
