using UnityEngine;

public class ExplosionPool : ObjectPool<Explosion>
{
    public static ExplosionPool Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    /// <summary>
    /// Spawns an explosion of a specific type at a given position.
    /// </summary>
    /// <param name="prefab">The explosion prefab to spawn.</param>
    /// <param name="position">The position to spawn the explosion.</param>
    public void Explode(Explosion prefab, Vector3 position)
    {
        Explosion explosion = GetObject(prefab, position, Quaternion.identity);
        if (explosion != null)
        {
            explosion.gameObject.SetActive(true);
            explosion.Trigger(); // Ensure the explosion behaves as expected (e.g., animation, sound, etc.)
        }
    }
}
