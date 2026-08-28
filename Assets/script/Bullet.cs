using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
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
            if (ShipExplosionClip != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(ShipExplosionClip, Camera.main.transform.position);
            }
            drone.Die();
            gameObject.SetActive(false);
            return;
        }

        // Player (Enemy ship)
        if (hitTarget.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (!HasActiveShield(hitTarget) && playerController != null)
            {
                player.Die();
            }
            if (ShipExplosionClip != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(ShipExplosionClip, Camera.main.transform.position);
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
