using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CustomAssetLoader : MonoBehaviour
{
    [SerializeField] private bool debug = false; // Debug toggle for logs

    private string userAssetPath;
    private string fallbackAssetPath;

    // List of supported image file extensions
    private readonly string[] supportedImageExtensions = { ".png", ".jpg", ".jpeg" };

    void Awake()
    {
        // Define the runtime user assets folder path (outside Unity project)
        userAssetPath = Path.Combine(Application.dataPath, "../UserAssets");

        // Define the fallback path inside Unity's Assets/Resources/Sprites folder
        fallbackAssetPath = Path.Combine(Application.dataPath, "Resources/Sprites");

        DebugMessage($"User assets folder path: {userAssetPath}");
        DebugMessage($"Fallback assets folder path: {fallbackAssetPath}");

        // Ensure the runtime directory exists
        if (!Directory.Exists(userAssetPath)) Directory.CreateDirectory(userAssetPath);
    }

    void OnEnable()
    {
        // Subscribe to the LoadCustomSpriteEvent
        EventDispatcher.AddListener<LoadCustomSpriteEvent>(OnLoadCustomSprite);
    }

    void OnDisable()
    {
        // Unsubscribe from the LoadCustomSpriteEvent
        EventDispatcher.RemoveListener<LoadCustomSpriteEvent>(OnLoadCustomSprite);
    }

    void Start()
    {
        // Raise the FilePathsLoadedEvent
        EventDispatcher.Raise<FilePathsLoadedEvent>(new FilePathsLoadedEvent());
    }

    void OnLoadCustomSprite(LoadCustomSpriteEvent e)
    {
        DebugMessage($"Loading custom sprite: {e.fileName}");
        LoadCustomSprite(e.fileName, e.target);
    }

    public void LoadCustomSprite(string fileName, GameObject target)
    {
        // Attempt to find the file in the UserAssets folder
        string userFilePath = FindFileWithExtension(userAssetPath, fileName);

        if (!string.IsNullOrEmpty(userFilePath))
        {
            DebugMessage($"File found in UserAssets folder: {userFilePath}");
            ApplySpriteFromFile(userFilePath, target);
        }
        else
        {
            DebugMessage($"File not found in UserAssets folder for: {fileName}");

            // Fallback: Search for matching files in Resources/Sprites
            foreach (string extension in supportedImageExtensions)
            {
                string spritePath = Path.Combine(fallbackAssetPath, fileName + extension);

                if (File.Exists(spritePath))
                {
                    DebugMessage($"Fallback sprite found in Resources: {spritePath}");
                    ApplySpriteFromFile(spritePath, target);
                    return;
                }
            }

            ApplyPlaceholder(target, fileName);
        }
    }

    private string FindFileWithExtension(string folderPath, string fileName)
    {
        foreach (string extension in supportedImageExtensions)
        {
            string filePath = Path.Combine(folderPath, fileName + extension);
            if (File.Exists(filePath))
            {
                return filePath;
            }
        }
        return null;
    }

    void ApplySpriteFromFile(string filePath, GameObject target)
    {
        // Load the file data into a byte array
        byte[] fileData = File.ReadAllBytes(filePath);

        // Create a Texture2D and load the image data
        Texture2D customTexture = new Texture2D(2, 2); // Temporary size; actual size is read when loading
        if (customTexture.LoadImage(fileData)) // Decode the image into the texture
        {
            // Create a sprite from the loaded texture
            Sprite customSprite = Sprite.Create(
                customTexture,
                new Rect(0, 0, customTexture.width, customTexture.height),
                new Vector2(0.5f, 0.5f) // Pivot at the center
            );

            // Apply the sprite to the appropriate component
            ApplySprite(customSprite, target);
        }
        else
        {
            DebugMessage($"Failed to load texture from file: {filePath}");
            ApplyPlaceholder(target, Path.GetFileName(filePath));
        }
    }

    void ApplySprite(Sprite sprite, GameObject target)
    {
        // Check if the GameObject has a SpriteRenderer component
        SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            DebugMessage("Applying sprite to SpriteRenderer.", false);
            spriteRenderer.sprite = sprite;

            // Adjust scale to ensure consistent size
            Vector2 desiredSize = new Vector2(1, 1); // Adjust to your consistent size
            AdjustSpriteRendererSize(spriteRenderer, desiredSize);

            return;
        }

        // Check if the GameObject has a UI Image component
        Image targetImage = target.GetComponent<Image>();
        if (targetImage != null)
        {
            DebugMessage("Applying sprite to Image component.", false);
            targetImage.sprite = sprite;

            // UI Images handle sizing automatically, no adjustment needed
            return;
        }

        DebugMessage("No SpriteRenderer or Image component found.", false);
    }

    void AdjustSpriteRendererSize(SpriteRenderer spriteRenderer, Vector2 desiredSize)
    {
        if (spriteRenderer.sprite == null) return;

        // Get the current sprite's size in world units
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // Calculate the scale needed to match the desired size
        Vector3 newScale = spriteRenderer.transform.localScale;
        newScale.x = desiredSize.x / spriteSize.x;
        newScale.y = desiredSize.y / spriteSize.y;

        // Apply the new scale to the SpriteRenderer's GameObject
        spriteRenderer.transform.localScale = newScale;

        DebugMessage($"SpriteRenderer resized to match desired size: {desiredSize}.", false);
    }

    void ApplyPlaceholder(GameObject target, string fileName = "unknown file") {
        // If no fallback sprite for fileName is found, find the Placeholder file in Resources and apply that instead IF it exists
        string placeholderPath = Path.Combine(fallbackAssetPath, "Placeholder.png");

        if (File.Exists(placeholderPath))
        {
            DebugMessage($"Fallback sprite not found for: {fileName}. Applying Placeholder sprite instead.");
            ApplySpriteFromFile(placeholderPath, target);
            return;
        } else {
            DebugMessage($"Fallback sprite not found in Resources for: {fileName}. No Placeholder sprite found.");
            DebugMessage($"Placeholder sprite path: {placeholderPath}");
        }
    }

    void DebugMessage(string message, bool raiseEvent = true)
    {
        if (debug) Debug.Log(message);

        EventDispatcher.Raise<DebugMessageEvent>(new DebugMessageEvent()
        {
            message = message
        });
    }
}
