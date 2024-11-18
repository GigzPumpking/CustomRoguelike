using UnityEngine;

public class CustomSprite : MonoBehaviour
{
    [SerializeField] private string fileName = "Placeholder";
    void OnEnable() {
        // Subscribe to file paths loaded event
        EventDispatcher.AddListener<FilePathsLoadedEvent>(OnFilePathsLoaded);
    }

    void OnDisable() {
        // Unsubscribe from file paths loaded event
        EventDispatcher.RemoveListener<FilePathsLoadedEvent>(OnFilePathsLoaded);
    }

    void OnFilePathsLoaded(FilePathsLoadedEvent e) {
        // Load the custom sprite
        EventDispatcher.Raise<LoadCustomSpriteEvent>(new LoadCustomSpriteEvent() {
            fileName = fileName,
            target = gameObject
        });
    }

    public void LoadNewSprite(string newFileName) {
        fileName = newFileName;
        EventDispatcher.Raise<LoadCustomSpriteEvent>(new LoadCustomSpriteEvent() {
            fileName = fileName,
            target = gameObject
        });
    }
}
