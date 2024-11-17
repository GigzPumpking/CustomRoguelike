using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CustomAssetLoader : MonoBehaviour
{
    [SerializeField] private string fileName = "Slam.png"; // Name of the file to load
    [SerializeField] private bool debug = false; // Debug toggle for logs

    private string userAssetPath;

    void Start()
    {
        // Define the user assets folder path
        userAssetPath = Path.Combine(Application.dataPath, "../UserAssets");

        if (debug) Debug.Log($"User assets folder path: {userAssetPath}");

        // Ensure the directory exists
        Directory.CreateDirectory(userAssetPath);

        // Load and apply the custom sprite
        LoadCustomSprite(fileName);
    }

    void LoadCustomSprite(string fileName)
    {
        string filePath = Path.Combine(userAssetPath, fileName);

        if (File.Exists(filePath))
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
                ApplySprite(customSprite);
            }
        }
        else
        {
            Debug.LogWarning($"File not found: {filePath}");
        }
    }

    void ApplySprite(Sprite sprite)
    {
        // Check if the GameObject has a SpriteRenderer component
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (debug) Debug.Log("Applying sprite to SpriteRenderer.");
            spriteRenderer.sprite = sprite;
            return;
        }

        // Check if the GameObject has a UI Image component
        Image targetImage = GetComponent<Image>();
        if (targetImage != null)
        {
            if (debug) Debug.Log("Applying sprite to UI Image.");
            targetImage.sprite = sprite;
            return;
        }

        // Log an error if neither component is found
        Debug.LogError("No SpriteRenderer or UI Image component found on this GameObject.");
    }
}
