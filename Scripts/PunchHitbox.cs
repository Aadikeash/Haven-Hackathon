
using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        player = transform.root;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemySlime enemy = other.GetComponent<EnemySlime>();

        if (enemy != null)
        {
            enemy.TakePunch(player.position);
        }
    }
}
