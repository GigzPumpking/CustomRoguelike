using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    [SerializeField] private LayerMask groundLayer; // Specify what layers count as ground

    // Debugging
    [SerializeField] private bool debug = false;

    private GameObject player;

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
    }

    public bool isObjectGrounded(Transform gameObject, float distance)
    {
        // Check if the object is grounded
        return Physics.Raycast(gameObject.position, Vector3.down, distance, groundLayer);
    }

    public void RegisterPlayer(GameObject playerObject)
    {
        player = playerObject;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}
