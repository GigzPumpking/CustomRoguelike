using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CustomAssetLoader : MonoBehaviour
{
    [SerializeField] private bool debug = false; // Debug toggle for logs

    private string userAssetPath;
    private string fallbackAssetPath;

    void Awake()
    {
        // Define the runtime user assets folder path (outside Unity project)
        userAssetPath = Path.Combine(Application.dataPath, "../UserAssets");

        // Define the fallback path inside Unity's Assets/Sprites folder
        fallbackAssetPath = Path.Combine(Application.dataPath, "Sprites");

        DebugMessage($"User assets folder path: {userAssetPath}");
        DebugMessage($"Fallback assets folder path: {fallbackAssetPath}");

        // Ensure the runtime directory exists
        if (!Directory.Exists(userAssetPath)) Directory.CreateDirectory(userAssetPath);
    }

    void OnEnable()
    {
        // Subscribe to the LoadCustomSpriteEvent
        EventDispatcher.AddListener<LoadCustomSpriteEvent>(OnLoadCustomSprite);

        // Raise the FilePathsLoadedEvent
        EventDispatcher.Raise<FilePathsLoadedEvent>(new FilePathsLoadedEvent());
    }

    void OnDisable()
    {
        // Unsubscribe from the LoadCustomSpriteEvent
        EventDispatcher.RemoveListener<LoadCustomSpriteEvent>(OnLoadCustomSprite);
    }

    void OnLoadCustomSprite(LoadCustomSpriteEvent e)
    {
        DebugMessage($"Loading custom sprite: {e.fileName}");
        LoadCustomSprite(e.fileName, e.target);
    }

    public void LoadCustomSprite(string fileName, GameObject target)
    {
        if (userAssetPath != null) {
            DebugMessage("User assets folder exists for file: " + fileName);
        } else {
            DebugMessage("User assets folder does not exist for file: " + fileName);
        }

        string userFilePath = Path.Combine(userAssetPath, fileName);

        // Attempt to load the image from the UserAssets folder
        if (File.Exists(userFilePath))
        {
            DebugMessage($"File found in UserAssets folder: {userFilePath}");

            ApplySpriteFromFile(userFilePath, target);
        }
        else
        {
            DebugMessage($"File not found in UserAssets folder: {userFilePath}");

            // Fallback: Load from Resources/Sprites folder
            string spritePath = Path.Combine("Sprites", Path.GetFileNameWithoutExtension(fileName));
            Sprite fallbackSprite = Resources.Load<Sprite>(spritePath);

            if (fallbackSprite != null)
            {
                DebugMessage($"Fallback sprite found in Resources: {spritePath}");

                ApplySprite(fallbackSprite, target);
            }
            else
            {
                DebugMessage($"Fallback sprite not found in Resources: {spritePath}");
            }
        }
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

    // Adjusts the scale of the SpriteRenderer to fit a consistent size
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


    void DebugMessage(string message, bool raiseEvent = true)
    {
        if (debug) Debug.Log(message);

        EventDispatcher.Raise<DebugMessageEvent>(new DebugMessageEvent()
        {
            message = message
        });
    }
}
