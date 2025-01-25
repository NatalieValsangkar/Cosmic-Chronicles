using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class StartButton : MonoBehaviour
{
    // This function will be called when the button is clicked
    public void StartGame()
    {
        Debug.Log("Game is starting...");

        // Load the main game scene
        // Replace "GameScene" with the name of your scene
        SceneManager.LoadScene("1. Game Room");
    }
}
