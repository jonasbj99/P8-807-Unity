using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPCAnimationHandler animationHandler;
    public float interactionDistance = 3f; // Distance at which NPC reacts

    private bool hasWaved = false; // Track if we've already played the wave animation
    private Transform player;

    private void Start()
    {
        // Find the player's transform (assuming only one player)
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        // Start in idle animation
        if (animationHandler != null)
        {
            animationHandler.PlayIdle();
        }
    }

    private void Update()
    {
        if (player == null || hasWaved)
            return; // Exit early if player not found or already waved

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Only check for interaction if we haven't waved yet
        if (distanceToPlayer <= interactionDistance && !hasWaved)
        {
            // Play wave animation once
            hasWaved = true; // Mark that we've played the wave animation
            
            if (animationHandler != null)
            {
                animationHandler.PlayWave();
                Debug.Log("Playing wave animation (one-time only)");
                
                // Return to idle animation after waving is complete
                Invoke("ReturnToIdle", 5f); // Adjust timing based on your wave animation length
            }
        }
    }
    
    private void ReturnToIdle()
    {
        if (animationHandler != null)
        {
            animationHandler.PlayIdle();
            Debug.Log("Returning to idle animation");
        }
    }
}