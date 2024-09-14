using UnityEngine;
using DG.Tweening; // DOTween namespace
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
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

    public ObjectPool<GameObject> shotPool;
    public ObjectPool<GameObject> muzzleFlashPool;
    public ObjectPool<GameObject> impactFlashPool;

    private void OnGUI()
    {
        // Create a button at the bottom of the screen
        if (GUI.Button(new Rect(10, Screen.height - 50, 150, 30), "Spawn Projectile"))
        {
            // Example transforms; adjust these to fit your needs
            Transform exampleStart = BlackjackStateMachine.Instance.player; // Use the current object for example
            Transform exampleEnd = BlackjackStateMachine.Instance.enemy;   // Use the current object for example

            // Call the function with example transforms
            StartCoroutine(SpawnProjectileCoroutine(exampleStart, exampleEnd, 1, true));
        }
    }

    void Awake()
    {
        DOTween.debugStoreTargetId = true;  // Enables the storage of target object IDs for debug purposes
        DOTween.debugMode = true;  // Enables verbose debugging logs

        // Initialize shot pool
        shotPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(shotPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: 21,
            maxSize: 30
        );

        // Initialize muzzle flash pool
        muzzleFlashPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(muzzleFlashes[Random.Range(0, muzzleFlashes.Count)]),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: 21,
            maxSize: 30
        );

        // Initialize impact flash pool
        impactFlashPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(impactFlashes[0]),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: 21,
            maxSize: 30
        );
    }

    public void SpawnProjectile(Transform startPos, Transform endPos, int count = 1, bool isPlayer = false)
    {
        StartCoroutine(SpawnProjectileCoroutine(startPos,endPos, count, isPlayer));
    }


    public List<GameObject> muzzleFlashes = new List<GameObject>();
    public List<GameObject> impactFlashes = new List<GameObject>();
    public IEnumerator SpawnProjectileCoroutine(Transform startTransform, Transform endTransform, int count = 1, bool isPlayer = false)
    {
        for (int i = 0; i < count; i++)
        {

            // Get projectile from pool instead of instantiating it
            GameObject obj = shotPool.Get();
            obj.transform.position = startTransform.position;
            obj.transform.rotation = Quaternion.identity;

            // Get muzzle flash from pool
            GameObject muzzleflash = muzzleFlashPool.Get();
            muzzleflash.transform.position = startTransform.position;

            // Create the parabolic path
            Vector3[] path = CreateParabolaPath(startTransform.position, endTransform.position, height);

            // Start the animation along the parabolic path
            obj.transform.DOPath(path, duration, PathType.CatmullRom)
                .SetEase(easeType)   // Use exposed ease type
                .SetLookAt(0.01f)
                .OnStart(() =>
                {
                    BounceTransform(startTransform);
                    AudioManager.Instance.PlayBulletFireSound();
                })
                .OnComplete(() =>
                {
                    ShakeTransform(endTransform); // Shake end transform
                    BlackjackUIManager.Instance.ShakeCamera();

                    // Get impact flash from pool
                    GameObject impactflash = impactFlashPool.Get();
                    impactflash.transform.position = endTransform.position;

                    AudioManager.Instance.PlayHitSound();

                    // Return objects to pool instead of destroying
                    shotPool.Release(obj);
                    muzzleFlashPool.Release(muzzleflash);
                    //impactFlashPool.Release(impactflash);

                    StartCoroutine(ReleaseAfterDuration(impactflash, 1f)); // 1 second fallback

                }).SetAutoKill(false);

            // Wait for the next projectile spawn
            yield return new WaitForSeconds(.1f);
        }
    }
    private IEnumerator ReleaseAfterDuration(GameObject obj, float delay)
    {
        // Wait for the given duration (e.g., the duration of the particle system)
        yield return new WaitForSeconds(delay);

        // Release the object back to the pool
        impactFlashPool.Release(obj);
    }



    private void BounceTransform(Transform transform)
    {
        // Apply a punch effect to simulate a bounce
        transform.DOPunchPosition(Vector3.up * bounceStrength, bounceDuration, bounceVibrato)
            .SetEase(Ease.OutBounce)
            .OnStart(() =>
            {
                transform.localPosition = Vector3.zero;
            })
            .OnComplete(() => 
            {
                transform.localPosition = Vector3.zero;
            });
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
