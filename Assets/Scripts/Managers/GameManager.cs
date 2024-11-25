using UnityEngine;

public class GameManager : MonoBehaviour, IKeyActionReceiver
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [SerializeField] private LayerMask groundLayer; // Specify what layers count as ground
    [SerializeField] private bool debug = false; // Debugging
    [SerializeField] private bool devmode = false; // Devmode
    [SerializeField] private bool SpawnOnLoad = true; // Spawn enemies on load
    [SerializeField] private KeyCode quitKey = KeyCode.Q; // Quit key
    [SerializeField] private KeyCode[] spawnEnemyKeys = { KeyCode.Alpha1 }; // Keys to spawn enemies

    private GameObject player;

    private void Awake()
    {
        // Ensure singleton instance
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another instance of GameManager exists. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager not found");
            return;
        }

        // Register key actions with InputManager
        InputManager.Instance.AddKeyBind(this, quitKey, "QuitGame");
        for (int i = 0; i < spawnEnemyKeys.Length; i++)
        {
            InputManager.Instance.AddKeyBind(this, spawnEnemyKeys[i], $"SpawnEnemy_{i}");
        }
    }

    private void Start()
    {
        if (SpawnOnLoad)
        {
            // Spawn enemies on load
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
        EventDispatcher.Raise(new PlayerRegisteredEvent { player = playerObject });
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void OnKeyAction(string action)
    {
        // Handle key actions
        if (action == "QuitGame")
        {
            Debug.Log("Quitting the game.");
            Application.Quit();
        }
        else if (action.StartsWith("SpawnEnemy_") && devmode)
        {
            // Extract the enemy type from the action
            if (int.TryParse(action.Split('_')[1], out int enemyType))
            {
                Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                EnemyPool.Instance.SpawnEnemy(enemyType, randomPosition);
                Debug.Log($"Spawned enemy of type {enemyType} at {randomPosition}.");
            }
            else
            {
                Debug.LogWarning($"Invalid spawn enemy action: {action}");
            }
        }
    }

    public bool isDevmode()
    {
        return devmode;
    }

    public KeyCode[] GetSpawnEnemyKeys()
    {
        return spawnEnemyKeys;
    }
}
