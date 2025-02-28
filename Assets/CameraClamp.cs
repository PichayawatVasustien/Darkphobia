using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;
    [SerializeField] private GameObject topBorder;
    [SerializeField] private GameObject bottomBorder;

    private Camera mainCamera;

    private float minX, maxX, minY, maxY;

    private void Start()
    {
        mainCamera = Camera.main;

        if (leftBorder == null || rightBorder == null || topBorder == null || bottomBorder == null)
        {
            Debug.LogError("Assign all four border GameObjects in the Inspector!");
            return;
        }

        // Get the boundary positions from the GameObjects
        minX = leftBorder.transform.position.x;
        maxX = rightBorder.transform.position.x;
        minY = bottomBorder.transform.position.y;
        maxY = topBorder.transform.position.y;
    }

    private void LateUpdate()
    {
        ClampCameraPosition();
    }

    private void ClampCameraPosition()
    {
        if (mainCamera == null) return;

        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float clampedX = Mathf.Clamp(transform.position.x, minX + camWidth / 2, maxX - camWidth / 2);
        float clampedY = Mathf.Clamp(transform.position.y, minY + camHeight / 2, maxY - camHeight / 2);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
