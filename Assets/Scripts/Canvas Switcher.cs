using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvas1; // First canvas
    public GameObject canvas2; // Second canvas

    public void OpenCanvas2()
    {
        canvas1.SetActive(false); // Hide first canvas
        canvas2.SetActive(true);  // Show second canvas
    }
}