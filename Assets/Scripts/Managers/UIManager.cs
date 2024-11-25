using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IKeyActionReceiver
{
    private GameObject debugger; // Reference to the debugger panel
    private GameObject devmode; // Reference to the devmode panel
    private string debugMessage = "Debug Log"; // Default debug message

    [SerializeField] private KeyCode debugToggleKey = KeyCode.BackQuote; // Debug toggle key
    [SerializeField] private KeyCode commandSystemEnterKey = KeyCode.T; // Key to activate the command system
    [SerializeField] private KeyCode commandSystemExitKey = KeyCode.Escape; // Key to exit the command system

    private ScrollRect debugScroll; // Reference to the debug scroll rect
    private TextMeshProUGUI debugText; // Reference to the debug text
    [SerializeField] private GameObject SpawnEnemyUIPrefab; // Reference to the SpawnEnemyUI prefab

    private CommandSystem commandSystem; // Reference to the CommandSystem

    void OnEnable()
    {
        EventDispatcher.AddListener<DebugMessageEvent>(OnDebugMessage);
    }

    void OnDisable()
    {
        EventDispatcher.RemoveListener<DebugMessageEvent>(OnDebugMessage);
    }

    private void Awake()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager not found");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found");
            return;
        }

        // Register keys with InputManager
        InputManager.Instance.AddKeyBind(this, debugToggleKey, "ToggleDebugger");
        InputManager.Instance.AddKeyBind(this, commandSystemEnterKey, "CommandSystemEnter");
        InputManager.Instance.AddKeyBind(this, commandSystemExitKey, "CommandSystemExit");

        // Find the debugger panel in children
        debugger = transform.Find("Debug").gameObject;
        debugger.SetActive(false);

        // Find the scroll rect in children
        debugScroll = debugger.GetComponentInChildren<ScrollRect>();

        // Find the debug text in children
        debugText = debugScroll.transform.Find("Viewport/Content/Text").GetComponent<TextMeshProUGUI>();

        // Find the command system in children
        commandSystem = GetComponentInChildren<CommandSystem>();

        commandSystem.gameObject.SetActive(false);

        // Find the devmode panel in children
        devmode = transform.Find("DevMode").gameObject;
    }

    private void Start()
    {
        if (GameManager.Instance.isDevmode() && devmode != null && SpawnEnemyUIPrefab != null)
        {
            // Loop through GameManager's spawnEnemyKeys and instantiate a SpawnEnemyUI for each key
            KeyCode[] spawnEnemyKeys = GameManager.Instance.GetSpawnEnemyKeys();
            for (int i = 0; i < spawnEnemyKeys.Length; i++)
            {
                Enemy enemy = EnemyPool.Instance.GetPrefab(i);
                if (enemy == null)
                {
                    Debug.LogError($"Enemy prefab not found for key: {spawnEnemyKeys[i]}");
                    continue;
                }
                GameObject spawnEnemyUI = Instantiate(SpawnEnemyUIPrefab, devmode.transform);
                spawnEnemyUI.GetComponent<SpawnEnemyUI>().setEnemy(enemy, spawnEnemyKeys[i]);

                // Move up by 400 units and right by 200 units for each new SpawnEnemyUI
                spawnEnemyUI.transform.localPosition = new Vector3(200 * i, 400, 0);
            }
        }
    }

    public void OnKeyAction(string action)
    {
        // Handle key actions
        if (action == "ToggleDebugger")
        {
            ToggleDebugger();
        }
        else if (action == "CommandSystemEnter")
        {
            ToggleCommandSystem(true);
        }
        else if (action == "CommandSystemExit")
        {
            ToggleCommandSystem(false);
        }
        else
        {
            Debug.LogWarning($"Unhandled action: {action}");
        }
    }

    private void ToggleDebugger()
    {
        // Toggle the debugger panel
        debugger.SetActive(!debugger.activeSelf);

        if (debugger.activeSelf)
        {
            UpdateDebugMessage();
        }
    }

    private void UpdateDebugMessage()
    {
        // Update the debug message
        if (debugText != null && debugScroll != null)
        {
            debugText.text = debugMessage;

            // Force a layout rebuild to ensure the text displays correctly
            LayoutRebuilder.ForceRebuildLayoutImmediate(debugText.rectTransform);

            // Reset the scroll position to the top
            debugScroll.verticalNormalizedPosition = 0f;
        }
        else
        {
            Debug.LogError("Debug Text or Scroll Rect not found");
        }
    }

    private void ToggleCommandSystem(bool toggle) {
        commandSystem.gameObject.SetActive(toggle);
    }

    public void SetDebugMessage(string message)
    {
        debugMessage = message;
    }

    public void AppendDebugMessage(string message)
    {
        debugMessage += "\n" + message;
    }

    private void OnDebugMessage(DebugMessageEvent e)
    {
        AppendDebugMessage(e.message);
    }
}
