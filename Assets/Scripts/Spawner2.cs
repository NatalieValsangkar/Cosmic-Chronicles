using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class Spawner2 : MonoBehaviour
{
    [SerializeField]
    XRInputValueReader<Vector2> m_TapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position");

    public XRInputValueReader<Vector2> tapStartPositionInput
    {
        get => m_TapStartPositionInput;
        set => XRInputReaderUtility.SetInputProperty(ref m_TapStartPositionInput, value, this);
    }

    public ARPlaneManager arPlaneManager;
    public GameObject characterPrefab;
    public GameObject asteroidPrefab; // Asteroid prefab
    public GameObject gemPrefab; // Gem prefab
    public ARRaycastManager raycastManager;

    private GameObject spawnedCharacter;
    private Vector3 targetPosition;
    private bool isCharacterSpawned = false;
    private bool m_IsPerformed = false;
    private bool m_WasPerformedThisFrame = false;
    private bool m_WasCompletedThisFrame = false;
    private Vector2 m_TapStartPosition;

    public GameObject player;

    // Parameters for rings and rotation
    public int asteroidsPerRing = 10; // Asteroids per ring
    public int gemCount = 5; // Number of gems
    public float[] ringRadii = { 0f, 0.5f, 1f }; // Radii for 3 rings
    public float rotationSpeed = 20.0f; // Rotation speed of asteroids/gems

    private List<GameObject> spawnedAsteroids = new List<GameObject>();
    private List<GameObject> spawnedGems = new List<GameObject>();

    private int playerScore = 0;

    void Awake()
    {
        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager component is missing. Please attach it to the GameObject.");
        }
    }

    void Update()
    {
        var prevPerformed = m_IsPerformed;
        var prevTapStartPosition = m_TapStartPosition;
        var tapPerformedThisFrame = tapStartPositionInput.TryReadValue(out m_TapStartPosition) && prevTapStartPosition != m_TapStartPosition;

        m_IsPerformed = tapPerformedThisFrame;
        m_WasPerformedThisFrame = !prevPerformed && m_IsPerformed;
        m_WasCompletedThisFrame = prevPerformed && !m_IsPerformed;

        if (m_WasPerformedThisFrame)
        {
            HandleTap(m_TapStartPosition);
        }

        // Rotate objects (asteroids and gems) around the character
        RotateObjects(spawnedAsteroids);
        RotateObjects(spawnedGems);
    }

    void HandleTap(Vector2 tapPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(tapPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            if (!isCharacterSpawned)
            {
                // Spawn the character on the first tap and adjust position above the plane
                spawnedCharacter = Instantiate(characterPrefab, hitPose.position, hitPose.rotation);

                // Adjust the character to spawn fully above the plane
                AdjustCharacterHeight(spawnedCharacter, hitPose.position);

                // Spawn asteroids and gems in rings around the character
                SpawnRings(spawnedCharacter.transform.position);

                isCharacterSpawned = true;
            }

            player.SetActive(true);
        }
        else
        {
            Debug.Log("No plane hit detected at tap position.");
        }
    }

    void AdjustCharacterHeight(GameObject character, Vector3 hitPosition)
    {
        Collider characterCollider = character.GetComponent<Collider>();
        Renderer characterRenderer = character.GetComponent<Renderer>();

        float heightOffset = 2f;

        if (characterCollider != null)
        {
            heightOffset = characterCollider.bounds.extents.y;
        }
        else if (characterRenderer != null)
        {
            heightOffset = characterRenderer.bounds.extents.y;
        }

        character.transform.position = hitPosition + Vector3.up * heightOffset;
    }

    void SpawnRings(Vector3 characterPosition)
    {
        for (int i = 0; i < ringRadii.Length; i++)
        {
            SpawnAsteroidsInRing(characterPosition, ringRadii[i], asteroidsPerRing);
        }

        SpawnGemsInRings(characterPosition);
    }

    void SpawnAsteroidsInRing(Vector3 center, float radius, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2 / count;
            Vector3 spawnPosition = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y,
                center.z + Mathf.Sin(angle) * radius
            );

            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
            spawnedAsteroids.Add(asteroid);
        }
    }

    void SpawnGemsInRings(Vector3 center)
    {
        for (int i = 0; i < gemCount; i++)
        {
            // Randomly select a ring for each gem
            float radius = ringRadii[Random.Range(0, ringRadii.Length)];
            float angle = Random.Range(0, Mathf.PI * 2);

            Vector3 spawnPosition = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y,
                center.z + Mathf.Sin(angle) * radius
            );

            GameObject gem = Instantiate(gemPrefab, spawnPosition, Quaternion.identity);
            spawnedGems.Add(gem);

            // Add a trigger to detect when the player collects the gem
            Collider gemCollider = gem.GetComponent<Collider>();
            if (gemCollider != null)
            {
                gemCollider.isTrigger = true;
            }
        }
    }

    void RotateObjects(List<GameObject> objects)
    {
        if (spawnedCharacter == null || objects.Count == 0)
            return;

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                obj.transform.RotateAround(
                    spawnedCharacter.transform.position,
                    Vector3.up,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides with a gem
        if (spawnedGems.Contains(other.gameObject))
        {
            // Increment score
            playerScore += 10;

            // Destroy the gem
            Destroy(other.gameObject);
            spawnedGems.Remove(other.gameObject);

            Debug.Log("Gem collected! Current score: " + playerScore);
        }
    }
}

