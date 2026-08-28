using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Cài đặt Di Chuyển")]
    public float speed = 3f;
    public float spinSpeed = 30f;

    void OnEnable()
    {

        // Bỏ qua va chạm với các object có tag "wall" hoặc "Wall"
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            Collider2D[] allColliders = Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
            foreach (Collider2D col in allColliders)
            {
                if (col.gameObject.tag == "Wall" || col.gameObject.tag == "wall")
                {
                    Physics2D.IgnoreCollision(myCollider, col, true);
                }
            }
        }
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
        // Tự hủy khi vượt ngoài giới hạn màn hình
        if (transform.position.x < -35f || transform.position.x > 45f || transform.position.y < -12.5f || transform.position.y > 12.5f)
        {
            // Destroy(gameObject);
            gameObject.SetActive(false); // Cho object pooling
        }

    }
}
