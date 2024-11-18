using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    [SerializeField] private bool isPlayer = false;

    void Awake() {
        // set isPlayer to true if the GameObject is tagged as "Player"
        if (gameObject.CompareTag("Player")) {
            isPlayer = true;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isPlayer)
        {
            EventDispatcher.Raise<PlayerCollisionEvent>(new PlayerCollisionEvent() {
                collision = collision
            });
        }
    }
}
