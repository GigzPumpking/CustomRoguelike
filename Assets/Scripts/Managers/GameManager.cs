using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    [SerializeField] private LayerMask groundLayer; // Specify what layers count as ground

    // Debugging
    [SerializeField] private bool debug = false;

    // Devmode

    [SerializeField] private bool devmode = false;

    [SerializeField] private bool SpawnOnLoad = true;

    // KeyCode for quitting the game
    [SerializeField] private KeyCode quitKey = KeyCode.Q;

    [SerializeField] private KeyCode[] spawnEnemyKeys = { KeyCode.Alpha1 };

    private GameObject player;

    private InputManager inputManager;

    private EnemyPool enemyPool;

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

        // Register the spawn enemy keys with the InputManager
        foreach (KeyCode keyCode in spawnEnemyKeys)
        {
            inputManager.RegisterKey(keyCode);
        }

        enemyPool = GetComponent<EnemyPool>();
    }

    void Start() {

        if (SpawnOnLoad)
        {
            // Call EnemyPool to spawn enemies in random locations
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                EnemyPool.Instance.SpawnEnemy(0, randomPosition);
            }
        }
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
            return;
        }

        if (devmode) {
            // Check if the spawn enemy keys are pressed
            foreach (KeyCode keyCode in spawnEnemyKeys)
            {
                if (e.keyCode == keyCode)
                {
                    // Spawn an enemy at a random location
                    Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                    // Spawn an enemy based on the index of the key code in the spawnEnemyKeys array
                    enemyPool.SpawnEnemy(keyCode - spawnEnemyKeys[0], randomPosition);
                    return;
                }
            }
        }
    }

    public void RegisterKey(KeyCode keyCode)
    {
        // Register the key with the InputManager
        inputManager.RegisterKey(keyCode);
    }

    public KeyCode[] GetSpawnEnemyKeys()
    {
        return spawnEnemyKeys;
    }

    public bool isDevmode()
    {
        return devmode;
    }
}
