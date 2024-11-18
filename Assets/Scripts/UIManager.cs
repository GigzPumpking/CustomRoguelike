using UnityEngine;
using TMPro;

// Skill Icons
// - hammer slam by Maxicons from <a href="https://thenounproject.com/browse/icons/term/skill-hammer-slam/" target="_blank" title="skill hammer slam Icons">Noun Project</a> (CC BY 3.0)

public class UIManager : MonoBehaviour
{
    private GameObject debugger; // Reference to the debugger panel

    private TextMeshProUGUI debugText; // Reference to the debug text

    private string debugMessage = "Debug Log"; // Default debug message

    [SerializeField] private KeyCode debugToggleKey = KeyCode.BackQuote; // Default debug toggle key
    

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

        GameManager.Instance.RegisterKey(debugToggleKey);
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
        Debug.Log("Debug message received: " + e.message);
        // Set the debug message
        AppendDebugMessage(e.message);
    }
}
