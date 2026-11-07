using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    #region Direction Sprites

    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite downSprite;

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (upSprite == null || rightSprite == null || leftSprite == null ||downSprite == null)
            throw new System.InvalidOperationException($"PlayerMovement on '{name}': assign all four sprites in the Inspector.");
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;

    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input.y > 0.01f)
        {
            moveInput = new Vector2(0, 1);
            spriteRenderer.sprite = upSprite;
        }
        else if (input.y < -0.01f)
        {
            moveInput = new Vector2(0, -1);
            spriteRenderer.sprite = downSprite;
        }
        else if (input.x > 0.01f)
        {
            moveInput = new Vector2(1, 0);
            spriteRenderer.sprite = rightSprite;
        }
        else if (input.x < -0.01f)
        {
            moveInput = new Vector2(-1, 0); 
            spriteRenderer.sprite = leftSprite;
        }
        else
            moveInput = Vector2.zero;
    }
}
