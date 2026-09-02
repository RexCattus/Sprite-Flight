using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Runtime.CompilerServices;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    public static event Action<float> OnPlayerDeath;
    public static event Action<float> OnPlayerScoreUpdate;
    public static event Action<float, float> OnPlayerFuelUpdate;

    Rigidbody2D rb;
    public float score = 0f;

    [Header("change settings")]
    public float speed = 1f;
    public float maxSpeed = 5f;
    public BaseSkill primarySkill;

    [Header("References")]
    public ParticleSystem[] flameEffects;
    public GameObject ExhaustEffect;
    public GameObject explosionEffect;
    public GameObject Ammo;
    public GameObject Shield;
    private Coroutine shieldCoroutine;
    public Transform ShootLocation;

    [Header("Audio")]
    public AudioSource EngineSound;
    public AudioSource SFXSound;
    public AudioClip ShootSound;
    public AudioClip ShieldSound;
    public AudioClip ShipExplode;

    [Header("Fuel System")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    //private VisualElement fuelFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // tránh bị xoay khi đập vào tường

        // vòng lặp chạy full flameEffect khi start do có nhiều particle system
        foreach (var p in flameEffects) { if (p != null) p.Play(); }
        OnPlayerFuelUpdate?.Invoke(currentFuel, maxFuel);
    }

    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime * 1;
        //UpdateScore();
        OnPlayerScoreUpdate?.Invoke(score);

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
        AudioSource.PlayClipAtPoint(ShipExplode, Camera.main.transform.position);
        OnPlayerDeath?.Invoke(score);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Fuel"))
        {
            currentFuel = Mathf.Min(currentFuel + 35f, maxFuel);
            OnPlayerFuelUpdate?.Invoke(currentFuel, maxFuel);

            other.gameObject.SetActive(false);
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
            other.gameObject.SetActive(false);
        }
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

                currentFuel -= Time.deltaTime * 3f;
                currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

                OnPlayerFuelUpdate?.Invoke(currentFuel, maxFuel);
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

    private void OnEnable()
    {
        Bullet.OnScoreBonus += handleScoreBonus;
    }

    private void OnDisable()
    {
        Bullet.OnScoreBonus -= handleScoreBonus;
    }

    private void handleScoreBonus(float bonus)
    {
        score += bonus;
    }

    private void ShootAmmo()
    {
        if (Ammo != null && currentFuel >= 10f)
        {
            ObjectPooler.Instance.SpawnFromPool("Bullet", ShootLocation.position, ShootLocation.rotation);

            currentFuel -= 3f;

            OnPlayerFuelUpdate?.Invoke(currentFuel, maxFuel);
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