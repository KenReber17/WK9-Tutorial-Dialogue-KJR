using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [Header("Rotation Speeds (degrees per second)")]
    [SerializeField] private float xRotationSpeed = 10f;
    [SerializeField] private float yRotationSpeed = 2f;
    [SerializeField] private float zRotationSpeed = 2f;

    void Update()
    {
        // Rotate the planet around all three axes
        transform.Rotate(xRotationSpeed * Time.deltaTime, 0, 0, Space.Self);
        transform.Rotate(0, yRotationSpeed * Time.deltaTime, 0, Space.Self);
        transform.Rotate(0, 0, zRotationSpeed * Time.deltaTime, Space.Self);
    }
}