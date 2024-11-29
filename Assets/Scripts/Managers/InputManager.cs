using System.Collections.Generic;
using System;
using UnityEngine;

public class InputManager : MonoBehaviour, IInputStateProvider
{
    public static InputManager Instance { get; private set; }

    private bool isListening = true;

    [System.Serializable]
    public class KeyBindPair
    {
        public MonoBehaviour script; // Associated script
        public List<KeyBindAction> keyBindActions; // Actions for this script

        public KeyBindPair(MonoBehaviour script)
        {
            this.script = script;
            this.keyBindActions = new List<KeyBindAction>();
        }
    }

    [System.Serializable]
    public class KeyBindAction
    {
        public enum ActionType
        {
            Core,      // Always active, bypasses isListening
            Gameplay,  // Requires isListening to be true
            UI         // Potential for specific categories in the future
        }

        public KeyCode keyCode;    // The key associated with the action
        public string action;      // Action identifier (e.g., "Jump", "Sprint")
        public ActionType actionType; // The type of action

        public KeyBindAction(KeyCode keyCode, string action, string actionTypeString = "Gameplay")
        {
            this.keyCode = keyCode;
            this.action = action;
            this.actionType = ParseActionType(actionTypeString);
        }

        public static ActionType ParseActionType(string actionTypeString)
        {
            // Default to Gameplay if the string is invalid or empty
            if (string.IsNullOrWhiteSpace(actionTypeString)) 
                return ActionType.Gameplay;

            // Attempt to parse the string to an ActionType
            if (Enum.TryParse(actionTypeString, true, out ActionType parsedType))
            {
                return parsedType;
            }

            // If parsing fails, log a warning and default to Gameplay
            Debug.LogWarning($"Invalid ActionType '{actionTypeString}'. Defaulting to Gameplay.");
            return ActionType.Gameplay;
        }
    }

    [SerializeField]
    private List<KeyBindPair> keyBindPairs = new List<KeyBindPair>();

    public bool IsInputEnabled => isListening;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public void AddKeyBind(MonoBehaviour script, KeyCode keyCode, string action, string actionType = "Gameplay")
    {
        // Find the existing KeyBindPair for the script
        KeyBindPair existingPair = keyBindPairs.Find(pair => pair.script == script);

        if (existingPair != null)
        {
            // Check if the key-action pair already exists
            if (!existingPair.keyBindActions.Exists(kba => kba.keyCode == keyCode && kba.action == action))
            {
                existingPair.keyBindActions.Add(new KeyBindAction(keyCode, action, actionType));
            }
        }
        else
        {
            // Create a new KeyBindPair and add it to the list
            KeyBindPair newPair = new KeyBindPair(script);
            newPair.keyBindActions.Add(new KeyBindAction(keyCode, action, actionType));
            keyBindPairs.Add(newPair);
        }
    }

    public void RemoveKeyBind(MonoBehaviour script, KeyCode keyCode, string action, string actionTypeString = "Gameplay")
    {
        // Find the existing KeyBindPair for the script
        KeyBindPair existingPair = keyBindPairs.Find(pair => pair.script == script);

        if (existingPair != null)
        {
            // Parse the string to an ActionType
            KeyBindAction.ActionType actionType = KeyBindAction.ParseActionType(actionTypeString);

            // Remove the specific key-action pair
            existingPair.keyBindActions.RemoveAll(
                kba => kba.keyCode == keyCode && 
                kba.action == action && 
                kba.actionType == actionType
            );

            // If no key-action pairs remain, remove the entire pair
            if (existingPair.keyBindActions.Count == 0)
            {
                keyBindPairs.Remove(existingPair);
            }
        }
    }

    void Update()
    {
        // Iterate through all keybinds and check for input
        foreach (KeyBindPair pair in keyBindPairs)
        {
            foreach (KeyBindAction keyBindAction in pair.keyBindActions)
            {
                if (Input.GetKeyDown(keyBindAction.keyCode))
                {
                    if (!ShouldProcessAction(keyBindAction))
                    {
                        continue;
                    }

                    if (pair.script is IKeyActionReceiver receiver)
                    {
                        receiver.OnKeyAction(keyBindAction.action);
                    }
                }
            }
        }
    }

    private bool ShouldProcessAction(KeyBindAction keyBindAction)
    {
        // Core actions bypass the isListening check
        if (keyBindAction.actionType == KeyBindAction.ActionType.Core)
        {
            return true;
        }

        // Otherwise, check isListening
        return isListening;
    }

    public void ToggleListening(bool listen)
    {
        isListening = listen;
    }
}
