using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput.x = Keyboard.current.dKey.isPressed ? 1 :
                      Keyboard.current.aKey.isPressed ? -1 : 0;
        moveInput.y = Keyboard.current.wKey.isPressed ? 1 :
                      Keyboard.current.sKey.isPressed ? -1 : 0;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * speed;
    }
}