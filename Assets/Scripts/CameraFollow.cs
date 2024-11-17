using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The Camera object will follow the player object in the third person view

    public Transform player; // The player object
    private Camera camera; // The camera object
    [SerializeField] private bool cursorState = false;

    public float sensitivity = 100f; // Mouse sensitivity
    public float verticalClamp = 80f; // Limit vertical rotation (pitch)

    private float pitch = 0f; // Vertical rotation (up/down)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the camera from children
        camera = GetComponentInChildren<Camera>();

        // Lock the cursor
        lockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the camera based on mouse input
        float horizontal = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime; // Horizontal rotation
        float vertical = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime; // Vertical rotation

        // Adjust the pitch (vertical rotation) and clamp it
        pitch -= vertical;
        pitch = Mathf.Clamp(pitch, -verticalClamp, verticalClamp);

        // Apply horizontal rotation to the player
        player.Rotate(Vector3.up * horizontal);

        // Apply vertical rotation (pitch) to the camera
        transform.localEulerAngles = new Vector3(pitch, player.eulerAngles.y, 0f);

        // Update the camera's position
        transform.position = player.position;

        // Handle cursor state toggling
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            setCursorState(!cursorState);
        }
    }

    public void setCursorState(bool state)
    {
        if (state)
        {
            unlockCursor();
        }
        else
        {
            lockCursor();
        }
    }

    public void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorState = false;
    }

    public void unlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorState = true;
    }
}
