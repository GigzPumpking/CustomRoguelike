using UnityEngine;
using System;
using System.Linq;

public class Player : MonoBehaviour
{
    public float speed = 10.0f; // The speed of the player object
    private float horizontal; // The horizontal input from the keyboard
    private float vertical; // The vertical input from the keyboard
    private Rigidbody rb; // The rigidbody of the player object
    [SerializeField] private float groundCheckDistance = 0.65f; // Distance to check below the player
    [SerializeField] private bool isGrounded;
    [SerializeField] private KeyCode[] skillBindings; // The key bindings for the skills

    // Health
    [SerializeField] private float health = 100f;
    private HealthBar healthBar;

    [SerializeField] private bool isInvulnerable = false;

    // Debugging
    [SerializeField] private bool debug = false;

    void Awake()
    {
        // Get the rigidbody component from the player object
        rb = GetComponent<Rigidbody>();

        // Register the player object with the GameManager
        GameManager.Instance.RegisterPlayer(gameObject);

        // Note: GameManager must be initialized before the player object
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
        InputHandler();
        GroundChecker();
    }

    void FixedUpdate()
    {
        MoveHandler();
    }

    void InputHandler()
    {
        // Get the input from the keyboard
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Check for any key press
        if (Input.anyKeyDown)
        {
            // Check if the key pressed is a skill binding
            foreach (KeyCode keyCode in skillBindings)
            {   
                if (Input.GetKeyDown(keyCode))
                {
                    if (debug)
                    {
                        Debug.Log("Key pressed: " + keyCode);
                    }
                    
                    // Raise the KeyPress event
                    EventDispatcher.Raise<KeyPressEvent>(new KeyPressEvent() {
                        keyCode = keyCode
                    });
                }
            }
        }
    }

    void MoveHandler()
    {
        // Move the player object
        transform.Translate(new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime);
    }

    void GroundChecker()
    {
        isGrounded = GameManager.Instance.isObjectGrounded(transform, groundCheckDistance);

        // Debugging
        if (debug)
        {
            Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
        }

        // if below ground, reset player's y position
        if (transform.position.y < -10)
        {
            transform.position = new Vector3(0, 1, 0);
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    void OnCollisionEnter(Collision collision)
    {
        EventDispatcher.Raise<PlayerCollisionEvent>(new PlayerCollisionEvent() {
            collision = collision
        });
    }

    public void AppendSkillBind(KeyCode keyCode)
    {
        // Append the key code to the skill bindings array
        Array.Resize(ref skillBindings, skillBindings.Length + 1);
        skillBindings[skillBindings.Length - 1] = keyCode;
    }

    public void RemoveSkillBind(KeyCode keyCode)
    {
        // Remove the key code from the skill bindings array
        skillBindings = skillBindings.Where(val => val != keyCode).ToArray();
    }

    public KeyCode[] GetSkillBindings()
    {
        // Get the skill bindings array
        return skillBindings;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
        {
            return;
        }

        // Reduce the health of the player object
        health -= damage;

        // Debugging
        if (debug)
        {
            Debug.Log("Player health: " + health);
        }

        // Check if the player object is dead
        if (health <= 0)
        {
            health = 0;
            // Raise event 
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }
    }

    public void TakeKnockback(float knockback, Vector3 direction)
    {
        if (direction == default(Vector3))
        {
            direction = -transform.forward;
        }

        // Apply knockback to the player object
        rb.AddForce(direction * knockback, ForceMode.Impulse);
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float value)
    {
        health = value;
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

}
