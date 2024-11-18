using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<KeyCode> registeredKeys = new List<KeyCode>();

    void OnEnable()
    {
        // Subscribe to the AddKeybindEvent
        EventDispatcher.AddListener<AddKeybindEvent>(OnAddKeybind);
    }

    void OnDisable()
    {
        // Unsubscribe from the AddKeybindEvent
        EventDispatcher.RemoveListener<AddKeybindEvent>(OnAddKeybind);
    }

    void OnAddKeybind(AddKeybindEvent e)
    {
        // Add the key binding to the list of registered keys if it doesn't already exist
        if (!registeredKeys.Contains(e.keyCode)) {
            registeredKeys.Add(e.keyCode);
        }
    }

    public void RegisterKey(KeyCode keyCode)
    {
        // Add the key binding to the list of registered keys if it doesn't already exist
        if (!registeredKeys.Contains(keyCode)) {
            registeredKeys.Add(keyCode);
        }
    }

    void Update()
    {
        // Check for any key press
        if (Input.anyKeyDown)
        {
            // Check if the key pressed is a registered key binding
            foreach (KeyCode keyCode in registeredKeys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    // Raise the KeyPress event
                    EventDispatcher.Raise<KeyPressEvent>(new KeyPressEvent()
                    {
                        keyCode = keyCode
                    });
                }
            }
        }
    }
}
