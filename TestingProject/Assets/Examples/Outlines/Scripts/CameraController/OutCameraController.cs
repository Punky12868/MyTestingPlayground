using UnityEngine;
using Rewired;

public class OutCameraController : MonoBehaviour
{
    Player input;

    [SerializeField] float normalMoveSpeed;
    [SerializeField] float fastMoveSpeed;
    [SerializeField] float sensitivity = 1f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private float moveSpeed;

    private void Awake()
    {
        input = ReInput.players.GetPlayer(0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MoveCamera();
        RotateCamera();
    }

    private void MoveCamera()
    {
        // Get the input from the keyboard or joystick
        float moveX = input.GetAxisRaw("Horizontal");
        float moveY = input.GetAxisRaw("Vertical");
        float moveZ = input.GetAxisRaw("YAxis");

        // Calculate the move speed based on the input
        moveSpeed = input.GetButton("Run") ? fastMoveSpeed : normalMoveSpeed;

        // Calculate the movement based on the input
        Vector3 move = new Vector3(moveX, moveZ, moveY) * moveSpeed * Time.deltaTime;

        // Apply the movement to the transform
        transform.Translate(move);
    }

    private void RotateCamera()
    {
        // Get the input from the mouse or joystick
        float mouseX = input.GetAxisRaw("Look X");
        float mouseY = input.GetAxisRaw("Look Y");

        // Update the rotation based on the input
        rotationX -= mouseY * sensitivity;
        rotationY += mouseX * sensitivity;

        // Clamp the x rotation to prevent flipping
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // Apply the rotation to the transform
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
