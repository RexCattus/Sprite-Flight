using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Runtime.CompilerServices;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float score = 0f;

    [Header("change settings")]
    public float speed = 1f;
    public float maxSpeed = 5f;
    public float scoreMutiplier = 1f;

    [Header("References")]
    public ParticleSystem flameEffect;
    public GameObject ExhaustEffect;
    public GameObject explosionEffect; // Prefab hiệu ứng nổ
    public GameManager gameManager; // Tham chiếu đến GameManager để cập nhật điểm số
    public UIDocument UIdoc; // Tham chiếu đến UI Document chứa Text để hiển thị điểm số
    private Label scoreText;
    private Button Restart;
    public GameObject Ammo;
    public GameObject Shield;
    private Coroutine shieldCoroutine;
    public Transform ShootLocation;

    [Header("Audio")]
    public AudioSource EngineSound;
    public AudioSource SFXSound;
    public AudioClip ShootSound;
    public AudioClip ShieldSound;

    [Header("Fuel System")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    private VisualElement fuelFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // tránh bị xoay khi đập vào tường
        scoreText = UIdoc.rootVisualElement.Q<Label>("ScoreLabel"); // Tìm Label trong UI Document
        Restart = UIdoc.rootVisualElement.Q<Button>("Restart"); // Tìm Button trong UI Document
        Restart.style.display = DisplayStyle.None; // Ẩn nút Restart
        Restart.clicked += RestartGame; // Gán sự kiện click cho nút Restart
        fuelFill = UIdoc.rootVisualElement.Q<VisualElement>("Fuel_Fill");

        // Đảm bảo Particle System luôn chạy ngầm
        if (flameEffect != null) flameEffect.Play();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ShootAmmo();
        }
    }

    void LateUpdate()
    {
        // Cố định lại vận tốc tối đa một lần nữa ngay sau khi hệ thống Vật lý tính toán xong.
        // Giúp triệt tiêu các lực đẩy sinh ra do lỗi kẹt Collider.
        if (rb != null && rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (Shield.activeSelf)
            {
                if (other.gameObject.CompareTag("Enemy"))
                {
                    Destroy(other.gameObject);
                    other.gameObject.GetComponent<Obstacle>().BreakBigRock();
                }
            }
            else
            {
                Destroy(gameObject);
                Instantiate(explosionEffect, transform.position, transform.rotation);
                Restart.style.display = DisplayStyle.Flex;
                gameManager.end_work(score);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Fuel"))
        {
            currentFuel = Mathf.Min(currentFuel + 35f, maxFuel);
            UpdateFuelUI();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Shield"))
        {
            // Bật khiên trong 10 giây
            if (shieldCoroutine != null)
            {
                StopCoroutine(shieldCoroutine);
            }
            shieldCoroutine = StartCoroutine(ActivateShield(10f));
            SFXSound.PlayOneShot(ShieldSound);
            Destroy(other.gameObject);
        }
    }

    private void UpdateScore()
    {
        score += Time.deltaTime * scoreMutiplier;
        scoreText.text = "Score: " + Mathf.FloorToInt(score); //Cập nhật điểm số trên UI
    }
    private void MovePlayer()
    {
        // Nếu đang giữ chuột
        if (Mouse.current.leftButton.isPressed && currentFuel > 0)
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            mousepos.z = transform.position.z; // Giữ trên mặt phẳng 2D
            Vector2 direction = mousepos - transform.position;
            float distance = direction.magnitude;

            // Chỉ di chuyển và quay nếu chuột không nằm quá sát phi thuyền
            if (distance > 0.4f)
            {
                // Xoay phi thuyền về hướng chuột bằng Slerp
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                float currentAngle = rb.rotation;
                float newAngle = Mathf.LerpAngle(currentAngle, angle, Time.deltaTime * 15f);
                rb.MoveRotation(newAngle);

                rb.AddForce(direction.normalized * speed);

                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }

                if (flameEffect != null)
                {
                    var em = flameEffect.emission;
                    em.enabled = true; // Bật van xả khói
                }
                ExhaustEffect.SetActive(true);

                if (EngineSound.isPlaying == false)
                {
                    EngineSound.Play();
                }

                currentFuel -= Time.deltaTime * 10f;
                currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
                UpdateFuelUI();
            }
            else
            {
                // Giảm tốc khi ở sát vị trí chuột
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 8f);
                if (flameEffect != null)
                {
                    var em = flameEffect.emission;
                    em.enabled = false; // Tắt van xả khói
                }
                ExhaustEffect.SetActive(false);

                if (EngineSound.isPlaying == true)
                {
                    EngineSound.Stop();
                }
            }
        }
        // nếu ko giữ chuột
        else
        {
            // Hãm phanh mượt mà trong không gian
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 2f);

            if (flameEffect != null)
            {
                var em = flameEffect.emission;
                em.enabled = false; // Tắt van xả khói
            }
            ExhaustEffect.SetActive(false);

            // Tắt âm thanh
            if (EngineSound.isPlaying == true)
            {
                EngineSound.Stop();
            }
        }
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Tải lại scene hiện tại để bắt đầu lại trò chơi
    }

    private void UpdateFuelUI()
    {
        float FuelPercentage = (currentFuel / maxFuel) * 100f;
        if (fuelFill != null)
        {
            fuelFill.style.height = new Length(FuelPercentage, LengthUnit.Percent);
        }
    }
    private void ShootAmmo()
    {
        if (Ammo != null && currentFuel >= 10f)
        {
            Instantiate(Ammo, ShootLocation.position, transform.rotation);
            currentFuel -= 10f;
            UpdateFuelUI();
            SFXSound.PlayOneShot(ShootSound);
        }
    }
    private IEnumerator ActivateShield(float duration)
    {
        Shield.SetActive(true);
        yield return new WaitForSeconds(duration);
        Shield.SetActive(false);
    }
}