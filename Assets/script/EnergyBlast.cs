using UnityEngine;
using System.Collections;

public class EnergyBlast : MonoBehaviour
{

    [Header("Setup Skill")]
    [SerializeField] private float pushForce = 5f;

    private CircleCollider2D circleCollider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 PushDirection = (other.transform.position - transform.position).normalized;
            Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
            enemyRb.AddForce(PushDirection * pushForce, ForceMode2D.Impulse);
        }
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }

    public void TurnOff()
    {
    gameObject.SetActive(false);
    }
}
