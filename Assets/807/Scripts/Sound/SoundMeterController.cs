using UnityEngine;
using TMPro;
using System.Collections;

public class SoundMeterController : MonoBehaviour
{
    // This script tries to mimic a real Sound Level Meter
    // It converts the digital full scale decibel to a defined ranged similar to real world sound pressure level scales

    [SerializeField] TMP_Text soundLevelText;
    [SerializeField] int sampleSize = 1024; // Higher samples might provide smaller jitters at the cost of higher processing (Should be a power of two)
    [SerializeField] float dBRange = 80f;   // Decibel range 0 - dBRange
    [SerializeField] float interval = 0.5f; // Seconds between each reading and display of sound level

    private float[] samples;
    private float rmsValue = 0f;
    private float dbFS = 0f;    
    private float dbPositive = 0f;



    void Update()
    {
        //if (dbPositive > )
    }

    void OnEnable()
    {
        samples = new float[sampleSize];
        StartCoroutine(SoundLevelReader());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator SoundLevelReader()
    {
        while (enabled)
        {
            AudioListener.GetOutputData(samples, 0);

            float sum = 0f;
            for (int i = 0; i < sampleSize; i++)
            {
                sum += samples[i] * samples[i];
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
}
