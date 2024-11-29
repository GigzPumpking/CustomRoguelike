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

    /// <summary>
    /// Spawns an enemy of a specific type at a given position.
    /// </summary>
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
    /// Spawns an enemy of a specific type by its name.
    /// </summary>
    /// <param name="name">The name of the enemy to spawn.</param>
    /// <param name="count">The number of enemies to spawn.</param>
    /// <returns>True if the enemy was successfully spawned, false otherwise.</returns>
    public bool SpawnEnemy(string name, int count)
    {
        // Find the prefab with the specified name
        Enemy prefab = FindEnemyPrefabByName(name);

        if (prefab == null)
        {
            Debug.LogError($"Enemy prefab not found for name: {name}");
            return false;
        }

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(prefab, new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)));
        }

        return true;
    }

    /// <summary>
    /// Finds an enemy prefab by its name.
    /// </summary>
    /// <param name="name">The name of the enemy to find.</param>
    /// <returns>The enemy prefab if found, or null if not found.</returns>
    private Enemy FindEnemyPrefabByName(string name)
    {
        foreach (var settings in poolSettingsList)
        {
            if (settings.prefab.GetName().ToLower() == name)
            {
                return settings.prefab;
            }
        }

        return null; // Return null if no matching prefab is found
    }
}
