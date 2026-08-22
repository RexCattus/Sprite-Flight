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
    public BaseSkill primarySkill;

    [Header("References")]
    public ParticleSystem[] flameEffects;
    public GameObject ExhaustEffect;
    public GameObject explosionEffect;
    public GameManager gameManager; // GameManager để cập nhật điểm
    public UIDocument UIdoc; // UI Document hiển thị điểm
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

        // auto tìm UIDoc và GameManager khi vào scene,tránh null reference
        if (UIdoc == null) UIdoc = FindFirstObjectByType<UIDocument>();
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();

        if (UIdoc != null && UIdoc.rootVisualElement != null)
        {
            scoreText = UIdoc.rootVisualElement.Q<Label>("ScoreLabel"); 
            Restart = UIdoc.rootVisualElement.Q<Button>("Restart"); 
            if (Restart != null)
            {
                Restart.style.display = DisplayStyle.None; 
                Restart.clicked += RestartGame; 
            }
            fuelFill = UIdoc.rootVisualElement.Q<VisualElement>("Fuel_Fill");
        }
        else
        {
            Debug.LogError("Không tìm thấy UI Document trong Scene");
        }

        // vòng lặp chạy full flameEffect khi start do có nhiều particle system
        foreach (var p in flameEffects) { if (p != null) p.Play(); }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ShootAmmo();
        }
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (primarySkill != null)
            {
                primarySkill.UseSkill();
            }
        }
    }

    void LateUpdate()
    {
        // Cố định lại vận tốc tối đa một lần nữa ngay sau khi hệ thống Vật lý tính xong.
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
    
    public void Die()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Restart.style.display = DisplayStyle.Flex;
        gameManager.end_work(score);
        Destroy(gameObject);
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
        scoreText.text = "Score: " + Mathf.FloorToInt(score); //Cập nhật điểm trên UI
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
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float currentAngle = rb.rotation;
                float newAngle = Mathf.LerpAngle(currentAngle, angle, Time.deltaTime * 15f);
                rb.MoveRotation(newAngle);

                rb.AddForce(direction.normalized * speed);

                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }

                foreach (var p in flameEffects)
                {
                    if (p != null) { var em = p.emission; em.enabled = true; }
                }
                if (ExhaustEffect != null) ExhaustEffect.SetActive(true);

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
                foreach (var p in flameEffects)
                {
                    if (p != null) { var em = p.emission; em.enabled = false; }
                }
                if (ExhaustEffect != null) ExhaustEffect.SetActive(false);

                if (EngineSound.isPlaying == true)
                {
                    EngineSound.Stop();
                }
            }
        }
        // nếu ko giữ chuột
        else
        {
            // Phanh lại
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 2f);

            foreach (var p in flameEffects)
            {
                if (p != null) { var em = p.emission; em.enabled = false; }
            }
            if (ExhaustEffect != null) ExhaustEffect.SetActive(false);

            // Tắt âm thanh
            if (EngineSound.isPlaying == true)
            {
                EngineSound.Stop();
            }
        }
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Tải lại scene hiện tại để restart
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
            // Dùng ShootLocation.rotation thay vì transform.rotation để dễ chỉnh hướng bắn
            Instantiate(Ammo, ShootLocation.position, ShootLocation.rotation);
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