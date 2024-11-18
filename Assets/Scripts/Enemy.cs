using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f; // The health of the enemy object

    private HealthBar healthBar; // The health bar of the enemy object

    private Transform target; // The target object

    private Rigidbody rb; // The rigidbody of the enemy object

    [SerializeField] private float speed = 5f; // The speed of the enemy object

    [SerializeField] private float damage = 10f; // The damage dealt to the player

    [SerializeField] private bool selfRecoil = true; // Whether the enemy should recoil when colliding with the player

    [SerializeField] private float recoilForce = 10f; // The force applied to the enemy when it collides with the player

    [SerializeField] private float knockback = 10f; // The force applied to the player when it collides with the enemy

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
        target = e.player.transform;
    }

    void Awake()
    {
        // Get the rigidbody component from the enemy object
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
        if (target == null)
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
            collision.gameObject.GetComponent<Player>().TakeDamage(damage, knockback, transform.forward);

            if (selfRecoil)
            {
                Recoil();
            }
        }
    }

    void Recoil()
    {
        rb.AddForce(-transform.forward * recoilForce, ForceMode.Impulse);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }
    }
}