using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveFood : MonoBehaviour
{
    public Transform targetPosition; // The position to move the instantiated copy to
    public GameObject foodObjects; // The prefab to instantiate

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the "5" key is pressed
        if (Keyboard.current[Key.Digit5].wasPressedThisFrame)
        {
            NewFoodPos(); // Call the function to instantiate and move the object
        }
    }

    public void NewFoodPos()
    {
        // Instantiate a copy of this object (including children) at the same position and rotation
        GameObject foodCopy = Instantiate(foodObjects, transform.position, transform.rotation);

        foodCopy.SetActive(true);

        // Move the instantiated copy to the target position
        foodCopy.transform.position = targetPosition.position;
    }
}
