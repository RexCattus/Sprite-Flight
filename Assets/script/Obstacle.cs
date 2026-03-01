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
        transform.localScale = new Vector3(RandomSize,RandomSize, 1);

        rb = GetComponent<Rigidbody2D>();
        float randomspd = Random.Range(minSpd, maxSpd)/RandomSize;

        Vector2 randomdir=Random.insideUnitCircle;
        rb.AddForce(randomdir * randomspd);

        float randomspin=Random.Range(-maxspinspd, maxspinspd);
        rb.AddTorque(randomspin);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * 6);
        if(transform.position.x<-30f||transform.position.y<-20f||transform.position.y>20f)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 contactPoint=collision.GetContact(0).point;
        GameObject bounceEffect=Instantiate(explosionEffect, contactPoint, Quaternion.identity);
        Destroy(bounceEffect, 1f); // Hủy hiệu ứng sau 1 giây
    }
}
