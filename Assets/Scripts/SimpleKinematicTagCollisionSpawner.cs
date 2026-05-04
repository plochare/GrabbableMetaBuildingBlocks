using UnityEngine;

/// <summary>
/// Simple contact checker for two specific tagged GameObjects.
///
/// Use this when both objects have kinematic Rigidbodies and non-trigger Colliders.
/// In that setup, Unity's normal OnCollisionEnter may not fire between two
/// kinematic bodies, so this script manually checks when the two Colliders begin
/// touching/overlapping.
///
/// The prefab is spawned only once when contact starts. It will not spawn again
/// while the objects remain touching, and it will not spawn during collision exit
/// jitter. The script only rearms after the objects have been fully separated for
/// a few FixedUpdate frames.
///
/// Attach this script to an empty manager GameObject in the scene.
/// </summary>
public class SimpleKinematicTagCollisionSpawner : MonoBehaviour
{
    [Header("Objects To Check")]
    [SerializeField] private GameObject objectA;
    [SerializeField] private string objectATag = "ObjectA";

    [SerializeField] private GameObject objectB;
    [SerializeField] private string objectBTag = "ObjectB";

    [Header("Prefab Spawn")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float destroyAfterSeconds = 2f;

    [Header("Contact Start Settings")]
    [Tooltip("Minimum overlap depth needed before this counts as a contact start. Increase slightly if it spawns while objects are barely separating.")]
    [SerializeField] private float minimumPenetrationToStart = 0.001f;

    [Tooltip("How many FixedUpdate frames the objects must be separated before another contact can spawn a prefab.")]
    [SerializeField] private int separatedFramesToRearm = 3;

    private Collider colliderA;
    private Collider colliderB;

    private bool hasSpawnedForCurrentTouch;
    private int separatedFrameCount;

    private void Awake()
    {
        if (objectA != null)
        {
            colliderA = objectA.GetComponent<Collider>();
        }

        if (objectB != null)
        {
            colliderB = objectB.GetComponent<Collider>();
        }
    }

    private void FixedUpdate()
    {
        if (!SetupIsValid())
        {
            return;
        }

        Vector3 contactPoint;
        bool isTouching = TryGetContactPoint(out contactPoint);

        if (isTouching)
        {
            separatedFrameCount = 0;

            if (!hasSpawnedForCurrentTouch)
            {
                SpawnPrefab(contactPoint);
                hasSpawnedForCurrentTouch = true;
            }
        }
        else
        {
            separatedFrameCount++;

            if (separatedFrameCount >= separatedFramesToRearm)
            {
                hasSpawnedForCurrentTouch = false;
            }
        }
    }

    private bool SetupIsValid()
    {
        if (objectA == null || objectB == null || prefabToSpawn == null)
        {
            return false;
        }

        if (colliderA == null || colliderB == null)
        {
            print("SimpleKinematicTagCollisionSpawner: Object A and Object B must both have Colliders.");
            return false;
        }

        if (!objectA.CompareTag(objectATag) || !objectB.CompareTag(objectBTag))
        {
            return false;
        }

        return true;
    }

    private bool TryGetContactPoint(out Vector3 contactPoint)
    {
        contactPoint = Vector3.zero;

        if (!colliderA.bounds.Intersects(colliderB.bounds))
        {
            return false;
        }

        Vector3 direction;
        float distance;

        bool isOverlapping = Physics.ComputePenetration(
            colliderA,
            colliderA.transform.position,
            colliderA.transform.rotation,
            colliderB,
            colliderB.transform.position,
            colliderB.transform.rotation,
            out direction,
            out distance
        );

        if (!isOverlapping || distance < minimumPenetrationToStart)
        {
            return false;
        }

        Vector3 pointOnA = colliderA.ClosestPoint(colliderB.bounds.center);
        Vector3 pointOnB = colliderB.ClosestPoint(colliderA.bounds.center);
        contactPoint = (pointOnA + pointOnB) * 0.5f;

        return true;
    }

    private void SpawnPrefab(Vector3 contactPoint)
    {
        GameObject spawnedPrefab = Instantiate(prefabToSpawn, contactPoint, Quaternion.identity);
        Destroy(spawnedPrefab, destroyAfterSeconds);
    }
}
