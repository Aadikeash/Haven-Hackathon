
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;

    public float spawnInterval = 3f;
    public float spawnRadius = 8f;
    public float minSpawnDistance = 3f;
    public float raycastHeight = 15f;

    public LayerMask groundLayer;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player != null && enemyPrefab != null)
            {
                TrySpawnEnemy();
            }
        }
    }

    void TrySpawnEnemy()
    {
        for (int i = 0; i < 20; i++)
        {
            float direction = Random.value < 0.5f ? -1f : 1f;

            float distance = Random.Range(minSpawnDistance, spawnRadius);

            float spawnX = player.position.x + direction * distance;

            Vector2 rayOrigin = new Vector2(
                spawnX,
                player.position.y + raycastHeight
            );

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                Vector2.down,
                raycastHeight * 2f,
                groundLayer
            );

            if (hit.collider != null && !hit.collider.isTrigger)
            {
                Vector3 spawnPosition = new Vector3(
                    spawnX,
                    hit.point.y,
                    0f
                );

                GameObject enemy = Instantiate(
                    enemyPrefab,
                    spawnPosition,
                    Quaternion.identity
                );

                Collider2D enemyCollider = enemy.GetComponent<Collider2D>();

                if (enemyCollider != null)
                {
                    float difference = hit.point.y - enemyCollider.bounds.min.y;
                    enemy.transform.position += Vector3.up * (difference + 0.02f);
                }

                return;
            }
        }
    }
}
