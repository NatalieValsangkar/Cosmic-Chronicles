using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.SceneManagement;


public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    XRInputValueReader<Vector2> m_TapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position");

    public XRInputValueReader<Vector2> tapStartPositionInput
    {
        get => m_TapStartPositionInput;
        set => XRInputReaderUtility.SetInputProperty(ref m_TapStartPositionInput, value, this);
    }

    private Camera arCamera;

    private bool m_IsPerformed = false;
    private bool m_WasPerformedThisFrame = false;
    private Vector2 m_TapStartPosition;

    void Awake()
    {
        // Ensure AR Camera is set
        arCamera = Camera.main;
        if (arCamera == null)
        {
            Debug.LogError("Main Camera is missing. Ensure you have a Camera tagged as MainCamera in the scene.");
        }
    }

    void Update()
    {
        var prevPerformed = m_IsPerformed;
        var prevTapStartPosition = m_TapStartPosition;
        var tapPerformedThisFrame = tapStartPositionInput.TryReadValue(out m_TapStartPosition) && prevTapStartPosition != m_TapStartPosition;

        m_IsPerformed = tapPerformedThisFrame;
        m_WasPerformedThisFrame = !prevPerformed && m_IsPerformed;

        if (m_WasPerformedThisFrame)
        {
            HandleTap(m_TapStartPosition);
        }
    }

    void HandleTap(Vector2 tapPosition)
    {
        // Convert tap position (screen coordinates) to a ray
        Ray ray = arCamera.ScreenPointToRay(tapPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != null)
            {
                Debug.Log($"Tapped on GameObject: {hitObject.name}");
                Debug.Log(hitObject.tag);
                if (hitObject.tag == "Rocket")
                {
                    SceneManager.LoadScene("2. Saturn Game", LoadSceneMode.Single);
                }
            }
        }
        else
        {
            Debug.Log("No object detected at the tap position.");
        }
    }
}
