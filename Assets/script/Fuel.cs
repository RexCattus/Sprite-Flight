using UnityEngine;

public class Fuel : MonoBehaviour
{
    [Header("Cài đặt Di Chuyển")]
    public float speed = 3f;
    public float spinSpeed = 30f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.angularVelocity = Random.Range(-spinSpeed, spinSpeed);
        }

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
        // Di chuyển sang trái giống như chướng ngại vật
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        }

        // Tự hủy khi vượt ngoài giới hạn màn hình
        if (transform.position.x < -22f || transform.position.x > 22f || transform.position.y < -12f || transform.position.y > 12f)
        {
            Destroy(gameObject);
        }
    }
}
