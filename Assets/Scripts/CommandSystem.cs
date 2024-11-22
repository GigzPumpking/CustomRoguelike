using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandSystem : MonoBehaviour
{
    private TMP_InputField inputField; // Input field for user input
    private TextMeshProUGUI textHistory; // Text display for command history
    private ScrollRect scrollRect; // ScrollRect for scrolling through history
    
    private GameObject content; // Content object for history text

    [SerializeField] private KeyCode typeKey = KeyCode.T; // Key to activate the input field

    private void Awake()
    {
        // Ensure references are assigned
        inputField = GetComponentInChildren<TMP_InputField>();
        textHistory = GetComponentInChildren<TextMeshProUGUI>();
        scrollRect = GetComponentInChildren<ScrollRect>();
        content = textHistory.transform.parent.gameObject;
    }

    private void OnEnable()
    {
        // Add listener to input field
        if (inputField != null)
        {
            inputField.onSubmit.AddListener(OnCommandEntered);
        }

        EventDispatcher.AddListener<KeyPressEvent>(OnKeyPress);
    }

    private void OnDisable()
    {
        // Remove listener to avoid memory leaks
        if (inputField != null)
        {
            inputField.onSubmit.RemoveListener(OnCommandEntered);
        }

        EventDispatcher.RemoveListener<KeyPressEvent>(OnKeyPress);
    }

    void Start()
    {
        GameManager.Instance.RegisterKey(typeKey);
    }

    private void OnCommandEntered(string command)
    {
        if (string.IsNullOrWhiteSpace(command)) return;

        // Add the command to the history and process it
        AddToHistory($"> {command}");
        ProcessCommand(command);

        // Clear input field and reactivate it
        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void AddToHistory(string message)
    {
        // Add new message to the text display
        textHistory.text += $"\n{message}";

        // Update the layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(textHistory.rectTransform);

        // update the scrollbar's size and position
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void ProcessCommand(string command)
    {
        // Example of command processing logic
        if (command == "spawn enemy")
        {
            AddToHistory("System: Spawned an enemy.");
        }
        else
        {
            AddToHistory($"System: Unknown command '{command}'.");
        }
    }

    private void OnKeyPress(KeyPressEvent e)
    {
        if (e.keyCode == typeKey)
        {
            // If input field is active, activate it
            if (inputField.gameObject.activeSelf)
            {
                inputField.ActivateInputField();
            }
        }
    }
}
