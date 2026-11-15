
using UnityEngine;

public class EnemySlime : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float detectionRange = 10f;
    public float knockbackForce = 7f;
    public int hitsToKill = 3;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private int hitsTaken;
    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);

            rb.linearVelocity = new Vector2(
                direction * moveSpeed,
                rb.linearVelocity.y
            );

            if (sr != null && direction != 0)
            {
                sr.flipX = direction > 0;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    public void TakePunch(Vector2 attackerPosition)
    {
        if (isDead) return;

        hitsTaken++;

        float direction = Mathf.Sign(transform.position.x - attackerPosition.x);

        if (direction == 0)
        {
            direction = 1f;
        }

        rb.linearVelocity = new Vector2(
            direction * knockbackForce,
            3f
        );

        if (hitsTaken >= hitsToKill)
        {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(1);
            }

            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
