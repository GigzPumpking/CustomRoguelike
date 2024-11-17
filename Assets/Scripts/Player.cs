using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f; // The speed of the player object
    private float horizontal; // The horizontal input from the keyboard
    private float vertical; // The vertical input from the keyboard

    // Jump Stats
    private Rigidbody rb; // The rigidbody of the player object
    [SerializeField] private float jumpForce = 10.0f; // The force of the jump
    [SerializeField] private float groundCheckDistance = 0.65f; // Distance to check below the player
    [SerializeField] private bool isGrounded;

    // Slam Stats
    [SerializeField] private float slamForce = 30.0f; // The force of the slam
    [SerializeField] private float slamRadius = 5.0f; // The radius of the slam effect
    private bool isSlamActive = false; // Indicates if the slam effect is waiting to be applied

    // Debugging
    [SerializeField] private bool debug = false;

    void Awake()
    {
        // Get the rigidbody component from the player object
        rb = GetComponent<Rigidbody>();
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                JumpAbility();
            }
            else if (!isSlamActive)
            {
                StartSlam();
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
    }

    void JumpAbility()
    {
        // Add force to the player object upwards
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Debugging
        if (debug)
        {
            Debug.Log("Jumped");
        }
    }

    void StartSlam()
    {
        // Add force to the player object downwards
        rb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);

        // Activate the slam effect
        isSlamActive = true;

        // Debugging
        if (debug)
        {
            Debug.Log("Slam started");
        }
    }

    void ApplySlamEffect()
    {
        // Detect objects in an area below the player
        Collider[] colliders = Physics.OverlapSphere(transform.position, slamRadius);

        // Loop through all the colliders
        foreach (Collider collider in colliders)
        {
            // Check if the collider is a rigidbody
            Rigidbody targetRb = collider.GetComponent<Rigidbody>();

            // Check if the rigidbody is not null and not the player's rigidbody
            if (targetRb != null && targetRb != rb)
            {
                // Add force to the rigidbody
                targetRb.AddForce(Vector3.up * (slamForce / 2), ForceMode.Impulse);
            }
        }

        // Debugging
        if (debug)
        {
            Debug.Log("Slammed");
            Debug.Log($"Slam affected {colliders.Length} objects in radius {slamRadius}");
        }

        // Reset the slam state
        isSlamActive = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player collided with the ground
        if (GameManager.Instance.isObjectGrounded(transform, groundCheckDistance))
        {
            // Apply the slam effect if it is active
            if (isSlamActive)
            {
                ApplySlamEffect();
            }
        }
    }

    // Draw gizmos for the slam area in the editor
    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Semi-transparent red
            Gizmos.DrawSphere(transform.position, slamRadius); // Draw the slam effect radius
        }
    }
}
