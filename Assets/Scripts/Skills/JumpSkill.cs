using UnityEngine;

public class JumpSkill : Skill
{
    private Rigidbody playerRb; // Reference to the player's Rigidbody
    [SerializeField] private float jumpForce = 10.0f; // The force of the jump

    protected override void ApplySkillEffect()
    {
        // Ensure player information is initialized
        if (player == null) {
            InitializePlayer();
        } else if (playerRb == null) {
            playerRb = player.GetComponent<Rigidbody>();
        }

        if (playerRb != null && playerScript != null)
        {
            if (playerScript.IsGrounded()) {
                // Add force to the player's Rigidbody upwards
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                if (debug) // Use the debug field inherited from Skill
                {
                    Debug.Log("JumpSkill activated: Jumped");
                }
            } else {
                if (debug) // Use the debug field inherited from Skill
                {
                    Debug.Log("JumpSkill activated: Player is not grounded.");
                }
            }
        }
        else
        {
            Debug.LogError("Player is not assigned.");
        }
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
                Debug.Log("Player initialized in JumpSkill.");
            }
        }
    }
}
