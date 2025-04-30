using UnityEngine;

public class ChecklistObject : MonoBehaviour
{
    Checklist checklist;
    
    void Start()
    {
        checklist = FindAnyObjectByType<Checklist>();
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Hand") && (checklist.interactLButton.action.IsPressed() || checklist.interactRButton.action.IsPressed()))
        {
            gameObject.SetActive(false);
            checklist.ObjectInteraction(gameObject);
            Debug.Log("OnTrigger Hand tag + IsPressed");
        }
    }
}
