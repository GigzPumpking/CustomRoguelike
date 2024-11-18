using UnityEngine;

public class ExplosionPool : ObjectPool<Explosion>
{
    public static ExplosionPool Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void Explode(Vector3 position)
    {
        Explosion explosion = GetObject(position, Quaternion.identity);
        explosion.gameObject.SetActive(true);
    }
}
