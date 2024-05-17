using UnityEngine;
using Rewired;

public class FollowTargetRotation : MonoBehaviour
{
    Player input;

    [SerializeField] Transform target;
    [SerializeField] float sensitivity = 1f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Awake()
    {
        input = ReInput.players.GetPlayer(0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Make sure the object follows the target's position
        transform.position = target.position;

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
