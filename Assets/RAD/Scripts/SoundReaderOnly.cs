using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SoundReaderOnly : MonoBehaviour
{
    // Sound Level Meter
    [SerializeField] TMP_Text soundLevelText;
    [SerializeField] int sampleSize = 1024; // Higher samples might provide smaller jitters at the cost of higher processing (Should be a power of two)
    [SerializeField] float dBRange = 140f;  // Decibel range 0 - dBRange
    [SerializeField] float interval = 0.5f; // Seconds between each reading and display of sound level
    float[] samples;
    float rmsValue = 0f;
    float dbFS = 0f;    
    float dbPositive = 0f;

    void OnEnable()
    {
        // Sound Level Meter
        samples = new float[sampleSize];
        StartCoroutine(SoundLevelReader());
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

            if (soundLevelText != null)
            {
                soundLevelText.text = dbPositive.ToString("F1");
            }

            yield return new WaitForSeconds(interval);
        }
    }
}
