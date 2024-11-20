using UnityEngine;

public class Explosion : MonoBehaviour, IPoolable<Explosion>
{
    private Animator animator;

    private Explosion prefab;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Use billboarding to keep the explosion facing the camera
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }

    public void Trigger()
    {
        // Play the explosion animation
        animator.SetTrigger("Explode");
    }

    public void SetPrefab(Explosion explosionPrefab)
    {
        prefab = explosionPrefab;
    }

    public Explosion GetPrefab()
    {
        return prefab;
    }
}
