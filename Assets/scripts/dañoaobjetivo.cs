using UnityEngine;
using UnityEngine.UI;

public class Objetivo : MonoBehaviour
{
    public float health = 100f; // Vida actual del objetivo
    public float maxHealth = 100f; // Vida máxima del objetivo
    public Slider healthBar; //barra de vida

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; 
            healthBar.value = health; 
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth); // Asegura que la vida no sea menor que 0

        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = health; 
        }
    }
}
