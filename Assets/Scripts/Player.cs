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

    // Health
    [SerializeField] private float currHealth = 100f;

    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private bool isInvulnerable = false;

    // Debugging
    [SerializeField] private bool debug = false;

    void Awake()
    {
        // Get the rigidbody component from the player object
        rb = GetComponent<Rigidbody>();

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found");
            return;
        }

        // Register the player object with the GameManager
        GameManager.Instance.RegisterPlayer(gameObject);
    }
    
    void Start()
    {
        UIManager.Instance.UpdatePlayerMaxHealth(maxHealth);
        UIManager.Instance.UpdatePlayerHealth(currHealth);
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

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
        {
            return;
        }

        // Reduce the health of the player object
        currHealth -= damage;

        // Debugging
        if (debug)
        {
            Debug.Log("Player health: " + currHealth);
        }

        // Check if the player object is dead
        if (currHealth <= 0)
        {
        }

        UIManager.Instance.UpdatePlayerHealth(currHealth);
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
        return currHealth;
    }

    public void SetHealth(float value)
    {
        currHealth = value;
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
