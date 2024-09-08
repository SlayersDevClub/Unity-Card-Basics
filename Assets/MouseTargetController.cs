using UnityEngine;

public class MouseTargetController : MonoBehaviour
{
    public Transform target; // The target object the camera looks at
    public float dampSpeed = 0.1f; // Damping speed, adjust for smoothness
    public float maxDistance = 2f; // Maximum distance the target can move from its initial position

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = target.position; // Store the initial position of the target
    }

    void Update()
    {
        // Get mouse position in screen space
        Vector3 mousePos = Input.mousePosition;

        // Convert mouse position to world space
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Get the target position in world space
            Vector3 targetPosition = hit.point;

            // Clamp the target position to stay within a limited range from the initial position
            targetPosition = initialPosition + Vector3.ClampMagnitude(targetPosition - initialPosition, maxDistance);

            // Apply a dampened movement towards the target position
            target.position = Vector3.Lerp(target.position, targetPosition, dampSpeed * Time.deltaTime);
        }
    }
}
