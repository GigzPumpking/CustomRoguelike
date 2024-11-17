using UnityEngine;

public class SlamSkill : Skill
{
    private Rigidbody playerRb; // Reference to the player's Rigidbody
    [SerializeField] private float slamForce = 30.0f; // The downward force of the slam
    [SerializeField] private float slamRadius = 5.0f; // The radius of the slam effect
    [SerializeField] private bool debug = false; // Debug toggle

    private bool isSlamActive = false; // Indicates if the slam effect is waiting to be applied

    protected override void Start()
    {
        base.Start();

        // Get the player's Rigidbody from the GameManager
        GameObject player = GameManager.Instance.GetPlayer();

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("Player object not found via GameManager.");
        }
    }

    protected override void ApplySkillEffect()
    {
        if (playerRb != null)
        {
            // Apply a downward force to the player
            playerRb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);
            isSlamActive = true;

            if (debug)
            {
                Debug.Log("SlamSkill activated: Slam started");
            }
        }
        else
        {
            Debug.LogError("Player Rigidbody is not assigned.");
        }
    }

    public void OnPlayerCollision(Collision collision)
    {
        if (isSlamActive && playerRb != null)
        {
            // Check if the player collided with the ground
            if (GameManager.Instance.isObjectGrounded(playerRb.transform, 0.65f))
            {
                ApplySlamEffect();
            }
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
