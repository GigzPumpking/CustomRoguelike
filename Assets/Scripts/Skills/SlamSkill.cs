using UnityEngine;

public class SlamSkill : Skill
{
    private Rigidbody playerRb; // Reference to the player's Rigidbody
    [SerializeField] private float slamForce = 30.0f; // The downward force of the slam
    [SerializeField] private float slamRadius = 5.0f; // The radius of the slam effect

    private bool isSlamActive = false; // Indicates if the slam effect is waiting to be applied

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
        if (player == null) {
            InitializePlayer();
        } else if (playerRb == null) {
            playerRb = player.GetComponent<Rigidbody>();
        }

        if (playerRb != null && playerScript != null)
        {
            if (!playerScript.IsGrounded())
            {
                // Apply a downward force to the player
                playerRb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);
                isSlamActive = true;
                playerScript.SetInvulnerable(true);

                if (debug)
                {
                    Debug.Log("SlamSkill activated: Slam started");
                }

                return true;
            } 
            else if (debug)
            {
                Debug.Log("SlamSkill activated: Player is grounded.");
                return false;
            }
        }
        else
        {
            Debug.LogError("Player Rigidbody is not assigned.");
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
                Debug.Log("Player initialized in SlamSkill.");
            }
        }
    }

    public void OnPlayerCollision(PlayerCollisionEvent e)
    {
        Debug.Log("SlamSkill: Player collided with an object.");
        if (isSlamActive && playerRb != null && playerScript != null)
        {
            ApplySlamEffect();
        }
    }

    private void ApplySlamEffect()
    {
        // Detect objects in the slam radius
        Collider[] colliders = Physics.OverlapSphere(playerRb.position, slamRadius);

        foreach (Collider collider in colliders)
        {
            Rigidbody targetRb = collider.GetComponent<Rigidbody>();
            if (targetRb != null && targetRb != playerRb)
            {
                // Add force to nearby objects
                targetRb.AddForce(Vector3.up * (slamForce / 2), ForceMode.Impulse);
            }
        }

        // Debugging
        if (debug)
        {
            Debug.Log($"SlamSkill effect applied: {colliders.Length} objects affected within radius {slamRadius}");
        }

        // Reset slam state
        isSlamActive = false;
        playerScript.SetInvulnerable(false);
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f); // Semi-transparent red
            Gizmos.DrawSphere(playerRb != null ? playerRb.position : Vector3.zero, slamRadius);
        }
    }
}
