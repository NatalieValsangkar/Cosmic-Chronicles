using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    public int playerScore = 0; // Player's score

    private void OnTriggerEnter(Collider other)
    {
        // If player collects a gem
        if (other.CompareTag("Gem"))
        {
            playerScore += 10; // Add points
            Destroy(other.gameObject); // Remove the gem
            Debug.Log("Gem collected! Score: " + playerScore);
        }
        // If player hits an asteroid
        else if (other.CompareTag("Asteroid"))
        {
            Debug.Log("Hit an asteroid! Game Over.");
            SceneManager.LoadScene("GameOverScene"); // Load Game Over scene
        }
        // If player reaches the character
        else if (other.CompareTag("Character"))
        {
            Debug.Log("Player reached the character! You Won!");
            SceneManager.LoadScene("YouWonScene"); // Load "You Won" scene
        }
    }
}
