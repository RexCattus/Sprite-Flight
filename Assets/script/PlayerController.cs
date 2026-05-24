using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;
    public float maxSpeed = 5f;
    Rigidbody2D rb;
    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMutiplier = 10f;
    public GameObject flameEffect;
    public GameObject ExhaustEffect;
    public UIDocument UIdoc; // Tham chiếu đến UI Document chứa Text để hiển thị điểm số
    private Label scoreText;
    private Button Restart;
    public GameObject explosionEffect; // Prefab hiệu ứng nổ
    public GameManager gameManager; // Tham chiếu đến GameManager để cập nhật điểm số
    public AudioSource EngineSound;

    [Header("Cai đặt Giới Hạn Di Chuyển")]
    private float minX = -12f;
    private float maxX = 14f;
    private float minY = -7.5f;
    private float maxY = 7.5f;

    [Header("Cài đặt Nhiên Liệu")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    private VisualElement fuelFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = UIdoc.rootVisualElement.Q<Label>("ScoreLabel"); // Tìm Label trong UI Document
        Restart = UIdoc.rootVisualElement.Q<Button>("Restart_Button"); // Tìm Button trong UI Document
        Restart.style.display = DisplayStyle.None; // Ẩn nút Restart
        Restart.clicked += RestartGame; // Gán sự kiện click cho nút Restart
        fuelFill = UIdoc.rootVisualElement.Q<VisualElement>("Fuel_Fill");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
    }

    void FixedUpdate()
    {
        MovePlayer();
        gioihan();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            Instantiate(explosionEffect, transform.position, transform.rotation); // Hiệu ứng nổ khi va chạm với Enemy
            Restart.style.display = DisplayStyle.Flex; // Hiển thị nút Restart
            gameManager.end_work(score); // Gọi phương thức kết thúc trò chơi và truyền điểm số hiện tại
        }
    }
    private void UpdateScore()
    {
        elapsedTime += Time.deltaTime;
        score = Mathf.FloorToInt(elapsedTime * scoreMutiplier);
        scoreText.text = "Score: " + score; //Cập nhật điểm số trên UI
    }
    private void MovePlayer()
    {
        // 1. NẾU ĐANG GIỮ CHUỘT
        if (Mouse.current.leftButton.isPressed && currentFuel > 0)
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            mousepos.z = transform.position.z; // Giữ trên mặt phẳng 2D
            Vector2 direction = mousepos - transform.position;
            float distance = direction.magnitude;

            // Chỉ di chuyển và quay nếu chuột không nằm quá sát phi thuyền
            if (distance > 0.4f)
            {
                // Xoay mượt mà phi thuyền về hướng chuột bằng Slerp
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);

                rb.AddForce(direction.normalized * speed);

                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }

                flameEffect.SetActive(true);
                ExhaustEffect.SetActive(true);

                // Bật âm thanh nếu nó chưa chạy
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
                // Giảm tốc mượt mà khi ở sát vị trí chuột để điều khiển chính xác
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 8f);
                flameEffect.SetActive(false);
                ExhaustEffect.SetActive(false);

                if (EngineSound.isPlaying == true)
                {
                    EngineSound.Stop();
                }
            }
        }
        // 2. NẾU KHÔNG GIỮ CHUỘT (hoặc vừa nhả ra)
        else
        {
            // Hãm phanh mượt mà trong không gian
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 2f);

            flameEffect.SetActive(false);
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
    private void gioihan()
    {
        Vector2 vitrihientai = rb.position;
        float clampedX = Mathf.Clamp(vitrihientai.x, minX, maxX);
        float clampedY = Mathf.Clamp(vitrihientai.y, minY, maxY);

        if (vitrihientai.x != clampedX || vitrihientai.y != clampedY)
        {
            rb.position = new Vector2(clampedX, clampedY);

            // Triệt tiêu vận tốc theo chiều chạm biên để tránh rung lắc
            Vector2 vel = rb.linearVelocity;
            if (vitrihientai.x != clampedX) vel.x = 0f;
            if (vitrihientai.y != clampedY) vel.y = 0f;
            rb.linearVelocity = vel;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Fuel"))
        {
            currentFuel = Mathf.Min(currentFuel + 25f, maxFuel);
            UpdateFuelUI();
            Destroy(other.gameObject);
        }
    }

    private void UpdateFuelUI()
    {
        float FuelPercentage = (currentFuel / maxFuel) * 100f;
        if (fuelFill != null)
        {
            fuelFill.style.height = new Length(FuelPercentage, LengthUnit.Percent);
        }
    }
}