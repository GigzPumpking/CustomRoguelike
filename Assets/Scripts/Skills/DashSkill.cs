using UnityEngine;

public class DashSkill : Skill
{
    private Rigidbody playerRb; // Reference to the player's Rigidbody

    [SerializeField] private float dashForce = 20.0f; // The force of the dash
    [SerializeField] private float knockbackForce = 15.0f; // Force applied to objects in range
    [SerializeField] private float knockbackRadius = 5.0f; // Radius of the knockback effect
    [SerializeField] private LayerMask knockbackLayers; // Layers affected by knockback

    private bool isDashActive = false; // Indicates if the dash is currently active
    private float dashTimelimit = 1.0f; // Time limit for the dash

    private float dashTimer = 0.0f; // Timer for the dash

    protected override void OnEnable()
    {
        base.OnEnable();
        EventDispatcher.AddListener<PlayerCollisionEvent>(OnPlayerCollision);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventDispatcher.RemoveListener<PlayerCollisionEvent>(OnPlayerCollision);
    }

    protected override bool ApplySkillEffect()
    {
        // Ensure player information is initialized
        if (player == null)
        {
            InitializePlayer();
        }
        else if (playerRb == null)
        {
            playerRb = player.GetComponent<Rigidbody>();
        }

        if (playerRb != null && playerScript != null)
        {
            if (!isDashActive)
            {
                // Apply forward dash force
                Vector3 dashDirection = player.transform.forward;
                playerRb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

                dashTimer = 0.0f;
                isDashActive = true;

                if (debug)
                {
                    Debug.Log("DashSkill activated: Player dashed forward.");
                }

                return true;
            }
        }
        else
        {
            Debug.LogError("DashSkill: Player Rigidbody or PlayerScript is not assigned.");
            return false;
        }

        return false;
    }

    private void InitializePlayer()
    {
        player = GameManager.Instance.GetPlayer();
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
            playerScript = player.GetComponent<Player>();

            if (debug)
            {
                Debug.Log("Player initialized in DashSkill.");
            }
        }
    }

    public void OnPlayerCollision(PlayerCollisionEvent e)
    {
        if (isDashActive && playerRb != null && playerScript != null)
        {
            // Check if the player collided with an object
            if (e.collision != null)
            {
                ApplyKnockbackEffect();
                isDashActive = false;

                if (debug)
                {
                    Debug.Log($"DashSkill: Collision detected with {e.collision.gameObject.name}.");
                }
            }
        }
    }

    private void ApplyKnockbackEffect()
    {
        // Detect objects in the knockback radius
        Collider[] colliders = Physics.OverlapSphere(playerRb.position, knockbackRadius, knockbackLayers);

        foreach (Collider collider in colliders)
        {
            Rigidbody targetRb = collider.GetComponent<Rigidbody>();
            if (targetRb != null && targetRb != playerRb)
            {
                // Apply knockback force to nearby objects
                Vector3 knockbackDirection = (collider.transform.position - player.transform.position).normalized;
                targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

                if (debug)
                {
                    Debug.Log($"DashSkill: Knockback applied to {collider.gameObject.name}.");
                }
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (isDashActive)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer >= dashTimelimit)
            {
                isDashActive = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            // Visualize the knockback radius
            Gizmos.color = new Color(0, 1, 0, 0.3f); // Semi-transparent green
            Gizmos.DrawSphere(playerRb != null ? playerRb.position : Vector3.zero, knockbackRadius);
        }
    }
}
