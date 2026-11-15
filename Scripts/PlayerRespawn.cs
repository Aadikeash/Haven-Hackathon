
using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;
    public float fallY = -15f;
    public float respawnDelay = 1f;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private bool isRespawning;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!isRespawning && transform.position.y < fallY)
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        if (isRespawning) return;

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnDelay);

        rb.linearVelocity = Vector2.zero;
        transform.position = respawnPoint.position;

        if (health != null)
            health.ResetHealth();

        isRespawning = false;
    }
}
