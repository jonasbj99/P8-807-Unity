using UnityEngine;

public class MoveSpeakerCart : MonoBehaviour
{
    public Transform movePoint1;
    public Transform movePoint2;

    public float speed = 5.0f;
    public float rotationSpeed = 5.0f;

    private Transform targetPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPoint = movePoint1;
    }

    // Update is called once per frame
    void Update()
    {  
        // Rotate the GameObject to face the target point
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction != Vector3.zero) // Avoid errors when direction is zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move the GameObject towards the target point
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Check if the GameObject has reached the target point
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Switch the target point
            targetPoint = targetPoint == movePoint1 ? movePoint2 : movePoint1;
        }
    }
}
