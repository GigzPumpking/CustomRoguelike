using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component, IPoolable<T>
{
    [System.Serializable]
    public struct PoolSettings
    {
        public T prefab;           // The prefab to instantiate
        public int initialSize;    // Initial size of the pool for this prefab
    }

    [SerializeField] private List<PoolSettings> poolSettingsList; // List of prefabs and their initial pool sizes
    [SerializeField] private bool expandPool = true;             // Allow the pool to expand if needed

    private Dictionary<T, Queue<T>> pools = new Dictionary<T, Queue<T>>();

    protected virtual void Awake()
    {
        InitializePools();
    }

    /// <summary>
    /// Initializes the pools for all prefabs.
    /// </summary>
    private void InitializePools()
    {
        foreach (var settings in poolSettingsList)
        {
            Queue<T> poolQueue = new Queue<T>();

            for (int i = 0; i < settings.initialSize; i++)
            {
                T newObject = CreateObject(settings.prefab);
                poolQueue.Enqueue(newObject);
            }

            pools.Add(settings.prefab, poolQueue);
        }
    }

    /// <summary>
    /// Creates a new object, disables it, and returns it.
    /// </summary>
    /// <param name="prefab">The prefab to instantiate.</param>
    /// <returns>The newly created object.</returns>
    private T CreateObject(T prefab)
    {
        T newObject = Instantiate(prefab, transform);
        newObject.gameObject.SetActive(false);
        return newObject;
    }

    /// <summary>
    /// Gets an object from the pool associated with the specified prefab.
    /// </summary>
    /// <param name="prefab">The prefab type to get from the pool.</param>
    /// <param name="position">The position to place the object.</param>
    /// <param name="rotation">The rotation to apply to the object.</param>
    /// <returns>The object from the pool.</returns>
    public T GetObject(T prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefab))
        {
            Debug.LogError($"No pool exists for prefab: {prefab.name}");
            return null;
        }

        Queue<T> poolQueue = pools[prefab];

        T objectFromPool;

        if (poolQueue.Count == 0)
        {
            if (expandPool)
            {
                objectFromPool = CreateObject(prefab);
            }
            else
            {
                Debug.LogWarning($"Pool for prefab {prefab.name} is empty and cannot expand.");
                return null;
            }
        }
        else
        {
            objectFromPool = poolQueue.Dequeue();
        }

        objectFromPool.transform.position = position;
        objectFromPool.transform.rotation = rotation;
        objectFromPool.gameObject.SetActive(true);

        // Call SetPrefab to store the prefab reference
        objectFromPool.SetPrefab(prefab);

        return objectFromPool;
    }

    /// <summary>
    /// Gets an object from the pool associated with the specified prefab index.
    /// </summary>
    /// <param name="index">The index of the prefab to get from the pool.</param>
    /// <param name="position">The position to place the object.</param>

    public T GetObject(int index, Vector3 position, Quaternion rotation)
    {
        if (index < 0 || index >= poolSettingsList.Count)
        {
            Debug.LogError($"Index {index} is out of range.");
            return null;
        }

        return GetObject(poolSettingsList[index].prefab, position, rotation);
    }

    /// <summary>
    /// Gets a prefab from the poolSettingsList.
    /// </summary>
    /// <param name="index">The index of the prefab to get from the poolSettingsList.</param>
    public T GetPrefab(int index)
    {
        if (index < 0 || index >= poolSettingsList.Count)
        {
            Debug.LogError($"Index {index} is out of range.");
            return null;
        }

        // Just get the prefab without instantiating it from the poolSettingsList
        return poolSettingsList[index].prefab;
    }

    /// <summary>
    /// Returns an object to the pool associated with its prefab.
    /// </summary>
    /// <param name="prefab">The prefab type of the object.</param>
    /// <param name="objectToReturn">The object to return to the pool.</param>
    public void ReturnObject(T prefab, T objectToReturn)
    {
        if (!pools.ContainsKey(prefab))
        {
            Debug.LogError($"No pool exists for prefab: {prefab.name}");
            Destroy(objectToReturn.gameObject);
            return;
        }

        objectToReturn.gameObject.SetActive(false);
        pools[prefab].Enqueue(objectToReturn);
    }

    /// <summary>
    /// Returns length of the poolSettingsList.
    /// </summary>
    public int GetPoolLength()
    {
        return poolSettingsList.Count;
    }
}
