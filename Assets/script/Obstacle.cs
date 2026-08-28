using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private float minSize = 5f;
    [SerializeField] private float maxSize = 10f;
    [SerializeField] private float minSpd = 100f;
    [SerializeField] private float maxSpd = 300f;
    [SerializeField] private float maxspinspd = 10f;

    [Header("References")]
    [SerializeField] private GameObject explosionEffect;

    [Header("Big rock Settings")]
    [SerializeField] private bool isBigRock = true;
    [SerializeField] private GameObject[] smallRockPrefab;
    [SerializeField] private int smallRockCount = 3;
    [SerializeField] private float rockSplitForce = 200f;

    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Xóa lực bay cũ đi
            rb.angularVelocity = 0f;
        }

        float RandomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(RandomSize, RandomSize, 1);

        rb = GetComponent<Rigidbody2D>();
        float randomspd = Random.Range(minSpd, maxSpd) / RandomSize;

        rb.AddForce(Vector2.left * randomspd);

        float randomspin = Random.Range(-maxspinspd, maxspinspd);
        rb.AddTorque(randomspin);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(Vector3.left * Time.deltaTime * 6);
        if (transform.position.x < -35f || transform.position.x > 45f || transform.position.y < -12.5f || transform.position.y > 12.5f)
        {
            // Destroy(gameObject);
            gameObject.SetActive(false); // Cho object pooling
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

        if (collision.gameObject.CompareTag("Player"))
        {
            if (HasActiveShield(collision.gameObject))
            {
                BreakBigRock();
                // Destroy(gameObject);
                gameObject.SetActive(false);
                return;
            }
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.Die();
        }
    }

    private bool HasActiveShield(GameObject target)
    {
        Transform[] allChildren = target.GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Shield") && child.gameObject.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }

    public void BreakBigRock()
    {
        if (isBigRock && smallRockPrefab != null)
        {
            for (int i = 0; i < smallRockCount; i++)
            {
                GameObject smallRock = Instantiate(smallRockPrefab[Random.Range(0, smallRockPrefab.Length)], transform.position, Quaternion.identity);

                Rigidbody2D smallRockRb = smallRock.GetComponent<Rigidbody2D>();
                if (smallRockRb != null)
                {
                     Vector2 randomDir = Random.insideUnitCircle.normalized; // Tạo một hướng ngẫu nhiên
                     smallRockRb.AddForce(randomDir * rockSplitForce);
                }
            }
        }
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
