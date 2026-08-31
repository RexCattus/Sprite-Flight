using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // Cấu trúc 1 ngăn tủ chứa đồ
    [System.Serializable]
    public class Pool
    {
        public string tag;          // Tên nhóm đồ cần pool
        public GameObject prefab;   // Prefab của món đó
        public int size;            // Số lượng chuẩn bị sẵn
    }

    public static ObjectPooler Instance; // Singleton để gọi từ bất kỳ script nào

    public List<Pool> pools; // Danh sách các ngăn
    public Dictionary<string, Queue<GameObject>> poolDictionary; // Sổ quản lý xem đồ nào đang ở trong tủ

    void Awake()
    {
        Instance = this; // gắn nguyên cái script này vào (biến script này có thể truy cập từ mọi script khác mà ko cần kéo thả)
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Quét qua tất cả các ngăn tủ đã setup
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Tạo sẵn số lượng object (theo size đã setup trong inspector) xong tắt đi
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj); // Nhét vào tủ
            }

            poolDictionary.Add(pool.tag, objectPool); // sau đó đưa cái ngăn đã setup đó vào từ điển chờ được dùng
        }
    }

    // Hàm này sẽ được gọi thay cho Instantiate() trong các script khác
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        // Lấy object cũ nhất ra khỏi tủ
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // Nếu object đã bị một script nào đó lỡ tay Destroy(), ta tạo lại cái mới thay thế
        if (objectToSpawn == null)
        {
            Debug.LogWarning("Một object trong Pool (" + tag + ") đã bị Destroy. Đang tạo lại");
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
        // mấy cái pos và rot này được truyền vào bằng các script khác khi dùng hàm này
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Xếp hàng nó lại vào cuối tủ (tái sử dụng)
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}