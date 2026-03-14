using UnityEngine;
using UnityEngine.InputSystem;

public class PathFindingManager : MonoBehaviour
{
    public Transform player;
    public Transform target;
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            // Chuyển trực tiếp tọa độ chuột thành tọa độ trong Game
            mousePos.z = 10f; // Khoảng cách từ camera đến mặt phẳng game
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
            worldPoint.z = 0; // Luôn giữ Z = 0 cho game 2D

            // Bắn tia laser tại đúng vị trí chuột để xem có trúng BG/Mặt đất không
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                target.position = worldPoint; // Đặt Target đúng chỗ chuột bấm
                grid.FindPath(player.position, target.position);
            }
        }
    }
}
