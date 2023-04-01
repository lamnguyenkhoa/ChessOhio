using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The object to orbit around
    public float rotationSpeed = 5.0f;
    public float distance;
    private float mouseX;
    private float mouseY;
    public float zoomSpeed = 2.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    void Start()
    {
        distance = Vector3.Distance(transform.position, target.position);
        mouseX = transform.rotation.eulerAngles.y;
        mouseY = transform.rotation.eulerAngles.x;
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0f);
        transform.position = target.position - (rotation * Vector3.forward * distance);

        if (maxDistance < distance)
        {
            maxDistance = distance;
        }
    }


    void Update()
    {
        if (Chessboard.instance.disableRaycastCount > 0)
        {
            return;
        }

        if (Input.GetMouseButton(1)) // Check if the right mouse button is held down
        {
            mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
            mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            mouseY = Mathf.Clamp(mouseY, -90f, 90f);
        }

        // Zoom the camera in and out based on the mouse scroll wheel
        distance -= Input.mouseScrollDelta.y * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Calculate the new position of the camera based on the target object's position and the camera's distance and rotation
        Vector3 targetPosition = target.position;
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0f);
        Vector3 cameraPosition = targetPosition - (rotation * Vector3.forward * distance);

        transform.rotation = rotation;
        transform.position = cameraPosition;
    }
}