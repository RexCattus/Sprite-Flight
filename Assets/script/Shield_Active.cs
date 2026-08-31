using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [SerializeField] private AudioClip[] ImpactSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            PlayDeflectSound();
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            PlayDeflectSound();
            return;
        }
    }

    private void PlayDeflectSound()
    {
        if (ImpactSound != null)
        {
            int randomIndex = Random.Range(0, ImpactSound.Length);
            AudioClip chosenClip = ImpactSound[randomIndex];
            AudioSource.PlayClipAtPoint(chosenClip, Camera.main.transform.position);
        }
    }
}