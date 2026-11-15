
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public GameObject winScreen;
    public bool freezeGame = true;

    private bool hasWon;

    void Start()
    {
        if (winScreen != null)
            winScreen.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasWon || !other.CompareTag("Player"))
            return;

        hasWon = true;

        if (winScreen != null)
            winScreen.SetActive(true);

        if (freezeGame)
            Time.timeScale = 0f;
    }
}
