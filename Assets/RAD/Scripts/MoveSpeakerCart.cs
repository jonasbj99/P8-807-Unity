using UnityEngine;

public class MoveSpeakerCart : MonoBehaviour
{
    public Transform[] movePoints;

    public float speed = 5.0f;
    public float rotationSpeed = 5.0f;

    private Transform targetPoint;

    private int currentPointIndex = 0; // Index of the current target

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure there are points to move between
        if (movePoints.Length > 0)
        {
            transform.position = movePoints[0].position; // Start at the first point
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (movePoints.Length == 0) return; // Exit if no points are set

        // Get the current target point
        Transform targetPoint = movePoints[currentPointIndex];

        // Rotate the GameObject to face the target point
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move the GameObject towards the target point
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Check if the GameObject has reached the target point
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Move to the next point, looping back to the first point if at the end
            currentPointIndex = (currentPointIndex + 1) % movePoints.Length;
        }
    }
}
