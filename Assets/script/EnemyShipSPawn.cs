using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private float spawnInterval = 3.5f;
    [SerializeField] private float screenOffset = 1.5f; // Khoảng cách nằm ngoài mép camera

    private Camera mainCamera;
    private float timer;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnAtRandomCorner();
            timer = 0f;
        }
    }

    private void SpawnAtRandomCorner()
    {
        if (dronePrefab == null || mainCamera == null) return;

        // Tính kích thước nửa màn hình theo trục X và Y của Camera 2D
        float camHeight = mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector3 camPos = mainCamera.transform.position;

        // 4 góc ngoài tầm nhìn kết hợp screenOffset
        Vector3[] corners = new Vector3[]
        {
            //new Vector3(camPos.x - camWidth - screenOffset, camPos.y - camHeight - screenOffset, 0f), // Dưới Trái
            new Vector3(camPos.x + camWidth + screenOffset, camPos.y - camHeight - screenOffset, 0f), // Dưới Phải
            //new Vector3(camPos.x - camWidth - screenOffset, camPos.y + camHeight + screenOffset, 0f), // Trên Trái
            new Vector3(camPos.x + camWidth + screenOffset, camPos.y + camHeight + screenOffset, 0f)  // Trên Phải
        };

        // Chọn ngẫu nhiên 1 góc để sinh tàu địch
        Vector3 spawnPos = corners[Random.Range(0, corners.Length)];
        Instantiate(dronePrefab, spawnPos, Quaternion.identity);
    }
}