using UnityEngine;
using UnityEngine.UI;

public class Objetivo : MonoBehaviour
{
    public float health = 100f; // Vida actual del objetivo
    public float maxHealth = 100f; // Vida máxima del objetivo
    public Slider healthBar; // Barra de vida

    public GameObject damageParticlesPrefab; // Prefab de partículas para cuando recibe daño

    // Variables para audio, partículas y volúmenes
    public AudioClip below75Audio; // Audio para cuando la salud baja de 75
    public float below75Volume = 1f; // Modificador de volumen para el audio de 75

    public AudioClip below50Audio; // Audio para cuando la salud baja de 50
    public float below50Volume = 1f; // Modificador de volumen para el audio de 50

    public AudioClip below25Audio; // Audio para cuando la salud baja de 25
    public float below25Volume = 1f; // Modificador de volumen para el audio de 25

    public GameObject below75Particles; // Partículas para cuando la salud baja de 75
    public GameObject below50Particles; // Partículas para cuando la salud baja de 50
    public GameObject below25Particles; // Partículas para cuando la salud baja de 25

    private ButtonSelectionManager buttonSelectionManager; // Referencia al ButtonSelectionManager
    private Animator animator; // Referencia al Animator
    private AudioSource audioSource; // Componente de AudioSource

    private bool hasTriggeredBelow75 = false; // Estado para evitar múltiples disparos de umbral 75
    private bool hasTriggeredBelow50 = false; // Estado para evitar múltiples disparos de umbral 50
    private bool hasTriggeredBelow25 = false; // Estado para evitar múltiples disparos de umbral 25

    void Start()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }

        // Buscar y asignar el ButtonSelectionManager
        buttonSelectionManager = FindObjectOfType<ButtonSelectionManager>();

        // Asignar el Animator del objeto
        animator = GetComponent<Animator>();

        // Obtener el componente de AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        // Inicializar la animación según el valor de vida
        UpdateAnimation();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth); // Asegura que la vida no sea menor que 0

        UpdateHealthBar();

        // Instanciar partículas de daño
        if (damageParticlesPrefab != null)
        {
            Instantiate(damageParticlesPrefab, transform.position, Quaternion.identity);
        }

        // Actualizar la animación según la nueva vida
        UpdateAnimation();

        // Reproducir sonidos y partículas en los umbrales de salud
        CheckHealthThresholds();

        if (health <= 0)
        {
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

    void UpdateAnimation()
    {
        // Asegurarse de que el Animator esté configurado
        if (animator != null)
        {
            // Calcular el porcentaje de vida actual
            float healthPercentage = (health / maxHealth) * 100f;

            // Actualizar el parámetro "vida" en el Animator
            animator.SetFloat("vida", healthPercentage);
        }
    }

    void CheckHealthThresholds()
    {
        // Reproducir audio y partículas al caer por debajo de ciertos niveles de salud
        if (health < 75.1 && !hasTriggeredBelow75)
        {
            PlaySound(below75Audio, below75Volume);
            if (below75Particles != null)
            {
                Instantiate(below75Particles, transform.position, Quaternion.identity);
            }
            hasTriggeredBelow75 = true; // Evitar futuras ejecuciones
        }

        if (health < 50.1 && !hasTriggeredBelow50)
        {
            PlaySound(below50Audio, below50Volume);
            if (below50Particles != null)
            {
                Instantiate(below50Particles, transform.position, Quaternion.identity);
            }
            hasTriggeredBelow50 = true; // Evitar futuras ejecuciones
        }

        if (health < 25.1 && !hasTriggeredBelow25)
        {
            PlaySound(below25Audio, below25Volume);
            if (below25Particles != null)
            {
                Instantiate(below25Particles, transform.position, Quaternion.identity);
            }
            hasTriggeredBelow25 = true; // Evitar futuras ejecuciones
        }
    }

    void PlaySound(AudioClip clip, float volumeMultiplier)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volumeMultiplier);
        }
    }

    void Die()
    {
        // Notificar al ButtonSelectionManager de la derrota
        buttonSelectionManager.CheckObjectiveHealth(health);
    }
}
