using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 10f;
    PlayerController playerController;
    [SerializeField] private AudioClip RockExplosionClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playerController.score += 10;
            
            // Phát âm thanh nổ ngay tại vị trí Camera để nghe rõ nhất, trước khi Đạn bị hủy
            if (RockExplosionClip != null)
            {
                AudioSource.PlayClipAtPoint(RockExplosionClip, Camera.main.transform.position);
            }

            collision.gameObject.GetComponent<Obstacle>().BreakBigRock();
            
            Destroy(collision.gameObject);
            Destroy(gameObject); // Hủy đạn sau cùng
        }
    }
}
