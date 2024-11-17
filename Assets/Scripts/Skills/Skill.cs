using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Skill : MonoBehaviour
{
    public string skillName; // The name of the skill

    private Image fill; // Black filter over the icon
    private TextMeshProUGUI cooldownText; // Text displaying the cooldown

    [SerializeField] private float cooldown = 5.0f; // The cooldown of the skill
    private float cooldownTimer = 0.0f; // The timer for the skill cooldown
    private bool isOnCooldown = false; // Indicates if the skill is on cooldown

    [SerializeField] private bool debug = false; // Debug toggle for logs

    protected virtual void OnEnable()
    {
        // Add the ActivateSkill method to the EventDispatcher
        EventDispatcher.AddListener<SkillActivatedEvent>(ActivateSkill);
    }

    protected virtual void OnDisable()
    {
        // Remove the ActivateSkill method from the EventDispatcher
        EventDispatcher.RemoveListener<SkillActivatedEvent>(ActivateSkill);
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

    public void ActivateSkill(SkillActivatedEvent e)
    {
        if (e.skillName != skillName)
        {
            // The skill name does not match, return
            return;
        }

        // Check if the skill is not on cooldown
        if (!isOnCooldown)
        {
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
