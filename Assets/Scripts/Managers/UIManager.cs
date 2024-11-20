using UnityEngine;
using TMPro;

// Skill Icons
// - hammer slam by Maxicons from <a href="https://thenounproject.com/browse/icons/term/skill-hammer-slam/" target="_blank" title="skill hammer slam Icons">Noun Project</a> (CC BY 3.0)

public class UIManager : MonoBehaviour
{
    private GameObject debugger; // Reference to the debugger panel

    private GameObject devmode; // Reference to the devmode panel

    private TextMeshProUGUI debugText; // Reference to the debug text

    private string debugMessage = "Debug Log"; // Default debug message

    [SerializeField] private KeyCode debugToggleKey = KeyCode.BackQuote; // Default debug toggle key

    [SerializeField] private GameObject SpawnEnemyUIPrefab; // Reference to the SpawnEnemyUI prefab
    

    void OnEnable() {
        // Subscribe to the KeyPressEvent
        EventDispatcher.AddListener<KeyPressEvent>(OnKeyPress);

        // Subscribe to the DebugMessageEvent
        EventDispatcher.AddListener<DebugMessageEvent>(OnDebugMessage);
    }

    void OnDisable() {
        // Unsubscribe from the KeyPressEvent
        EventDispatcher.RemoveListener<KeyPressEvent>(OnKeyPress);

        // Unsubscribe from the DebugMessageEvent
        EventDispatcher.RemoveListener<DebugMessageEvent>(OnDebugMessage);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the debugger panel in children
        debugger = transform.Find("Debug").gameObject;

        // Find the debug text in children of the debugger panel
        debugText = debugger.transform.Find("DebugText").GetComponent<TextMeshProUGUI>();

        debugger.SetActive(false);

        // Find the devmode panel in children
        devmode = transform.Find("DevMode").gameObject;

        GameManager.Instance.RegisterKey(debugToggleKey);

        // Check if the devmode panel exists
        if (GameManager.Instance.isDevmode() && devmode != null && SpawnEnemyUIPrefab != null) {
            // Loop through GameManager's spawnEnemyKeys and instantiate a SpawnEnemyUI for each key
            KeyCode[] spawnEnemyKeys = GameManager.Instance.GetSpawnEnemyKeys();
            for (int i = 0; i < spawnEnemyKeys.Length; i++) {
                Enemy enemy = EnemyPool.Instance.GetPrefab(i);
                if (enemy == null) {
                    Debug.LogError("Enemy prefab not found for key: " + spawnEnemyKeys[i]);
                    continue;
                }
                GameObject spawnEnemyUI = Instantiate(SpawnEnemyUIPrefab, devmode.transform);
                spawnEnemyUI.GetComponent<SpawnEnemyUI>().setEnemy(enemy, spawnEnemyKeys[i]);
                // Move up by 400 units and right by 200 units for each new SpawnEnemyUI
                spawnEnemyUI.transform.localPosition = new Vector3(200 * i, 400, 0);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnKeyPress(KeyPressEvent e) {
        // Check if the debug toggle key is pressed
        if (e.keyCode == debugToggleKey) {
            // Toggle the debugger panel
            debugger.SetActive(!debugger.activeSelf);

            // Update the debug message
            debugText.text = debugMessage;
        }
    }

    public void SetDebugMessage(string message) {
        debugMessage = message;
    }

    public void AppendDebugMessage(string message) {
        debugMessage += "\n" + message;
    }

    void OnDebugMessage(DebugMessageEvent e) {
        AppendDebugMessage(e.message);
    }
}
