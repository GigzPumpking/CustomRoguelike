using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour, IKeyActionReceiver
{
    public static UIManager Instance { get; private set; } // Singleton instance

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

    private GameObject playerUI; // Reference to the player UI
    private HealthBar playerHealthBar; // Reference to the player's health bar
    private GameObject skillsUI; // Reference to the skills UI

    // List of all skills
    private List<Skill> skills = new List<Skill>();

    // Keycode-to-skill-index mapping
    private Dictionary<KeyCode, List<int>> keycodeToSkillIndexes = new Dictionary<KeyCode, List<int>>();

    // Default keycodes and maximum limits
    [SerializeField] private KeyCode[] defaultKeycodes = { KeyCode.Space, KeyCode.LeftShift, KeyCode.Q };
    [SerializeField] private Skill[] initialSkills; // Initial skills to assign
    [SerializeField] private int maxSkills = 5; // Maximum number of skills

    [SerializeField] private bool debug = false; // Debugging

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes

        if (InputManager.Instance == null)
        {
            if (debug) Debug.LogError("InputManager not found");
            return;
        }

        if (GameManager.Instance == null)
        {
            if (debug) Debug.LogError("GameManager not found");
            return;
        }

        // Initialize skillbinds with default skills
        InitializeDefaultSkills();

        // Register keys with InputManager
        InputManager.Instance.AddKeyBind(this, debugToggleKey, "ToggleDebugger");
        InputManager.Instance.AddKeyBind(this, commandSystemEnterKey, "CommandSystemEnter");
        InputManager.Instance.AddKeyBind(this, commandSystemExitKey, "CommandSystemExit");

        // Find the player UI in children
        playerUI = transform.Find("PlayerUI").gameObject;
        playerHealthBar = playerUI.GetComponentInChildren<HealthBar>();
        skillsUI = playerUI.transform.Find("Skills").gameObject;

        // Find the debugger panel in children
        debugger = transform.Find("Debug").gameObject;
        debugger.SetActive(false);
        debugScroll = debugger.GetComponentInChildren<ScrollRect>();
        debugText = debugScroll.transform.Find("Viewport/Content/Text").GetComponent<TextMeshProUGUI>();

        // Find the command system in children
        commandSystem = GetComponentInChildren<CommandSystem>();
        commandSystem.gameObject.SetActive(false);

        // Find the devmode panel in children
        devmode = transform.Find("DevMode").gameObject;
    }

    private void OnEnable()
    {
        EventDispatcher.AddListener<DebugMessageEvent>(OnDebugMessage);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveListener<DebugMessageEvent>(OnDebugMessage);
    }

    private void Start()
    {
        InitializeDevmode();
    }

    private void InitializeDevmode()
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
                    if (debug) Debug.LogError($"Enemy prefab not found for key: {spawnEnemyKeys[i]}");
                    continue;
                }
                GameObject spawnEnemyUI = Instantiate(SpawnEnemyUIPrefab, devmode.transform);
                spawnEnemyUI.GetComponent<SpawnEnemyUI>().setEnemy(enemy, spawnEnemyKeys[i]);

                // Move up by 400 units and right by 200 units for each new SpawnEnemyUI
                spawnEnemyUI.transform.localPosition = new Vector3(200 * i, 400, 0);
            }
        }
    }

    private void InitializeDefaultSkills()
    {
        if (initialSkills == null || defaultKeycodes.Length == 0)
        {
            if (debug) Debug.LogError("Initial skills or default keycodes are not set properly.");
            return;
        }

        for (int i = 0; i < initialSkills.Length && i < maxSkills; i++) // Respect maxSkills
        {
            AddSkill(initialSkills[i], defaultKeycodes[i % defaultKeycodes.Length]);
        }
    }

    public bool AddSkill(Skill skill, KeyCode key)
    {
        // Check skill limit
        if (skills.Count >= maxSkills)
        {
            if (debug) Debug.LogWarning($"Cannot add skill {skill.name}: Maximum skill limit of {maxSkills} reached.");
            return false;
        }

        if (skill == null || key == KeyCode.None)
        {
            if (debug) Debug.LogWarning("Invalid skill or keycode.");
            return false;
        }

        // Add skill to the list and retrieve its index
        int skillIndex = skills.Count;
        skills.Add(skill);

        // Map the keycode to the skill's index
        if (!keycodeToSkillIndexes.ContainsKey(key))
        {
            keycodeToSkillIndexes[key] = new List<int>();
        }
        keycodeToSkillIndexes[key].Add(skillIndex);

        // Register the action in InputManager
        InputManager.Instance.AddKeyBind(this, key, $"Skill_{skillIndex + 1}");
        skill.UpdateKeyCode(key);

        if (debug) Debug.Log($"Skill {skill.name} added as Skill_{skillIndex + 1} and bound to key {key}");

        return true;
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
        else if (action.StartsWith("Skill_"))
        {
            // Parse the skill index from the action name
            string[] actionParts = action.Split('_');
            if (actionParts.Length != 2 || !int.TryParse(actionParts[1], out int skillIndex))
            {
                if (debug) Debug.LogWarning($"Invalid skill action: {action}");
                return;
            }

            skillIndex--; // Convert 1-based index to 0-based

            // Activate the skill by its index
            if (skillIndex >= 0 && skillIndex < skills.Count)
            {
                Skill skill = skills[skillIndex];
                if (skill != null)
                {
                    skill.ActivateSkill();
                    if (debug) Debug.Log($"Activated {skill.name} from Skill_{skillIndex + 1}");
                }
            }
            else
            {
                if (debug) Debug.LogWarning($"Skill index out of range: {skillIndex + 1}");
            }
        }
        else
        {
            if (debug) Debug.LogWarning($"Unhandled action: {action}");
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
            if (debug) Debug.LogError("Debug Text or Scroll Rect not found");
        }
    }

    private void ToggleCommandSystem(bool toggle)
    {
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

    public void UpdatePlayerHealth(float health)
    {
        // Update the player's health bar
        playerHealthBar.SetHealth(health);
    }

    public void UpdatePlayerMaxHealth(float maxHealth)
    {
        // Update the player's max health
        playerHealthBar.SetMaxHealth(maxHealth);
    }
}
