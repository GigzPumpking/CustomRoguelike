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

}
