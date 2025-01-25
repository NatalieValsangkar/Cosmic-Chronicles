using UnityEngine;

public class EndFlag : MonoBehaviour
{
    public GameObject gameCompleteUI; // Reference to the Game Complete UI

    private void OnTriggerEnter(Collider other) // For 3D games
    {
        if (other.CompareTag("Player")) // Check if the player hits the flag
        {
            ShowGameCompleteScreen();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // For 2D games
    {
        if (other.CompareTag("Player")) // Check if the player hits the flag
        {
            ShowGameCompleteScreen();
        }
    }

    void ShowGameCompleteScreen()
    {
        // Activate the Game Complete UI
        if (gameCompleteUI != null)
        {
            gameCompleteUI.SetActive(true);
        }

        // Stop the game (optional)
        Time.timeScale = 0f;
    }
}
