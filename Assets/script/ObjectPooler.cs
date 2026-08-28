using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // Cấu trúc của 1 ngăn tủ chứa đồ
    [System.Serializable]
    public class Pool
    {
        public string tag;          // Tên loại đồ (VD: "Bullet")
        public GameObject prefab;   // Prefab của món đồ (Viên đạn)
        public int size;            // Số lượng đồ chuẩn bị sẵn (VD: 20 viên)
    }

    public static ObjectPooler Instance; // Singleton để gọi từ bất kỳ script nào

    public List<Pool> pools; // Danh sách các ngăn tủ (ngăn chứa đạn, ngăn chứa đá...)
    public Dictionary<string, Queue<GameObject>> poolDictionary; // Cuốn sổ quản lý xem đồ nào đang ở trong tủ

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Quét qua tất cả các ngăn tủ bạn đã cài đặt
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Tạo sẵn số lượng object (size) và TẮT CHÚNG ĐI (cất vào tủ)
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj); // Nhét vào tủ
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // Hàm này sẽ được gọi thay cho Instantiate()
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        // Lấy object cũ nhất ra khỏi tủ
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // TỰ ĐỘNG SỬA LỖI (SELF-HEAL): 
        // Nếu object đã bị một script nào đó lỡ tay Destroy(), ta tạo lại cái mới thay thế
        if (objectToSpawn == null)
        {
            Debug.LogWarning("Một object trong Pool (" + tag + ") đã bị Destroy. Đang tự động tạo lại...");
            foreach (Pool p in pools)
            {
                if (p.tag == tag)
                {
                    objectToSpawn = Instantiate(p.prefab);
                    break;
                }
            }
            
            if (objectToSpawn == null) return null;
        }

        // Kích hoạt nó lên và đặt vị trí mới
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Xếp hàng nó lại vào cuối tủ (để xoay vòng tái sử dụng)
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}