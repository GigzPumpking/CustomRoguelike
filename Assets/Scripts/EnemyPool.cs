using UnityEngine;

public class EnemyPool : ObjectPool<Enemy>
{
    public static EnemyPool Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    /// <summary>
    /// Spawns an enemy of a specific type at a given position.
    /// </summary>
    /// <param name="prefab">The enemy prefab to spawn.</param>
    /// <param name="position">The position to spawn the enemy.</param>
    public void SpawnEnemy(Enemy prefab, Vector3 position)
    {
        Enemy enemy = GetObject(prefab, position, Quaternion.identity);
        if (enemy != null)
        {
            enemy.SetTarget(GameManager.Instance.GetPlayer().transform);
        }
    }

    // <summary>
    /// Spawns an enemy of a specific type at a given position.
    /// <summary>
    /// <param name="index">The index of the enemy prefab to spawn.</param>
    /// <param name="position">The position to spawn the enemy.</param>
    public void SpawnEnemy(int index, Vector3 position)
    {
        Enemy enemy = GetObject(index, position, Quaternion.identity);
        if (enemy != null)
        {
            enemy.SetTarget(GameManager.Instance.GetPlayer().transform);
        }
    }

    /// <summary>
    /// Despawns the specified enemy, returning it to the pool.
    /// </summary>
    /// <param name="prefab">The prefab type of the enemy.</param>
    /// <param name="enemy">The enemy instance to despawn.</param>
    public void DespawnEnemy(Enemy prefab, Enemy enemy)
    {
        ReturnObject(prefab, enemy);
    }
}
