using UnityEngine;

public class SpikedSphere : MonoBehaviour
{
    [Header("Spike Settings")]
    [SerializeField] private float spikeLength = 0.03f; // Length of spikes
    [SerializeField] private float spikeFrequency = 5f; // How frequently spikes appear (lower is more frequent)
    [SerializeField] private float pulseSpeed = 1f; // Speed of pulsing for spikes
    [SerializeField] private float pulseIntensity = 0.5f; // How much spikes will pulse

    private MeshFilter meshFilter;
    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private Vector3[] modifiedVertices;

    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null)
        {
            Debug.LogError("MeshFilter component not found on this GameObject.");
            return;
        }

        meshFilter = filter;
        originalMesh = meshFilter.mesh;
        originalVertices = originalMesh.vertices;
        modifiedVertices = new Vector3[originalVertices.Length];
        originalVertices.CopyTo(modifiedVertices, 0);
    }

    void Update()
    {
        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            Vector3 originalVert = originalVertices[i];
            
            // Use noise to determine where spikes should be
            float noise = Mathf.PerlinNoise(originalVert.x * spikeFrequency + Time.time, originalVert.y * spikeFrequency);
            if (noise > 0.5f) // This threshold determines sparsity of spikes
            {
                // Direction of spike
                Vector3 spikeDir = originalVert.normalized;
                
                // Pulse effect for the spike
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity + 1f;
                float spikeScale = spikeLength * pulse;
                
                // Add spike to the vertex
                modifiedVertices[i] = originalVert + spikeDir * spikeScale;
            }
            else
            {
                modifiedVertices[i] = originalVert; // No spike, keep original position
            }
        }

        // Apply the modified vertices to the mesh
        meshFilter.mesh.vertices = modifiedVertices;
        meshFilter.mesh.RecalculateNormals(); // To ensure lighting looks correct with new geometry
    }

    // Clean up on disable to revert to original mesh if needed
    void OnDisable()
    {
        if (meshFilter != null && originalMesh != null)
        {
            meshFilter.mesh = originalMesh;
        }
    }
}