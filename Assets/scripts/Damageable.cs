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
        Debug.Log($"{gameObject.name} recibi� {amount} de da�o.");
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
        // Aqu� puedes a�adir efectos de muerte, animaciones, etc.
        Destroy(gameObject);
    }
}
