using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Skill : MonoBehaviour
{
    public string skillName; // The name of the skill
    private KeyCode keyCode; // The key code to activate the skill
    [SerializeField] private int skillIndex; // The index of the skill in the skill list

    private Image fill; // Black filter over the icon
    private TextMeshProUGUI cooldownText; // Text displaying the cooldown
    private CustomAssetLoader customAssetLoader; // Reference to the CustomAssetLoader script
    [SerializeField] private float cooldown = 5.0f; // The cooldown of the skill
    private float cooldownTimer = 0.0f; // The timer for the skill cooldown
    private bool isOnCooldown = false; // Indicates if the skill is on cooldown
    protected GameObject player; // Reference to the player object
    protected Player playerScript; // Reference to the player script

    [SerializeField] protected bool debug = false; // Debug toggle for logs

    protected virtual void OnEnable()
    {
        // Add the ActivateSkill method to the EventDispatcher
        EventDispatcher.AddListener<SkillActivatedEvent>(ActivateSkill);

        // Add the KeyPress event to the EventDispatcher
        EventDispatcher.AddListener<KeyPressEvent>(OnKeyPress);

        // Add PlayerRegisteredEvent to the EventDispatcher
        EventDispatcher.AddListener<PlayerRegisteredEvent>(OnPlayerRegistered);
    }

    protected virtual void OnDisable()
    {
        // Remove the ActivateSkill method from the EventDispatcher
        EventDispatcher.RemoveListener<SkillActivatedEvent>(ActivateSkill);

        // Remove the KeyPress event from the EventDispatcher
        EventDispatcher.RemoveListener<KeyPressEvent>(OnKeyPress);

        // Remove the PlayerRegisteredEvent from the EventDispatcher
        EventDispatcher.RemoveListener<PlayerRegisteredEvent>(OnPlayerRegistered);
    }

    private void OnPlayerRegistered(PlayerRegisteredEvent e)
    {
        player = e.player;
        playerScript = player.GetComponent<Player>();

        // Set the key code to the skill binding at the skill index if the skillbindings index is valid
        if (playerScript.GetSkillBindings().Length > skillIndex) {
            keyCode = playerScript.GetSkillBindings()[skillIndex];
        } else if (debug) {
            Debug.LogError("Skill index is out of range.");
        }
    }

    protected virtual void Start()
    {
        // Get the Image component from the child named "Fill"
        fill = transform.Find("Fill").GetComponent<Image>();

        if (fill != null)
        {
            fill.enabled = false;
        }

        // Get the TextMeshProUGUI component from the child named "CooldownText"
        cooldownText = transform.Find("Cooldown").GetComponent<TextMeshProUGUI>();

        // Set the cooldown text to an empty string
        cooldownText.text = "";

        // Get the CustomAssetLoader script from the child named "Image"
        customAssetLoader = transform.Find("Image").GetComponent<CustomAssetLoader>();

        // Set the skill name of the CustomAssetLoader script to the skill name
        customAssetLoader.SetFileName(skillName + ".png");
    }

    protected virtual void Update()
    {
        if (isOnCooldown)
        {
            // Update the cooldown timer
            cooldownTimer -= Time.deltaTime;

            // Update the cooldown text
            cooldownText.text = $"{cooldownTimer:0.0}";

            // Check if the cooldown timer has reached zero
            if (cooldownTimer <= 0.0f)
            {
                // Reset the cooldown timer
                cooldownTimer = 0.0f;

                // Indicate that the skill is no longer on cooldown
                isOnCooldown = false;

                // Hide the black filter
                if (fill != null)
                {
                    fill.enabled = false;
                }

                // Clear the cooldown text
                cooldownText.text = "";
            }
        }
    }

    protected virtual void OnKeyPress(KeyPressEvent e)
    {
        if (e.keyCode != keyCode)
        {
            // The key does not match the skill name,
            if (debug)
            {
                Debug.Log("Key does not match the skill name.");
            }
            return;
        }

        ActivateSkill();
    }

    public void ActivateSkill(SkillActivatedEvent e)
    {
        if (e.skillName != skillName)
        {
            // The skill name does not match, return
            return;
        }

        ActivateSkill();
    }

    public void ActivateSkill() {
        // Check if the skill is not on cooldown
        if (!isOnCooldown)
        {
            if (debug)
            {
                Debug.Log($"{skillName} activated.");
            }

            // Call the abstract ApplySkillEffect method for derived classes to define behavior
            ApplySkillEffect();

            // Start the cooldown timer
            cooldownTimer = cooldown;

            // Indicate that the skill is on cooldown
            isOnCooldown = true;

            // Show the black filter
            if (fill != null)
            {
                fill.enabled = true;
            }
        }
    }

    // Abstract method to be implemented by derived classes
    protected abstract void ApplySkillEffect();
}
