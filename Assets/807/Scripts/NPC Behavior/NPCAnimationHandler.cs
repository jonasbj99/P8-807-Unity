using UnityEngine;

public class NPCAnimationHandler : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    
    // Animation parameter names
    public string waveAnimation = "IsWaving";
    

    private void Awake()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        if (animator != null)
        {
            // Set waving to false to return to idle
            animator.SetBool(waveAnimation, false);
        }
        else
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }
    }

    public void PlayWave()
    {
        if (animator != null)
        {
            // Set waving to true to trigger wave animation
            animator.SetBool(waveAnimation, true);
        }
        else
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }
    }
}