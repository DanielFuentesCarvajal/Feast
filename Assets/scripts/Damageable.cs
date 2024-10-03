using UnityEngine;
using UnityEngine.UI;
public interface IDamageable
{
    void TakeDamage(float amount);
}

public class Damageable : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public Slider healthBar; // Asigna esto en el Inspector si usas una barra de salud

    public void TakeDamage(float amount)
    {
        Debug.Log($"{gameObject.name} recibió {amount} de daño.");
        health -= amount;
        UpdateHealthBar();

        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} ha muerto.");
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }

    void Die()
    {
        // Aquí puedes añadir efectos de muerte, animaciones, etc.
        Destroy(gameObject);
    }
}
