using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommandSystem : MonoBehaviour
{
    private TMP_InputField inputField;
    private TextMeshProUGUI textHistory;
    private ScrollRect scrollRect;
    private bool isInputFieldFocused;

    [SerializeField] private float layoutUpdateDelay = 0.3f;

    // Dictionary to store commands
    private Dictionary<string, ICommand> commands;

    private void Awake()
    {
        // Find required UI components
        inputField = GetComponentInChildren<TMP_InputField>();
        textHistory = GetComponentInChildren<TextMeshProUGUI>();
        scrollRect = GetComponentInChildren<ScrollRect>();

        // Initialize the command dictionary
        commands = new Dictionary<string, ICommand>();

        // Register commands
        RegisterCommand(new ClearHistoryCommand(ClearHistory));
        RegisterCommand(new HelpCommand(commands)); // Help command dynamically lists all commands
        RegisterCommand(new SpawnEnemyCommand());
    }

    private void OnEnable()
    {
        // Attach input field listener
        if (inputField != null)
        {
            inputField.onSubmit.AddListener(OnCommandEntered);
            inputField.onSelect.AddListener(OnInputFieldFocused);
            inputField.onDeselect.AddListener(OnInputFieldUnfocused);
        }

        inputField.ActivateInputField();
        ScrollToBottom();
    }

    private void OnDisable()
    {
        OnInputFieldUnfocused(null);

        // Detach listener to avoid memory leaks
        if (inputField != null)
        {
            inputField.onSubmit.RemoveListener(OnCommandEntered);
            inputField.onSelect.RemoveListener(OnInputFieldFocused);
            inputField.onDeselect.RemoveListener(OnInputFieldUnfocused);
        }
    }

    private void OnInputFieldFocused(string input)
    {
        isInputFieldFocused = true;
        InputManager.Instance.ToggleListening(!isInputFieldFocused);
    }

    private void OnInputFieldUnfocused(string input)
    {
        isInputFieldFocused = false;
        InputManager.Instance.ToggleListening(!isInputFieldFocused);
    }

    private void RegisterCommand(ICommand command)
    {
        if (!commands.ContainsKey(command.Name))
        {
            commands.Add(command.Name, command);
        }
    }

    public void Toggle(bool active)
    {
        if (active && gameObject.activeSelf)
        {
            inputField.ActivateInputField();
            return;
        }

        gameObject.SetActive(active);

        if (inputField == null) {
            inputField = GetComponentInChildren<TMP_InputField>();
        }

        if (active)
        {    
            inputField.ActivateInputField();
            inputField.text = "";
        }
    }

    private void OnCommandEntered(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        // Add command to history and process
        AddToHistory($"> {input}");
        ProcessCommand(input.ToLower());

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void ProcessCommand(string input)
    {
        // Parse input respecting quoted parameters
        string[] parts = ParseCommandWithQuotes(input);
        if (parts.Length == 0) return;

        string commandKey = parts[0];
        string[] parameters = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (commands.TryGetValue(commandKey, out ICommand command))
        {
            command.Execute(parameters, AddToHistory);
        }
        else
        {
            AddToHistory($"Error: Unknown command '{commandKey}'.");
        }
    }

    private void AddToHistory(string message)
    {
        // Add message to the history
        textHistory.text = string.IsNullOrEmpty(textHistory.text) ? message : $"{textHistory.text}\n{message}";

        // Use a coroutine to ensure the layout rebuild and scrolling happen in sequence
        StartCoroutine(ScrollToBottomAfterLayout());
    }

    private System.Collections.IEnumerator ScrollToBottomAfterLayout()
    {
        yield return new WaitForSeconds(layoutUpdateDelay);
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void ClearHistory()
    {
        // Clear the command history display
        textHistory.text = "";
        scrollRect.verticalNormalizedPosition = 1f; // Reset scroll to the top
    }

    private string[] ParseCommandWithQuotes(string input)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string currentPart = "";

        foreach (char c in input)
        {
            if (c == '"' && !inQuotes)
            {
                inQuotes = true;
            }
            else if (c == '"' && inQuotes)
            {
                inQuotes = false;
                result.Add(currentPart);
                currentPart = "";
            }
            else if (c == ' ' && !inQuotes)
            {
                if (!string.IsNullOrWhiteSpace(currentPart))
                {
                    result.Add(currentPart);
                    currentPart = "";
                }
            }
            else
            {
                currentPart += c;
            }
        }

        if (!string.IsNullOrWhiteSpace(currentPart))
        {
            result.Add(currentPart);
        }

        return result.ToArray();
    }
}
