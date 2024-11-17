using UnityEngine;

public class JumpSkill : Skill
{
    private Rigidbody playerRb; // The Rigidbody of the player
    [SerializeField] private float jumpForce = 10.0f; // The force of the jump
    [SerializeField] private bool debug = false; // Debug toggle

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
            // Add force to the player's Rigidbody upwards
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (debug)
            {
                Debug.Log("JumpSkill activated: Jumped");
            }
        }
        else
        {
            Debug.LogError("Player Rigidbody is not assigned.");
        }
    }
}
