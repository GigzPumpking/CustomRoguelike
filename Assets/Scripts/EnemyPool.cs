using UnityEngine;

public class EnemyPool : ObjectPool<Enemy>
{
    public static EnemyPool Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void SpawnEnemy(Vector3 position)
    {
        Enemy enemy = GetObject(position, Quaternion.identity);
        enemy.gameObject.SetActive(true);

        enemy.SetTarget(GameManager.Instance.GetPlayer().transform);
    }
}
