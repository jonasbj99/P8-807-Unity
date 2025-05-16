using UnityEngine;

public class FadeInOut : MonoBehaviour
{

    public CanvasGroup canvasGroup; // Reference to the CanvasGroup component
    public bool fadein = false;
    public bool fadeout = false;

    public float TimeToFade; // Duration of the fade effect

    // Update is called once per frame
    void Update()
    {
        if (fadein == true)
        {
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += TimeToFade * Time.deltaTime; // Increase alpha value over time
                if (canvasGroup.alpha >= 1)
                {
                    fadein = false; // Stop fading in when fully visible
                }
            }
        }

        if (fadeout == true)
        {
            if (canvasGroup.alpha >= 0)
            {
                canvasGroup.alpha -= TimeToFade * Time.deltaTime; // Increase alpha value over time
                if (canvasGroup.alpha == 0)
                {
                    fadeout = false; // Stop fading in when fully visible
                }
            }
        }
    }

    public void FadeIn()
    {
        fadein = true; // Start fading in
    }

    public void FadeOut()
    {
        fadeout = true; // Start fading out
    }
}
