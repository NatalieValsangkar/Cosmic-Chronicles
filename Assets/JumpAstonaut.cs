using UnityEngine;
using UnityEngine.UI;

public class JumpButton : MonoBehaviour
{
    public Rigidbody playerRigidbody;  // The player's Rigidbody component
    public Button jumpButton;         // The UI Button for jumping
    public float jumpForce = 5f;      // The force applied for jumping

    private bool isGrounded = true;   // Check if the player is on the ground

    void Start()
    {
        // Ensure the Rigidbody and Button components are assigned
        if (playerRigidbody == null)
        {
            Debug.LogError("Player Rigidbody is not assigned!");
        }

        if (jumpButton == null)
        {
            Debug.LogError("Jump Button is not assigned!");
        }

        // Add a listener to the jump button to call the Jump method when clicked
        jumpButton.onClick.AddListener(Jump);
    }

    void Jump()
    {
        // Allow jumping only if the player is grounded
        if (isGrounded)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Player is now in the air
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player touches the ground to enable jumping again
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}

