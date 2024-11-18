using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    [SerializeField] private LayerMask groundLayer; // Specify what layers count as ground

    // Debugging
    [SerializeField] private bool debug = false;

    // KeyCode for quitting the game
    [SerializeField] private KeyCode quitKey = KeyCode.Q;

    private GameObject player;

    private InputManager inputManager;

    void OnEnable()
    {
        // Subscribe to the KeyPressEvent
        EventDispatcher.AddListener<KeyPressEvent>(OnKeyPress);
    }

    void OnDisable()
    {
        // Unsubscribe from the KeyPressEvent
        EventDispatcher.RemoveListener<KeyPressEvent>(OnKeyPress);
    }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another instance of GameManager exists. Destroying this one.");
            Destroy(gameObject); // Destroy this instance if it's not the first
            return;
        }

        // Set the instance to this GameObject
        Instance = this;

        // Prevent this GameObject from being destroyed when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Set the input manager from this GameObject
        inputManager = GetComponent<InputManager>();

        // Initialize the InputManager if it doesn't exist
        if (inputManager == null)
        {
            inputManager = gameObject.AddComponent<InputManager>();
        }

        // Register the quit key with the InputManager

        inputManager.RegisterKey(quitKey);
    }

    public bool isObjectGrounded(Transform gameObject, float distance)
    {
        // Check if the object is grounded
        return Physics.Raycast(gameObject.position, Vector3.down, distance, groundLayer);
    }

    public void RegisterPlayer(GameObject playerObject)
    {
        player = playerObject;

        if (debug)
        {
            Debug.Log("Player registered.");
        }

        // Raise the PlayerRegistered event
        EventDispatcher.Raise<PlayerRegisteredEvent>(new PlayerRegisteredEvent() {
            player = playerObject
        });
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    void OnKeyPress(KeyPressEvent e)
    {
        // Check if the quit key is pressed
        if (e.keyCode == quitKey)
        {
            // Quit the game
            Application.Quit();
        }
    }

    public void RegisterKey(KeyCode keyCode)
    {
        // Register the key with the InputManager
        inputManager.RegisterKey(keyCode);
    }
}
