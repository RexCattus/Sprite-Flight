using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public Transform targetTransform;
    private List<Node> _path; // Dùng biến phụ để kiểm soát
    public List<Node> path
    {
        get => _path;
        set
        {
            _path = value;
            targetIndex = 0; // <--- QUAN TRỌNG: Reset chỉ số mỗi khi nhận đường mới
        }
    }
    int targetIndex;
    public float spd = 5f;
    void Start()
    {
        path = null;
        targetIndex = 0;
    }

    void Update()
    {
        // 1. Nếu còn đường đi trong danh sách ô lưới
        if (path != null && targetIndex < path.Count)
        {
            Node targetNode = path[targetIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetNode.worldPos, spd * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetNode.worldPos) < 0.05f)
            {
                targetIndex++;
            }
        }
        // 2. Nếu đã đi hết ô lưới, tiến nốt bước cuối tới Target thực sự
        else if (path != null && targetIndex >= path.Count)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, spd * Time.deltaTime);

            // DỪNG LẠI khi khoảng cách cực nhỏ
            if (Vector2.Distance(transform.position, targetTransform.position) < 1.2f)
            {
                path = null; // Xóa đường đi để Player đứng yên
                targetIndex = 0;
                Debug.Log("Đã đến đích và dừng lại!");
            }
        }
    }
}
