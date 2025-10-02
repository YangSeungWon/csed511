using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public string name;
        public PrimitiveType primitiveType;
        public Vector3 scale = Vector3.one;
        public float mass = 1f;
        public float bounciness = 0f;
        public Color color = Color.white;
        public PhysicMaterialCombine bounceCombine = PhysicMaterialCombine.Average;
    }

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnRadius = 2f;

    [Header("Spawnable Objects")]
    [SerializeField] private List<SpawnableObject> spawnableObjects = new List<SpawnableObject>
    {
        new SpawnableObject
        {
            name = "Light Cube",
            primitiveType = PrimitiveType.Cube,
            scale = new Vector3(0.2f, 0.2f, 0.2f),
            mass = 0.5f,
            bounciness = 0.2f,
            color = new Color(1f, 0.8f, 0.6f)
        },
        new SpawnableObject
        {
            name = "Heavy Cube",
            primitiveType = PrimitiveType.Cube,
            scale = new Vector3(0.3f, 0.3f, 0.3f),
            mass = 3f,
            bounciness = 0.1f,
            color = new Color(0.3f, 0.3f, 0.5f)
        },
        new SpawnableObject
        {
            name = "Bouncy Sphere",
            primitiveType = PrimitiveType.Sphere,
            scale = new Vector3(0.25f, 0.25f, 0.25f),
            mass = 1f,
            bounciness = 0.9f,
            color = new Color(1f, 0.2f, 0.2f),
            bounceCombine = PhysicMaterialCombine.Maximum
        },
        new SpawnableObject
        {
            name = "Cylinder",
            primitiveType = PrimitiveType.Cylinder,
            scale = new Vector3(0.15f, 0.3f, 0.15f),
            mass = 1.5f,
            bounciness = 0.3f,
            color = new Color(0.2f, 1f, 0.2f)
        },
        new SpawnableObject
        {
            name = "Capsule",
            primitiveType = PrimitiveType.Capsule,
            scale = new Vector3(0.2f, 0.4f, 0.2f),
            mass = 1.2f,
            bounciness = 0.4f,
            color = new Color(1f, 1f, 0.2f)
        },
        new SpawnableObject
        {
            name = "Small Sphere",
            primitiveType = PrimitiveType.Sphere,
            scale = new Vector3(0.15f, 0.15f, 0.15f),
            mass = 0.3f,
            bounciness = 0.6f,
            color = new Color(0.2f, 0.8f, 1f)
        }
    };

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        // Use XR Origin position as spawn point if not set
        if (spawnPoint == null)
        {
            GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)");
            if (xrOrigin != null)
            {
                spawnPoint = xrOrigin.transform;
            }
            else
            {
                spawnPoint = transform;
            }
        }
    }

    public void SpawnObject(int index)
    {
        if (index < 0 || index >= spawnableObjects.Count)
        {
            Debug.LogWarning($"Invalid spawn index: {index}");
            return;
        }

        SpawnableObject objData = spawnableObjects[index];

        // Calculate random spawn position around spawn point
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = spawnPoint.position +
                                spawnPoint.forward * 2f +
                                new Vector3(randomCircle.x, 1f, randomCircle.y);

        // Create primitive object
        GameObject obj = GameObject.CreatePrimitive(objData.primitiveType);
        obj.name = objData.name;
        obj.transform.position = spawnPosition;
        obj.transform.localScale = objData.scale;
        obj.tag = "Pickupable";

        // Add Rigidbody
        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.mass = objData.mass;
        rb.useGravity = true;

        // Create and apply physics material for bounciness
        PhysicMaterial physicMat = new PhysicMaterial($"{objData.name}_Material");
        physicMat.bounciness = objData.bounciness;
        physicMat.bounceCombine = objData.bounceCombine;

        Collider col = obj.GetComponent<Collider>();
        if (col != null)
        {
            col.material = physicMat;
        }

        // Set color
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            renderer.material.color = objData.color;
        }

        // Add XR Grab Interactable
        XRGrabInteractable grabInteractable = obj.AddComponent<XRGrabInteractable>();
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.throwOnDetach = true;
        grabInteractable.throwSmoothingDuration = 0.15f;

        // Track spawned object
        spawnedObjects.Add(obj);

        Debug.Log($"Spawned {objData.name} at {spawnPosition}");
    }

    public void ClearAllSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
        Debug.Log("Cleared all spawned objects");
    }

    public int GetSpawnableObjectCount()
    {
        return spawnableObjects.Count;
    }

    public string GetObjectName(int index)
    {
        if (index >= 0 && index < spawnableObjects.Count)
        {
            return spawnableObjects[index].name;
        }
        return "Unknown";
    }
}
