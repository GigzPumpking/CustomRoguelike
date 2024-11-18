using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthBar; // The health bar of the object
    [SerializeField] private bool billboarding = true; // Whether the health bar should face the camera

    void Awake()
    {
        // Get the health bar component from the object
        healthBar = GetComponentInChildren<Slider>();
    }

    void Update()
    {
        // Use billboarding to keep the health bar facing the camera
        if (healthBar != null && billboarding)
        {
            healthBar.transform.LookAt(Camera.main.transform);
            healthBar.transform.Rotate(0, 180, 0);
        }
    }

    public void SetHealth(float health)
    {
        // Set the health value of the object
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }

    public void SetMaxHealth(float maxHealth)
    {
        // Set the max health value of the object
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
    }
}
