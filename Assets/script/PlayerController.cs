using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreText = UIdoc.rootVisualElement.Q<Label>("ScoreLabel"); // Tìm Label trong UI Document
        Restart = UIdoc.rootVisualElement.Q<Button>("Restart_Button"); // Tìm Button trong UI Document
        Restart.style.display = DisplayStyle.None; // Ẩn nút Restart
        Restart.clicked += RestartGame; // Gán sự kiện click cho nút Restart
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        UpdateScore();
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
    if (Mouse.current.leftButton.isPressed)
    {
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        Vector2 direction = mousepos - transform.position; 
        transform.up = direction; 
        rb.AddForce(direction.normalized * speed); 

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed; 
        }

        flameEffect.SetActive(true);
        ExhaustEffect.SetActive(true);  

        // Bật âm thanh nếu nó chưa chạy
        if (EngineSound.isPlaying == false) { 
            EngineSound.Play(); 
        }
    }
    // 2. NẾU KHÔNG GIỮ CHUỘT (hoặc vừa nhả ra)
    else 
    { 
        flameEffect.SetActive(false); 
        ExhaustEffect.SetActive(false); 

        // Tắt âm thanh
        if (EngineSound.isPlaying == true) {
            EngineSound.Stop();
        }
    }
}
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Tải lại scene hiện tại để bắt đầu lại trò chơi
    }
}