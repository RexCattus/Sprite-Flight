using System.Collections;
using UnityEngine;

public class DroneEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 7f; // Tự hủy nếu bay ra ngoài quá lâu

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;     
    [SerializeField] private int burstCount = 3;      // Số lượng đạn bắn ra
    [SerializeField] private float burstInterval = 0.12f; // Khoảng cách giữa các phát bắn

    [Header("Explosion & Debris")]
    [SerializeField] private float scatterForce = 5f;
    [SerializeField] private float torqueForce = 20f;
    [SerializeField] private float debrisLifeTime = 2.5f;
    [SerializeField] private GameObject explosionVFX;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Nhắm vào vị trí của Player ngay thời điểm Spawn
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            moveDirection = (player.transform.position - transform.position).normalized;
            
            //  Xoay đầu tàu về hướng mục tiêu
            transform.right = -moveDirection; 
        }
        else
        {
            moveDirection = Vector2.down;
        }

        // Phóng về phía trước
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }

        // Bắt đầu 3 phát
        StartCoroutine(ShootBurstRoutine());

        // Tự hủy sau khi bay hết quãng đường
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator ShootBurstRoutine()
    {
        // Chờ 0.2s sau khi xuất hiện rồi mới bắt đầu bắn
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < burstCount; i++)
        {
            if (isDead) yield break;

            Shoot();
            yield return new WaitForSeconds(burstInterval);
        }
    }

    private void Shoot()
{
    if (bulletPrefab != null && firePoint != null)
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}

    // Cơ chế phân rã mảnh vỡ
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        // Duyệt qua các bộ phận con
        Transform[] parts = GetComponentsInChildren<Transform>();

        foreach (Transform part in parts)
        {
            if (part == transform) continue; // Bỏ qua object cha

            part.SetParent(null); // Tách khỏi cha để không bị Destroy ngay lập tức

            // Gắn Rigidbody2D cho từng mảnh để tác dụng lực vật lý
            Rigidbody2D partRb = part.gameObject.AddComponent<Rigidbody2D>();
            partRb.gravityScale = 0f;

            // Tính hướng văng từ tâm tàu ra ngoài
            Vector2 scatterDir = (part.position - transform.position).normalized;
            if (scatterDir == Vector2.zero)
            {
                scatterDir = Random.insideUnitCircle.normalized;
            }

            partRb.AddForce(scatterDir * scatterForce, ForceMode2D.Impulse);
            partRb.AddTorque(Random.Range(-torqueForce, torqueForce), ForceMode2D.Impulse);

            // Tự hủy mảnh sau vài giây
            Destroy(part.gameObject, debrisLifeTime);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
       {
            if (HasActiveShield(collision.gameObject))
            {
                Die();
            }
            else
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.Die();
                }
                Die();
            }
            return;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
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
}