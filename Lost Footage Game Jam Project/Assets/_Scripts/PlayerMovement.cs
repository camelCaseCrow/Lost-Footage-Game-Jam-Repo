using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float secondsPerFrame = 0.15f;   // how long each walk frame shows
    [SerializeField] private AK.Wwise.Event footstepsSound;

    #region Direction Sprites

    // Standing still: one sprite per direction (from herafinalneutral).
    [SerializeField] private Sprite upIdle;
    [SerializeField] private Sprite downIdle;
    [SerializeField] private Sprite leftIdle;
    [SerializeField] private Sprite rightIdle;

    // Walking: frames played in order, per direction.
    [SerializeField] private Sprite[] upWalk;
    [SerializeField] private Sprite[] downWalk;
    [SerializeField] private Sprite[] leftWalk;
    [SerializeField] private Sprite[] rightWalk;

    #endregion

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;

    private Sprite currentIdle;     // standing sprite for the way she's facing
    private Sprite[] currentWalk;   // walking frames for the way she's facing
    private int frameIndex;         // which walk frame is showing
    private float frameTimer;       // time since the last frame change
    private bool wasWalking;        // was she walking last frame?

    // Runs before every Start, so cutscenes can call SetDirection from their Start.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (upIdle == null || downIdle == null || leftIdle == null || rightIdle == null)
        {
            throw new System.InvalidOperationException($"PlayerMovement on '{name}': assign all four Idle sprites in the Inspector.");
        }

        if (upWalk.Length == 0 || downWalk.Length == 0 || leftWalk.Length == 0 || rightWalk.Length == 0)
        {
            throw new System.InvalidOperationException($"PlayerMovement on '{name}': add frames to all four Walk lists in the Inspector.");
        }

        // Start facing down, standing still.
        currentIdle = downIdle;
        currentWalk = downWalk;
        spriteRenderer.sprite = currentIdle;
    }

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
        Animate();
        UpdateFootsteps();
    }

    // Starts the looping footsteps when she starts walking, stops them when she stops.
    private void UpdateFootsteps()
    {
        bool isWalking = moveInput != Vector2.zero;

        if (isWalking && !wasWalking)
        {
            footstepsSound.Post(gameObject);
        }
        else if (!isWalking && wasWalking)
        {
            footstepsSound.Stop(gameObject);
        }

        wasWalking = isWalking;
    }

    // Wired to PlayerInput > Events > Player > Move.
    public void Move(InputAction.CallbackContext context)
    {
        SetDirection(context.ReadValue<Vector2>());
    }

    // Turns and moves Hera. Called by Move (keyboard) and by cutscenes. Vector2.zero stops her.
    public void SetDirection(Vector2 direction)
    {
        if (direction.y > 0.01f)
        {
            moveInput = Vector2.up;
            currentIdle = upIdle;
            currentWalk = upWalk;
        }
        else if (direction.y < -0.01f)
        {
            moveInput = Vector2.down;
            currentIdle = downIdle;
            currentWalk = downWalk;
        }
        else if (direction.x > 0.01f)
        {
            moveInput = Vector2.right;
            currentIdle = rightIdle;
            currentWalk = rightWalk;
        }
        else if (direction.x < -0.01f)
        {
            moveInput = Vector2.left;
            currentIdle = leftIdle;
            currentWalk = leftWalk;
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    // Flips through the walk frames while moving. Shows the standing sprite when stopped.
    private void Animate()
    {
        if (moveInput == Vector2.zero)
        {
            spriteRenderer.sprite = currentIdle;
            frameIndex = 0;
            frameTimer = 0f;
            return;
        }

        frameTimer += Time.deltaTime;
        if (frameTimer >= secondsPerFrame)
        {
            frameTimer -= secondsPerFrame;
            frameIndex++;
        }

        // Past the last frame (or the new direction has fewer frames): back to the first.
        if (frameIndex >= currentWalk.Length)
        {
            frameIndex = 0;
        }

        spriteRenderer.sprite = currentWalk[frameIndex];
    }
}
