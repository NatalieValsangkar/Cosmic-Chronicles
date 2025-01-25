using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public string mainGameSceneName = "2. Triton's Leap Original"; // Replace with your scene name

    public void OnStartButtonClick()
    {
        if (!string.IsNullOrEmpty(mainGameSceneName)) // Check if the scene name is valid
        {
            SceneManager.LoadScene(mainGameSceneName); // Load the scene
        }
        else
        {
            Debug.LogError("Main game scene name is not set in the StartGameButton script!"); // Log error
        }
    }
}
