using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 15f;
    PlayerController playerController;
    [SerializeField] private AudioClip RockExplosionClip;
    [SerializeField] private AudioClip ShipExplosionClip;

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
        if (hitTarget.TryGetComponent<Obstacle>(out Obstacle rock))
        {
            if (playerController != null)
            {
                playerController.score += 10;
            }
            if (RockExplosionClip != null)
            {
                AudioSource.PlayClipAtPoint(RockExplosionClip, Camera.main.transform.position);
            }
            rock.BreakBigRock();
            Destroy(gameObject);
            return;
        }

        // Enemy Ship
        if (hitTarget.TryGetComponent<DroneEnemy>(out DroneEnemy drone))
        {
            if (playerController != null)
            {
                playerController.score += 10;
            }
            if (ShipExplosionClip != null)
            {
                AudioSource.PlayClipAtPoint(ShipExplosionClip, Camera.main.transform.position);
            }
            playerController.score += 10;
            drone.Die();
            Destroy(gameObject);
            return;
        }

        // Player (Enemy ship)
        if (hitTarget.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (!HasActiveShield(hitTarget) && playerController != null)
            {
                player.Die();
            }
            if (ShipExplosionClip != null)
            {
                AudioSource.PlayClipAtPoint(ShipExplosionClip, Camera.main.transform.position);
            }
            Destroy(gameObject);
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
