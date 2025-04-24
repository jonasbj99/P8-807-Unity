using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SoundMeterController : MonoBehaviour
{
    // These comments are somewaht outdated
       // This script tries to mimic a real Sound Level Meter
        // It converts the digital full scale decibel to a defined ranged similar to real world sound pressure level scales


    // Sound Level Meter
    [SerializeField] TMP_Text soundLevelText;
    [SerializeField] int sampleSize = 1024; // Higher samples might provide smaller jitters at the cost of higher processing (Should be a power of two)
    [SerializeField] float dBRange = 140f;  // Decibel range 0 - dBRange
    [SerializeField] float interval = 0.5f; // Seconds between each reading and display of sound level
    float[] samples;
    float rmsValue = 0f;
    float dbFS = 0f;    
    float dbPositive = 0f;

    // Exsposure Meter
    [SerializeField] Slider exposureSlider;
    [SerializeField]float exposureUpMultiplier = 5f;    // Multiplier for the speed of moving the exposure level up
    [SerializeField]float exposureDownMultiplier = 2f;  // Multiplier for the speed of moving the exposure level down
    int exposureThreshold = 85; // Decibel threshold for increasing exposure level
    float currentExposure = 0f;
    float targetExposure = 100f;
    float exposureSpeed = 1f;
    

    // Hearing Health
    [SerializeField] Slider hearingHealthSlider;
    int healthThreshold = 70;   // Exposure threshold for decreasing health bar
    int hearingHealth = 100;
    int hearingDamage = 5;  // Amount of damage done every tick
    float damageDelay = 1f;    // Defines the tick delay for health damage

    private void Start()
    {
        // Exposure Meter
        exposureSlider.value = currentExposure;
        exposureSlider.maxValue = targetExposure;

        // Hearing Health
        hearingHealthSlider.maxValue = hearingHealth;
        hearingHealthSlider.value = hearingHealth;
    }

    void Update()
    {
        //Exposure Meter
        if (dbPositive > exposureThreshold)
        {
            exposureSpeed = (((dbPositive - exposureThreshold)/100) + 1) * exposureUpMultiplier; // Adjust Speed above threshold
            currentExposure = Mathf.MoveTowards(currentExposure, targetExposure, exposureSpeed * Time.deltaTime);
        }
        else
        {
            exposureSpeed = 1f * exposureDownMultiplier; // Adjust Speed below threshold
            currentExposure = Mathf.MoveTowards(currentExposure, 0f, exposureSpeed * Time.deltaTime);
        }
        exposureSlider.value = currentExposure;

        Debug.Log("Health: " + hearingHealth + ", Exposure: " + currentExposure + ", Decibel: " + dbPositive);
    }

    void OnEnable()
    {
        // Sound Level Meter
        samples = new float[sampleSize];
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
            // If not adjusting spread on Audio Source, use this to correct spatial sound problems
            // If sound needs to be lower when the back is turned to the source, rotate the Audio Listener to fix offset
            // Adjust both Audio Listener rotation and Spread for a hybrid fix
            /*
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
            */

            AudioListener.GetOutputData(samples, 0);

            float sum = 0f;
            for (int i = 0; i < sampleSize; i++)
            {
                sum += samples[i] * samples[i]; // Change samples to mono, if mono is used
            }

            rmsValue = Mathf.Sqrt(sum / sampleSize);
            dbFS = 20f * Mathf.Log10(rmsValue);

            if (float.IsNegativeInfinity(dbFS))
            {
                dbFS = -dBRange;
            }

            dbPositive = dbFS + dBRange;

            if (soundLevelText != null)
            {
                soundLevelText.text = dbPositive.ToString("F1") + " dB";
            }

            yield return new WaitForSeconds(interval);
        }
    }

    // Hearing Health
    IEnumerator HearingHealthDamage()
    {
        while(enabled)
        {
            if (currentExposure > healthThreshold)
            {
                if (hearingHealth > 0)
                {
                    hearingHealth -= hearingDamage;
                    Debug.Log(hearingHealth + " Oww");
                }
                else
                {
                    hearingHealth = 0;
                    Debug.Log("GGs");
                    // Potential for losing the game here
                }
            }
            hearingHealthSlider.value = hearingHealth;

            yield return new WaitForSeconds(damageDelay);
        }
    }
}
