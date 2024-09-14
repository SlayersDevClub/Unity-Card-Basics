using UnityEngine;

public class MouseTargetController : MonoBehaviour
{
    public Transform target; // The target object the camera looks at
    public float dampSpeed = 0.1f; // Damping speed, adjust for smoothness
    public float maxDistance = 2f; // Maximum distance the target can move from its initial position

    [Range(1,4)]
    public float rangeXmin = 2.5f, rangeXmax = 2.5f, rangeYmin = 2.5f, rangeYmax = 2.5f;

    private Vector3 initialPosition;

    public bool debugView = false;

    void Start()
    {
        initialPosition = target.position; // Store the initial position of the target
        print(Screen.width/2);
    }
    Vector2 lastMousePos;
    public RectTransform boundPanel; // Reference to the UI Panel RectTransform

    void Update()
    {

        if ((Input.mousePosition.x < Screen.width / rangeXmin || Input.mousePosition.x > Screen.width / rangeXmax) ||
            (Input.mousePosition.y < Screen.height / rangeYmin || Input.mousePosition.y > Screen.height / rangeYmax))
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
        else 
        {
            target.position = Vector3.Lerp(target.position, initialPosition, dampSpeed * Time.deltaTime);
        }
        if(debugView)
        {
            // Calculate the size and position based on screen size and range
            float xMin = Screen.width / rangeXmin;
            float xMax = Screen.width / rangeXmax;
            float yMin = Screen.height / rangeYmin;
            float yMax = Screen.height / rangeYmax;

            // Set the size and position of the RectTransform
            boundPanel.sizeDelta = new Vector2(xMax - xMin, yMax - yMin);
            boundPanel.anchoredPosition = new Vector2((xMin + xMax) / 2 - Screen.width / 2, (yMin + yMax) / 2 - Screen.height / 2);
        }
    }
}
