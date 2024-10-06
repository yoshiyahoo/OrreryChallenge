using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public float health = 100f;
    public float damagePerAsteroid = 10f;
    public GameObject gameOverScreen;
    public TMP_Text healthBar;

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }
    void Start()
    {
        gameOverScreen.SetActive(false);
        healthBar.text = "Earth Health: " + health;

    }
    public void TakeDamage(float amount)
    {
        health -= amount;
        healthBar.text = "Earth Health: " + health;
        Debug.Log("Earth took damage! Current health: " + health);
    }

    public void goHome()
    {
        SceneManager.LoadScene(0);
    }

    void Die()
    {
        gameOverScreen.SetActive(true);
        Debug.Log("Earth has died!");
        // Add additional logic for what happens when the Earth dies (e.g., game over)
    }
}
