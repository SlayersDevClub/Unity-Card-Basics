using UnityEngine;
using DG.Tweening; // DOTween namespace
using System.Collections;
using System.Collections.Generic;

public class AttackFXMover : MonoBehaviour
{
    public GameObject shotPrefab, muzzleFlashPrefab, impactPrefab;           // The shotPrefab to instantiate and animate
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
            Transform exampleStart = BlackjackStateMachine.Instance.player; // Use the current object for example
            Transform exampleEnd = BlackjackStateMachine.Instance.enemy;   // Use the current object for example

            // Call the function with example transforms
            StartCoroutine(SpawnProjectileCoroutine(exampleStart, exampleEnd));
        }
    }

    public void SpawnProjectile(Transform startPos, Transform endPos, int count = 1, bool isPlayer = false)
    {
        StartCoroutine(SpawnProjectileCoroutine(startPos,endPos, count, isPlayer));
    }


    public List<GameObject> muzzleFlashes = new List<GameObject>();
    public List<GameObject> impactFlashes = new List<GameObject>();
    public IEnumerator SpawnProjectileCoroutine(Transform startTransform, Transform endTransform, int count = 1, bool isPlayer = false)
    {
        for(int i = 0; i < count; i++)
        {

            // Instantiate the object at the specified start transform position
            GameObject obj = Instantiate(shotPrefab, startTransform.position, Quaternion.identity);

            int randomIndex = Random.Range(0, muzzleFlashes.Count);
            GameObject muzzleflash = Instantiate(muzzleFlashes[randomIndex], startTransform.position + new Vector3(0, 0.01f, 0), Quaternion.identity);

            // Create the parabolic path
            Vector3[] path = CreateParabolaPath(startTransform.position, endTransform.position, height);

            // Start the animation along the parabolic path
            obj.transform.DOPath(path, duration, PathType.CatmullRom)
                .SetEase(easeType)   // Use exposed ease type
                .OnStart(() => BounceTransform(startTransform)) // Bounce start transform
                .OnComplete(() =>
                {
                    ShakeTransform(endTransform); // Shake end transform
                    Destroy(obj); // Destroy object on completion
                    BlackjackUIManager.Instance.ShakeCamera();

                    if(isPlayer)
                    {
                        BlackjackUIManager.Instance.RemoveOneObjectFromList(true);
                    } 
                    else 
                    {
                        BlackjackUIManager.Instance.RemoveOneObjectFromList(false);
                    }

                    int randomIndex = Random.Range(0, impactFlashes.Count);
                    Instantiate(impactFlashes[randomIndex], endTransform.position + new Vector3(0,0.01f,0), Quaternion.identity);
                });
        // Yield until the animation duration is complete
        yield return new WaitForSeconds(.1f);
        }
    }




    private void BounceTransform(Transform transform)
    {
        // Apply a punch effect to simulate a bounce
        transform.DOPunchPosition(Vector3.up * bounceStrength, bounceDuration, bounceVibrato)
            .SetEase(Ease.OutBounce);
    }
    Tween shake;
    Vector3 startShakePos;
    private void ShakeTransform(Transform transform)
    {
        Vector3 originalPosition = transform.position;
        if(shake == null) shake = transform.DOShakePosition(shakeDuration, shakeStrength).SetEase(Ease.Linear).SetAutoKill(true).OnComplete(() =>
        {
            transform.localPosition = Vector3.zero;
            shake = null;

        }); 

        shake.Rewind();
        shake.Play();
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
