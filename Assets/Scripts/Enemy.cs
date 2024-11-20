using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f; // The health of the enemy object

    private HealthBar healthBar; // The health bar of the enemy object

    [SerializeField] Animator animator; // The animator of the enemy object

    private Transform target; // The target object

    [SerializeField] private float speed = 5f; // The speed of the enemy object

    [SerializeField] private float damage = 10f; // The damage dealt to the player

    [SerializeField] private bool selfRecoil = true; // Whether the enemy should recoil when colliding with the player

    [SerializeField] private float recoilForce = 10f; // The force applied to the enemy when it collides with the player

    [SerializeField] private float knockback = 10f; // The force applied to the player when it collides with the enemy

    private float knockbackRecoveryTime = 10f; // The maximum time it takes for the player to recover from knockback

    private float stillThreshold = 0.1f; // The threshold for the player to be considered still

    private bool canMove = true; // Whether the enemy can move

    private Rigidbody rb; // The rigidbody of the enemy object

    void OnEnable()
    {
        // Listen to player registered event
        EventDispatcher.AddListener<PlayerRegisteredEvent>(OnPlayerRegistered);
    }

    void OnDisable()
    {
        // Stop listening to player registered event
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
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBar>();
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        if (target == null || !canMove)
        {
            return;
        }

        // Rotate towards the target
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);

        // Move towards the target
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Damage the player
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            collision.gameObject.GetComponent<Player>().TakeKnockback(knockback, transform.forward);

            if (selfRecoil)
            {
                TakeKnockback(recoilForce, -transform.forward);
            }
        }
    }

    public void DisableRigidbody() {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void EnableRigidbody() {
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Die();
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }
    }

    public void TakeKnockback(float knockback, Vector3 direction)
    {
        if (rb != null)
        {
            rb.AddForce(direction * knockback, ForceMode.Impulse);
            canMove = false;

            // Start a coroutine to recover from knockback
            StartCoroutine(RecoverFromKnockback());
        }
    }

    IEnumerator RecoverFromKnockback()
    {
        // Wait for maximum recovery time or until the enemy is still
        float timer = 0f;
        while (timer < knockbackRecoveryTime && rb.linearVelocity.magnitude > stillThreshold)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canMove = true;
    }

    void Die()
    {
        EventDispatcher.Raise<EnemyDeathEvent>(new EnemyDeathEvent { enemy = this.gameObject });

        if (ExplosionPool.Instance != null)
        {
            ExplosionPool.Instance.Explode(transform.position + Vector3.up);
        }

        Destroy(gameObject);
    }
}
