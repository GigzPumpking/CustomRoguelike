using UnityEngine;

public class Explosion : MonoBehaviour
{
    void Update()
    {
        // Use billboarding to keep the explosion facing the camera
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }
}
