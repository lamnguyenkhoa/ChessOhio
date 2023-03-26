using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    public Transform center; // The center point to orbit around
    public float speed = 1.0f; // The speed of the orbit

    void Update()
    {
        transform.RotateAround(center.position, Vector3.up, speed * Time.deltaTime);
    }
}