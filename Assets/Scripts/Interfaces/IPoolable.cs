using UnityEngine;

public interface IPoolable<T> where T : Component
{
    void SetPrefab(T prefab);
    T GetPrefab();
}
