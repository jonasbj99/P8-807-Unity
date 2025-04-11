using UnityEngine;
using TMPro;
using System.Collections;

public class SoundMeterController : MonoBehaviour
{
    [SerializeField] TMP_Text soundLevelText;

    [SerializeField] int sampleSize = 1024;
    [SerializeField] float dBRange = 80f; // Decibel range 0 - dBRange
    private float[] samples;
    private float rmsValue = 0f;
    private float dbFS = 0f;         // dB relative to full scale (negative)
    private float dbPositive = 0f;   // shifted to positive

    void Start()
    {
        samples = new float[sampleSize];
    }

    void Update()
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
            dbFS = dBRange;
        }

        dbPositive = dbFS + dBRange;

        Debug.Log(dbPositive);
        soundLevelText.text = dbPositive.ToString("F1") + " dB";
    }

    /*
    IEnumerator SoundLevelReader() 
    {


        yield return new WaitForSeconds(0.5f);
    }
    */
}
