
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;

    public float moveSpeed = 6f;
    public float moveForce = 3f;
    public float jumpForce = 10f;
    public float wallJumpForce = 8f;
    public float slidingFriction = 8f;

    private float moveInput;
    private bool facingLeft = false;

    private bool jumpRequested = false;
    private bool wallJumpRequested = false;
    private float wallJumpDirection = 0f;
    private bool isGrounded = false;
    private float coyoteTimer = 0f;

    public GameObject punchObject;
    public float punchSpeed = 2f;
    public float extraCooldown = 0.05f;
    public float attackKnockback = 5f;

    private bool canPunch = true;
    private float baseClipLength = 0.3f;
    private Vector3 punchStartPos;
    private SpriteRenderer punchSr;
    private Animator punchAnim;

    public Sprite wallSprite;
    private List<ContactPoint2D> contacts = new List<ContactPoint2D>();
    private bool isClinging = false;
    private bool clingingLeft = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        if (sr != null)
        {
            facingLeft = sr.flipX;
        }

        if (punchObject != null)
        {
            punchStartPos = punchObject.transform.localPosition;
            punchSr = punchObject.GetComponent<SpriteRenderer>();
            punchAnim = punchObject.GetComponent<Animator>();

            if (punchAnim != null && punchAnim.runtimeAnimatorController != null)
            {
                foreach (AnimationClip clip in punchAnim.runtimeAnimatorController.animationClips)
                {
                    baseClipLength = clip.length;
                    break;
                }
            }

            punchObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        moveInput = 0;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            moveInput -= 1;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            moveInput += 1;
        }

        if (moveInput != 0)
        {
            facingLeft = moveInput < 0;

            if (punchObject != null)
            {
                float dir = facingLeft ? -1f : 1f;
                punchObject.transform.localPosition = new Vector3(
                    Mathf.Abs(punchStartPos.x) * dir,
                    punchStartPos.y,
                    punchStartPos.z
                );

                if (punchSr != null)
                {
                    punchSr.flipX = facingLeft;
                }
            }
        }

        CheckContacts();

        if (isGrounded || isClinging)
        {
            coyoteTimer = 0.12f;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && coyoteTimer > 0f)
        {
            if (isClinging)
            {
                wallJumpRequested = true;
                wallJumpDirection = clingingLeft ? 1f : -1f;
            }
            else
            {
                jumpRequested = true;
            }

            coyoteTimer = 0f;
        }

        bool attackPressed =
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
            Keyboard.current.jKey.wasPressedThisFrame ||
            Keyboard.current.zKey.wasPressedThisFrame;

        if (attackPressed && canPunch)
        {
            StartCoroutine(DoPunch());
        }
    }

    void LateUpdate()
    {
        if (sr == null) return;

        if (isClinging && wallSprite != null)
        {
            sr.sprite = wallSprite;
            sr.flipX = clingingLeft;
        }
        else
        {
            sr.flipX = facingLeft;
        }
    }

    void CheckContacts()
    {
        if (rb == null) return;

        rb.GetContacts(contacts);

        isGrounded = false;
        bool touchingRightWall = false;
        bool touchingLeftWall = false;

        foreach (ContactPoint2D contact in contacts)
        {
            if (contact.collider == null || contact.collider.isTrigger) continue;

            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
            }
            else if (contact.normal.x < -0.5f)
            {
                touchingRightWall = true;
            }
            else if (contact.normal.x > 0.5f)
            {
                touchingLeftWall = true;
            }
        }

        bool clingingRight = !isGrounded && touchingRightWall && moveInput > 0;
        clingingLeft = !isGrounded && touchingLeftWall && moveInput < 0;
        isClinging = clingingRight || clingingLeft;
    }

    IEnumerator DoPunch()
    {
        canPunch = false;

        float knockbackDirection = facingLeft ? 1f : -1f;

        rb.linearVelocity = new Vector2(
            knockbackDirection * attackKnockback,
            rb.linearVelocity.y
        );

        if (punchObject != null)
        {
            punchObject.SetActive(true);

            if (punchAnim != null)
            {
                punchAnim.speed = punchSpeed;
            }
        }

        yield return new WaitForSeconds(baseClipLength / punchSpeed);

        if (punchObject != null)
        {
            punchObject.SetActive(false);
        }

        yield return new WaitForSeconds(extraCooldown);
        canPunch = true;
    }

    void FixedUpdate()
    {
        if (moveInput != 0f)
        {
            rb.AddForce(new Vector2(moveInput * moveForce, 0f), ForceMode2D.Force);
        }
        else
        {
            float newX = Mathf.Lerp(
                rb.linearVelocity.x,
                0f,
                slidingFriction * Time.fixedDeltaTime
            );

            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }

        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
        {
            float clampedX = Mathf.Sign(rb.linearVelocity.x) * moveSpeed;
            rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);
        }

        if (wallJumpRequested)
        {
            rb.linearVelocity = new Vector2(
                wallJumpDirection * wallJumpForce,
                jumpForce
            );

            wallJumpRequested = false;
        }
        else if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpRequested = false;
        }
    }
}
