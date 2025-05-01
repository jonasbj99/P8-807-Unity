using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightManager : MonoBehaviour
{
    private Light[] sceneLights; // Array to store all lights in the scene

    public float colorChangeInterval = 0.1f; // Time interval for changing colors
    private float timer = 0f;

    public Material spotlightMat;

    [System.Obsolete]
    void Start()
    {
        // Find all lights in the scene and filter only spotlights
        Light[] allLights = FindObjectsOfType<Light>(true); // Include inactive lights
        sceneLights = System.Array.FindAll(allLights, light => light.type == UnityEngine.LightType.Spot);

    }

    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Change colors and intensities when the timer exceeds the interval
        if (timer >= colorChangeInterval)
        {
            ChangeLightProperties();
            ChangeSpotlightMat();
            timer = 0f; // Reset the timer
        }


    }

    private void ChangeLightProperties()
    {
        foreach (Light light in sceneLights)
        {
            // Assign a random color to each light
            light.color = new Color(Random.value, Random.value, Random.value);

            // Assign a random intensity between 20 and 30
            light.intensity = Random.Range(200f, 280f);
        }
    }

    private void ChangeSpotlightMat()
    {
        // Assign a random color to the emission color of the spotlight material
        Color randomEmissionColor = new Color(Random.value, Random.value, Random.value);
        spotlightMat.SetColor("_EmissionColor", randomEmissionColor);

        // Ensure the material's emission is enabled
        spotlightMat.EnableKeyword("_EMISSION");
    }

    private void ResetEmissionColor()
    {
        // Set the emission color to white
        spotlightMat.SetColor("_EmissionColor", Color.white);

        // Ensure the material's emission is enabled
        spotlightMat.EnableKeyword("_EMISSION");
    }

    void OnApplicationQuit()
    {
        // Reset the emission color when the game stops running
        ResetEmissionColor();
    }

}


