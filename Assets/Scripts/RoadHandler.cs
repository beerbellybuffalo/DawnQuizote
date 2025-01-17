using System;
using UnityEngine;

/*This script is to be attached to road prefabs*/

public class RoadHandler : MonoBehaviour
{
    public GameObject roadPrefab; // Prefab of the road section
    private GameObject currentRoadSection; // Reference to the current road section
    public float Speed = 4.0f;

    void Start()
    {
        // Initialize
        currentRoadSection = transform.root.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, 0, Speed) * Time.deltaTime;
    }

    public void SpawnNextRoad()
    {
        // Get the mesh bounds
        Mesh mesh = roadPrefab.GetComponent<MeshFilter>().sharedMesh;
        Vector3 meshSize = mesh.bounds.size;

        // Calculate the road length in world space
        float roadLength = meshSize.z * roadPrefab.transform.localScale.z;

        // Calculate the new position
        Vector3 newPosition = currentRoadSection.transform.position + new Vector3(0, 0, roadLength);

        // Instantiate the next road section
        Instantiate(roadPrefab, newPosition, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the trigger, checking if it was player...");
        // Check if the player passes through the current road section's trigger
        if (other.CompareTag("Player"))
        {
            Debug.Log("Instantiating New Section!");
            SpawnNextRoad();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            Debug.Log("Player has moved to next section, destroying road...");
            Destroy(currentRoadSection);
        }
    }
}
