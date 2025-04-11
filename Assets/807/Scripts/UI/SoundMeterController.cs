using UnityEngine;
using TMPro;

public class SoundMeterController : MonoBehaviour
{
    [SerializeField] TMP_Text soundLevelText;

    [SerializeField] int sampleSize = 1024;
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
            dbFS = -80f;

        dbPositive = dbFS + 80f; // Now ranges from 0 (quiet) to 80 (loud)

        Debug.Log(dbPositive);
        soundLevelText.text = dbPositive.ToString("F1") + " dB";
    }
}
