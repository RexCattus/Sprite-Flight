using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float minSize = 1f;
    public float maxSize = 3f;
    public float minSpd = 100f;
    public float maxSpd = 300f;
    public float maxspinspd = 10f;

    public GameObject explosionEffect; // Prefab hiệu ứng nổ khi va chạm với Player
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float RandomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(RandomSize, RandomSize, 1);

        rb = GetComponent<Rigidbody2D>();
        float randomspd = Random.Range(minSpd, maxSpd) / RandomSize;

        Vector2 randomdir = Random.insideUnitCircle;
        rb.AddForce(randomdir * randomspd);

        float randomspin = Random.Range(-maxspinspd, maxspinspd);
        rb.AddTorque(randomspin);

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

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(Vector3.left * Time.deltaTime * 6);
        if (transform.position.x < -22f || transform.position.x > 22f || transform.position.y < -12f || transform.position.y > 12f)
        {
            Destroy(gameObject);
        }
        if (rb.linearVelocity.magnitude > maxSpd)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpd;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint = collision.GetContact(0).point;
        GameObject bounceEffect = Instantiate(explosionEffect, contactPoint, Quaternion.identity);
        Destroy(bounceEffect, 1f); // Hủy hiệu ứng sau 1 giây
    }
}
