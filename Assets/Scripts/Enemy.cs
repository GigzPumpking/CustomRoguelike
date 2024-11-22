using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour, IPoolable<Enemy>
{
    [SerializeField] private string name = "Enemy"; // Name of the enemy
    [SerializeField] private string filename = "Enemy"; // Filename for enemy resources
    private CustomSprite customSprite;

    [SerializeField] private float maxHealth = 100f; // Maximum health of the enemy
    [SerializeField] private float health = 100f;   // Current health of the enemy
    [SerializeField] private float speed = 5f;      // Movement speed of the enemy
    [SerializeField] private float damage = 10f;    // Damage dealt by the enemy
    [SerializeField] private float recoilForce = 10f;  // Recoil force on the enemy
    [SerializeField] private float knockback = 10f;    // Knockback force on the player

    private HealthBar healthBar;
    private Animator animator;

    private Enemy prefab;

    protected Transform target;

    private float knockbackRecoveryTime = 10f; // Time to recover from knockback
    private float stillThreshold = 0.1f;      // Threshold to determine if the enemy is still

    protected bool canMove = true;

    private Rigidbody rb;

    [SerializeField] private Explosion explosion;

    // Properties for stats
    protected virtual float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = Mathf.Max(1f, value); // Ensure maxHealth is at least 1
    }

    protected virtual float Health
    {
        get => health;
        set {
            health = Mathf.Clamp(value, 0f, MaxHealth); // Clamp health between 0 and maxHealth

            if (health <= 0)
            {
                Die();
            }

            if (healthBar != null)
            {
                healthBar.SetHealth(Health);
            }
        }
    }

    protected virtual float Speed
    {
        get => speed;
        set => speed = Mathf.Max(0f, value); // Ensure speed is non-negative
    }

    protected virtual float Damage
    {
        get => damage;
        set => damage = Mathf.Max(0f, value); // Ensure damage is non-negative
    }

    protected virtual float RecoilForce
    {
        get => recoilForce;
        set => recoilForce = Mathf.Max(0f, value); // Ensure recoil force is non-negative
    }

    protected virtual float Knockback
    {
        get => knockback;
        set => knockback = Mathf.Max(0f, value); // Ensure knockback is non-negative
    }

    void OnEnable()
    {
        EventDispatcher.AddListener<PlayerRegisteredEvent>(OnPlayerRegistered);
    }

    void OnDisable()
    {
        EventDispatcher.RemoveListener<PlayerRegisteredEvent>(OnPlayerRegistered);
    }

    void OnPlayerRegistered(PlayerRegisteredEvent e)
    {
        SetTarget(e.player.transform);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        InitializeHealthBar();
        InitializeCustomSprite();
        InitializeAnimator();
    }

    private void InitializeHealthBar()
    {
        healthBar ??= GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(MaxHealth);
            healthBar.SetHealth(Health);
        }
    }

    private void InitializeCustomSprite()
    {
        customSprite ??= GetComponentInChildren<CustomSprite>();
        if (customSprite != null)
        {
            customSprite.SetFilename(filename);
        }
    }

    private void InitializeAnimator()
    {
        animator ??= GetComponent<Animator>();
    }


    void Update()
    {
        MoveTowardsTarget();
    }

    protected virtual void MoveTowardsTarget()
    {
        if (target == null || !canMove)
        {
            return;
        }

        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);

        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerCollision(collision);
        }
    }

    protected virtual void OnPlayerCollision(Collision collision)
    {
        collision.gameObject.GetComponent<Player>().TakeDamage(Damage);
        collision.gameObject.GetComponent<Player>().TakeKnockback(Knockback, transform.forward);

        if (RecoilForce > 0)
        {
            TakeKnockback(RecoilForce, -transform.forward);
        }
    }

    public void DisableRigidbody()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void EnableRigidbody()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
    }

    public void TakeKnockback(float knockbackForce, Vector3 direction)
    {
        if (rb != null)
        {
            rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
            canMove = false;

            if (Health > 0)
            {
                StartCoroutine(RecoverFromKnockback());
            }
        }
    }

    private IEnumerator RecoverFromKnockback()
    {
        float timer = 0f;
        while (timer < knockbackRecoveryTime && rb.linearVelocity.magnitude > stillThreshold)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canMove = true;
    }

    public void SetPrefab(Enemy prefab)
    {
        this.prefab = prefab;
    }

    public Enemy GetPrefab()
    {
        return prefab;
    }

    public string GetFilename()
    {
        return filename;
    }

    public string GetName()
    {
        return name;
    }

    protected virtual void Die()
    {
        EventDispatcher.Raise<EnemyDeathEvent>(new EnemyDeathEvent { enemy = this.gameObject });

        Explode();

        if (EnemyPool.Instance != null)
        {
            EnemyPool.Instance.ReturnObject(prefab, this);
        }
    }

    protected virtual void Explode() 
    {
        if (ExplosionPool.Instance != null)
        {
            if (explosion == null)
            {
                explosion = ExplosionPool.Instance.GetObject(0, Vector3.zero, Quaternion.identity);
            }

            ExplosionPool.Instance.Explode(explosion, transform.position + Vector3.up);
        }
    }
}
