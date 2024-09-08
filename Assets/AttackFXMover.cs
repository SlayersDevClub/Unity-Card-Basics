using UnityEngine;
using DG.Tweening; // DOTween namespace

public class AttackFXMover : MonoBehaviour
{
    public GameObject prefab;           // The prefab to instantiate and animate
    public float duration = 2f;         // Duration of the animation
    public float height = 2f;           // Height of the parabola
    public Ease easeType = Ease.Linear; // Expose DOTween ease type in Inspector

    public float bounceDuration = 0.5f; // Duration of the bounce effect
    public float bounceStrength = 1f;   // Strength of the bounce effect
    public int bounceVibrato = 10;      // Vibrato (number of bounces)
    public float shakeDuration = 0.5f;  // Duration of the shake effect
    public float shakeStrength = 1f;    // Strength of the shake effect

    private void OnGUI()
    {
        // Create a button at the bottom of the screen
        if (GUI.Button(new Rect(10, Screen.height - 50, 150, 30), "Spawn Projectile"))
        {
            // Example transforms; adjust these to fit your needs
            Transform exampleStart = BlackjackUIManager.Instance.player; // Use the current object for example
            Transform exampleEnd = BlackjackUIManager.Instance.enemy;   // Use the current object for example

            // Call the function with example transforms
            SpawnProjectile(exampleStart, exampleEnd);
        }
    }

    public void SpawnProjectile(Transform startTransform, Transform endTransform)
    {
        // Instantiate the object at the specified start transform position
        GameObject obj = Instantiate(prefab, startTransform.position, Quaternion.identity);

        // Create the parabolic path
        Vector3[] path = CreateParabolaPath(startTransform.position, endTransform.position, height);

        // Animate the object along the parabolic path
        obj.transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(easeType)   // Use exposed ease type
            .OnStart(() => BounceTransform(startTransform)) // Bounce start transform
            .OnComplete(() => {
                ShakeTransform(endTransform); // Shake end transform
                Destroy(obj); // Destroy object on completion
            });
    }

    private void BounceTransform(Transform transform)
    {
        // Apply a punch effect to simulate a bounce
        transform.DOPunchPosition(Vector3.up * bounceStrength, bounceDuration, bounceVibrato)
            .SetEase(Ease.OutBounce);
    }

    private void ShakeTransform(Transform transform)
    {
        // Store the original position
        Vector3 originalPosition = transform.position;

        // Apply a shake effect to simulate a shake
        transform.DOShakePosition(shakeDuration, shakeStrength)
            .OnKill(() => transform.position = originalPosition) // Ensure position is reset
            .SetEase(Ease.Linear);
    }

    // Function to create a parabolic path using a set of points
    private Vector3[] CreateParabolaPath(Vector3 start, Vector3 end, float height)
    {
        Vector3 middle = Vector3.Lerp(start, end, 0.5f); // Find the midpoint
        middle.y += height; // Raise the midpoint to create a parabola

        // Return a path array with three points: start, mid, end
        return new Vector3[] { start, middle, end };
    }
}
