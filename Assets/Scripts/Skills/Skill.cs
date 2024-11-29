using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Skill : MonoBehaviour
{
    public string skillName; // The name of the skill
    private TextMeshProUGUI keyCodeDisplay; // The text displaying the key code

    private Image fill; // Black filter over the icon
    private TextMeshProUGUI cooldownText; // Text displaying the cooldown
    [SerializeField] private float cooldown = 5.0f; // The cooldown of the skill
    private float cooldownTimer = 0.0f; // The timer for the skill cooldown
    private bool isOnCooldown = false; // Indicates if the skill is on cooldown
    protected GameObject player; // Reference to the player object
    protected Player playerScript; // Reference to the player script

    [SerializeField] protected bool debug = false; // Debug toggle for logs

    protected virtual void Awake()
    {
        // Get the TextMeshProUGUI component from the child named "CooldownText"
        cooldownText = transform.Find("Cooldown").GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Start()
    {
        // Get the Image component from the child named "Fill"
        fill = transform.Find("Fill").GetComponent<Image>();

        if (fill != null)
        {
            fill.enabled = false;
        }

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

                // Set the cooldown text to the key code
                cooldownText.text = "";
            }
        }
    }

    protected virtual void OnEnable()
    {
        // Subscribe to PlayerRegisteredEvent to set key bindings dynamically
        EventDispatcher.AddListener<PlayerRegisteredEvent>(OnPlayerRegistered);
    }

    protected virtual void OnDisable()
    {
        // Unsubscribe from PlayerRegisteredEvent
        EventDispatcher.RemoveListener<PlayerRegisteredEvent>(OnPlayerRegistered);
    }

    private void OnPlayerRegistered(PlayerRegisteredEvent e)
    {
        player = e.player;
        playerScript = player.GetComponent<Player>();
    }

    public void UpdateKeyCode(KeyCode key)
    {
        if (keyCodeDisplay == null)
        {  
            keyCodeDisplay = transform.Find("KeyCode").Find("Text").GetComponent<TextMeshProUGUI>();
        }

        // Display the key code in the UI
        keyCodeDisplay.text = key.ToString();
    }

    public void ActivateSkill()
    {
        // Check if the skill is not on cooldown
        if (!isOnCooldown)
        {
            if (debug)
            {
                Debug.Log($"{skillName} activated.");
            }

            // Call the abstract ApplySkillEffect method for derived classes to define behavior
            bool success = ApplySkillEffect();

            if (!success)
            {
                // The skill effect failed, return
                return;
            }

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

    protected void DamageEnemy(GameObject enemy, float damage)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e == null)
        {
            e = enemy.GetComponentInParent<Enemy>();
        }

        if (e != null)
        {
            e.TakeDamage(damage);
        }
    }

    protected void KnockbackEnemy(GameObject enemy, float knockback, Vector3 direction)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e == null)
        {
            e = enemy.GetComponentInParent<Enemy>();
        }

        if (e != null)
        {
            e.TakeKnockback(knockback, direction);
        }
    }

    protected void DamageAndKnockbackEnemy(GameObject enemy, float damage, float knockback, Vector3 direction)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e == null)
        {
            e = enemy.GetComponentInParent<Enemy>();
        }

        if (e != null)
        {
            e.TakeDamage(damage);
            e.TakeKnockback(knockback, direction);
        }
    }

    // Abstract method to be implemented by derived classes
    protected abstract bool ApplySkillEffect();
}
