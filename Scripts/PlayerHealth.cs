
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth = 5;
    public Text heartText;

    private PlayerRespawn respawn;
    private bool isDead;

    void Awake()
    {
        respawn = GetComponent<PlayerRespawn>();
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHearts();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;

        if (respawn != null)
            respawn.RespawnPlayer();
        else
            ResetHealth();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        UpdateHearts();
    }

    void UpdateHearts()
    {
        if (heartText != null)
        {
            heartText.text =
                new string('♥', currentHealth) +
                new string('♡', maxHealth - currentHealth);
        }
    }
}
