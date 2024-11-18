using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] private T prefab; // Prefab to instantiate
    [SerializeField] private int initialPoolSize = 10; // Initial number of objects in the pool
    [SerializeField] private bool expandPool = true; // Allow the pool to expand if needed

    private Queue<T> pool = new Queue<T>();

    protected virtual void Awake()
    {
        InitializePool();
    }

    /// <summary>
    /// Initializes the pool with the specified number of objects.
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateObject();
        }
    }

    /// <summary>
    /// Creates a new object, adds it to the pool, and disables it.
    /// </summary>
    /// <returns>The newly created object.</returns>
    private T CreateObject()
    {
        T newObject = Instantiate(prefab, transform);
        newObject.gameObject.SetActive(false);
        pool.Enqueue(newObject);
        return newObject;
    }

    /// <summary>
    /// Gets an object from the pool, enabling it and optionally moving it to a specific location.
    /// </summary>
    /// <param name="position">The position to place the object.</param>
    /// <param name="rotation">The rotation to apply to the object.</param>
    /// <returns>The object from the pool.</returns>
    public T GetObject(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
        {
            if (expandPool)
            {
                CreateObject();
            }
            else
            {
                Debug.LogWarning("Object pool is empty and cannot expand.");
                return null;
            }
        }

        T objectFromPool = pool.Dequeue();
        objectFromPool.transform.position = position;
        objectFromPool.transform.rotation = rotation;
        objectFromPool.gameObject.SetActive(true);
        return objectFromPool;
    }

    /// <summary>
    /// Returns an object to the pool, disabling it and adding it back to the queue.
    /// </summary>
    /// <param name="objectToReturn">The object to return to the pool.</param>
    public void ReturnObject(T objectToReturn)
    {
        objectToReturn.gameObject.SetActive(false);
        pool.Enqueue(objectToReturn);
    }
}
