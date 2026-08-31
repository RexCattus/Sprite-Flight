using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 originPos;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        originPos = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void Shake(float duration = 0.3f, float magnitude = 0.25f)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = originPos; // trả về vị trí gốc 
        }
        shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Dịch chuyển cam quanh toạ độ random 
            transform.localPosition = new Vector3(originPos.x + offsetX, originPos.y + offsetY, originPos.z);
            elapsed += Time.deltaTime;
            yield return null; // chờ sang frame tiếp theo
        }

        // Hết giờ thì đưa camera về đúng vị trí cũ
        transform.localPosition = originPos;
        shakeCoroutine = null;
    }
}
