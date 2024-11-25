using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

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
        public KeyCode keyCode;  // The key associated with the action
        public string action;    // Action identifier (e.g., "Jump", "Sprint")

        public KeyBindAction(KeyCode keyCode, string action)
        {
            this.keyCode = keyCode;
            this.action = action;
        }
    }

    [SerializeField]
    private List<KeyBindPair> keyBindPairs = new List<KeyBindPair>();

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

    public void AddKeyBind(MonoBehaviour script, KeyCode keyCode, string action)
    {
        // Find the existing KeyBindPair for the script
        KeyBindPair existingPair = keyBindPairs.Find(pair => pair.script == script);

        if (existingPair != null)
        {
            // Check if the key-action pair already exists
            if (!existingPair.keyBindActions.Exists(kba => kba.keyCode == keyCode && kba.action == action))
            {
                existingPair.keyBindActions.Add(new KeyBindAction(keyCode, action));
            }
        }
        else
        {
            // Create a new KeyBindPair and add it to the list
            KeyBindPair newPair = new KeyBindPair(script);
            newPair.keyBindActions.Add(new KeyBindAction(keyCode, action));
            keyBindPairs.Add(newPair);
        }
    }

    public void RemoveKeyBind(MonoBehaviour script, KeyCode keyCode, string action)
    {
        // Find the existing KeyBindPair for the script
        KeyBindPair existingPair = keyBindPairs.Find(pair => pair.script == script);

        if (existingPair != null)
        {
            // Remove the specific key-action pair
            existingPair.keyBindActions.RemoveAll(kba => kba.keyCode == keyCode && kba.action == action);

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
                    if (pair.script is IKeyActionReceiver receiver)
                    {
                        receiver.OnKeyAction(keyBindAction.action);
                    }
                }
            }
        }
    }
}
