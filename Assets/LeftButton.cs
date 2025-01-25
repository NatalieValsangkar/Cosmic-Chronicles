using UnityEngine;

public class LeftButton : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed for horizontal movement
    public float jumpForce = 10f; // Force for jumping
    private Rigidbody2D rb; // Reference to the Rigidbody2D
    private bool isMovingLeft = false; // Track if moving left
    private bool isMovingRight = false; // Track if moving right

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D on the same GameObject
    }

    void Update()
    {
        // Update the player's horizontal velocity based on button states
        if (isMovingLeft)
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
        else if (isMovingRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else
        {
            // Stop horizontal movement if no button is pressed
            //rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    // Called when the Left button is pressed
    public void OnLeftButtonDown()
    {
        isMovingLeft = true;
    }

    // Called when the Left button is released
    public void OnLeftButtonUp()
    {
        isMovingLeft = false;
    }

    // Called when the Right button is pressed
    public void OnRightButtonDown()
    {
        isMovingRight = true;
    }

    // Called when the Right button is released
    public void OnRightButtonUp()
    {
        isMovingRight = false;
    }

    // Called to make the player jump
    public void Jump()
    {
        Debug.Log("Jump button pressed");
        // Check if the player is grounded (vertical speed close to 0)
        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            Debug.Log("Player is grounded. Jumping!");
            // rb.velocity = new Vector2(rb.velocity.x, 0); // Reset vertical velocity to avoid stacking velocity
            rb.AddForce(new Vector2(0, jumpForce)); //, ForceMode2D.Impulse); // Apply jump force
        }
        else
        {
            Debug.Log("Player is not grounded. Cannot jump.");
        }
    }
}
