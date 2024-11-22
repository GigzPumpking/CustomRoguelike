using UnityEngine;
using UnityEngine.UI;

public class CustomSprite : MonoBehaviour
{
    [SerializeField] private string fileName = "Placeholder";

    private SpriteRenderer spriteRenderer;

    private Image image;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
    }


    void OnEnable() {
        // Subscribe to file paths loaded event
        EventDispatcher.AddListener<FilePathsLoadedEvent>(OnFilePathsLoaded);

        if ((spriteRenderer != null && spriteRenderer.sprite == null) || (image != null && image.sprite == null)) {
            // Try to load the custom sprite
            LoadNewSprite(fileName);
        }
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

    public void SetFilename(string newFileName) {
        fileName = newFileName;
        LoadNewSprite(fileName);
    }
}
