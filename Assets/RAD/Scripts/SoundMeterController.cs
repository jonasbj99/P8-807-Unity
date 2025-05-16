using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SoundMeterController : MonoBehaviour
{
    // Sound Level Meter
    [SerializeField] TMP_Text soundLevelText;
    [SerializeField] int sampleSize = 1024; // Higher samples might provide smaller jitters at the cost of higher processing (Should be a power of two)
    [SerializeField] float dBRange = 110f;  // Decibel range 0 - dBRange
    [SerializeField] float interval = 0.5f; // Seconds between each reading and display of sound level
    float[] samples;
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
    //[SerializeField] Gradient healthGradient; Meant for gradient health color change
    //RawImage healthFill; Meant for gradient health color change
    int healthThreshold = 75;   // Exposure threshold for decreasing health bar
    int lowHealthThreshold = 30; // Threshold for indicating low health
    int hearingHealth = 100;
    [SerializeField] int hearingDamage = 5;  // Amount of damage done every tick
    [SerializeField] float damageDelay = 1f;    // Defines the tick delay for health damage
    int healthPickUpAmount = 10; // Health gain when pick up is taken
    float earplugEffect = 1f;
    int earplugSubtract = 0;
    float earplugTime = 10f; // Time that earplugs last

    // Win Lose
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject lossScreen;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text winText;

    public AudioSource tinnitusSource;
    

    private void Start()
    {
        AudioListener.volume = 1f;

        lossScreen.SetActive(false);
        winScreen.SetActive(false);

        // Exposure Meter
        exposureSlider.value = currentExposure;
        exposureSlider.maxValue = targetExposure;

        // Hearing Health
        hearingHealthSlider.maxValue = hearingHealth;
        hearingHealthSlider.value = hearingHealth;
        //healthFill = hearingHealthSlider.fillRect.gameObject.GetComponent<RawImage>();
        //healthFill.color = healthGradient.Evaluate(hearingHealth / 100);
    }

    void Update()
    {
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

        //healthFill.color = healthGradient.Evaluate(hearingHealth / 100);

        //Debug.Log("Health: " + hearingHealth + ", Exposure: " + currentExposure + ", Decibel: " + dbPositive);
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

                    LowHealthIndication();
                }
                else
                {
                    hearingHealth = 0;
                    GameLost();
                }
                
                if (!tinnitusSource.isPlaying)
                {
                    tinnitusSource.Play();
                }

            }
            else if (currentExposure < healthThreshold)
            {
                tinnitusSource.Stop();
            }
            hearingHealthSlider.value = hearingHealth;

            yield return new WaitForSeconds(damageDelay);
        }
    }

    void LowHealthIndication()
    {
        if (hearingHealth < lowHealthThreshold)
        {
            // Indicate low health here
        }
    }

    public void OnHealthPickUp()
    {
        if (hearingHealth <= 90)
        {
            hearingHealth += healthPickUpAmount;
        }
        else
        {
            hearingHealth = 100;
        }
    }

    public void OnEarplugPickUp()
    {
        StartCoroutine(EarplugActive());
    }

    // Effect of earplugs must be changed here
    IEnumerator EarplugActive()
    {
        earplugEffect = 0.4f;
        earplugSubtract = 10;

        yield return new WaitForSeconds(earplugTime);

        earplugEffect = 1f;
        earplugSubtract = 0;
    }

    void GameLost()
    {
        // Freeze character??
        StopAllCoroutines();
        tinnitusSource.Stop();
        AudioListener.volume = 0;
        lossScreen.SetActive(true);
    }

    public void GameWon()
    {
        // Freeze character??
        StopAllCoroutines();
        tinnitusSource.Stop();
        AudioListener.volume = 0;

        string wText;

        switch (hearingHealth)
        {
            case >= 75:
                wText = "You are a Hearing Hero!";      // Text when scoring between 75 and 100 health
                break;
            case >= 50:
                wText = "You are a Noise Ninja!";       // Text when scoring between 50 and 75 health
                break;
            case >= 25:
                wText = "You are a Volume Rookie...";    // Text when scoring between 25 and 50 health 
                break;
            case < 25:
                wText = "You are Lost in Static...";    // Text when scoring between 0 and 25 health
                break;
        }

        winText.text = wText;
        scoreText.text = hearingHealth.ToString() + " / 100";
        winScreen.SetActive(true);
    }
}
