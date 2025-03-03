using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float gravity = -9.81f;

    private CharacterController characterController;
    private float verticalRotation = 0.0f;
    private Vector3 velocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // ?}?E?X?J?[?\???????b?N
        Cursor.visible = false; // ?}?E?X?J?[?\?????\??
    }

    void Update()
    {
        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Reset vertical velocity each frame
        velocity.y = 0f;

        // Vertical movement (Space/Shift)
        if (Input.GetKey(KeyCode.Space))
        {
            velocity.y += speed;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity.y -= speed;
        }
        Vector3 move = transform.right * x + transform.forward * z + Vector3.up * velocity.y;
        characterController.Move(move * speed * Time.deltaTime);

        // Camera Rotation (Mouse Look)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
