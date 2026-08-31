using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    PlayerController playerController;
    [SerializeField] private AudioClip RockExplosionClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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
        HandleHit(collision.gameObject);
    }

    private void HandleHit(GameObject hitTarget)
    {
        // Thiên thạch
        if (hitTarget.TryGetComponent<Obstacle>(out Obstacle rock)) // tìm Component Obstacle rồi đưa vào một biến mới là rock 
        {
            if (playerController != null)
            {
                playerController.score += 10;
            }
            if (RockExplosionClip != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(RockExplosionClip, Camera.main.transform.position);
            }
            rock.BreakBigRock();
            gameObject.SetActive(false); // Cho object pooling
            return;
        }

        // Enemy Ship
        if (hitTarget.TryGetComponent<DroneEnemy>(out DroneEnemy drone))
        {
            if (playerController != null)
            {
                playerController.score += 10;
            }
            drone.Die();
            gameObject.SetActive(false);
            return;
        }

        // Player ( từ Enemy ship)
        if (hitTarget.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (!HasActiveShield(hitTarget))
            {
                player.Die();
            }
            gameObject.SetActive(false);
            return;
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
