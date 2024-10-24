using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Enemigo : MonoBehaviour
{
    public Tilemap tilemap;
    public float speed = 2f; // Velocidad de movimiento
    public float health = 100f; // Vida del enemigo
    public float maxHealth = 100f; // Vida máxima del enemigo
    public float damage = 10f; // Daño que inflige el enemigo

    [Header("Recompensa")]
    public float reward = 0.2f; // Valor en monedas al destruir al enemigo (ahora es decimal)

    [Header("Partículas")]
    public GameObject hitParticlesPrefab; // Prefab de partículas al ser herido
    public GameObject deathParticlesPrefab; // Prefab de partículas al morir

    [Header("Sonido")]
    public AudioClip deathSound; // Sonido de muerte
    public float deathSoundVolume = 1.0f; // Volumen del sonido de muerte (1 = volumen normal)
    private AudioSource audioSource; // Componente de audio

    public Slider healthBar; // Barra de vida
    public Transform objetivo;

    private Vector3Int currentTile;
    private Queue<Vector3Int> tileQueue = new Queue<Vector3Int>();
    private HashSet<Vector3Int> visitedTiles = new HashSet<Vector3Int>();
    private Vector3 lastPosition; // Para almacenar la última posición
    private Vector3[] directions = new Vector3[]
    {
        new Vector3(1f, 0f, 0f),   // Derecha
        new Vector3(0.5f, -0.51f, 0f),  // Derecha-Abajo
        new Vector3(-0.5f, -0.51f, 0f),    // Izquierda-Abajo
        new Vector3(-1f, 0f, 0f), // Izquierda
        new Vector3(-0.5f, 0.51f, 0f),  // Izquierda-Arriba
        new Vector3(0.5f, 0.51f, 0f)      // Derecha-Arriba
    };

    void Start()
    {
        currentTile = tilemap.WorldToCell(transform.position);
        tileQueue.Enqueue(currentTile);
        visitedTiles.Add(currentTile);
        healthBar.maxValue = maxHealth; // Establecer el máximo
        healthBar.value = health; // Establecer el valor inicial
        UpdateHealthBar();
        lastPosition = transform.position; // Inicializar la última posición
        audioSource = GetComponent<AudioSource>(); // Obtener el componente de audio
        StartCoroutine(MoveToNextTile());
    }

    IEnumerator MoveToNextTile()
    {
        while (tileQueue.Count > 0)
        {
            Vector3Int targetTile = tileQueue.Dequeue();
            Vector3 targetPosition = tilemap.GetCellCenterWorld(targetTile);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f) // Aumentamos el margen de distancia
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // Determinar la dirección de movimiento y aplicar flip si es necesario
                Vector3 direction = transform.position - lastPosition;
                if (direction.x < 0) // Se está moviendo hacia la izquierda
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (direction.x > 0) // Se está moviendo hacia la derecha
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }

                // Actualizar la última posición
                lastPosition = transform.position;

                yield return null;
            }

            // Añadir los vecinos no visitados al queue
            foreach (Vector3 direction in directions)
            {
                Vector3Int neighborTile = tilemap.WorldToCell(targetPosition + direction);

                if (tilemap.HasTile(neighborTile) && !visitedTiles.Contains(neighborTile))
                {
                    tileQueue.Enqueue(neighborTile);
                    visitedTiles.Add(neighborTile);
                }
            }
        }
    }

    // Este método se llamará automáticamente cuando el enemigo colisione con un trigger.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Objetivo"))
        {
            Objetivo objetivoScript = other.GetComponent<Objetivo>();
            if (objetivoScript != null)
            {
                objetivoScript.TakeDamage(damage); // Inflige daño al objetivo
            }
            Die(); // Destruye el enemigo
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Salud antes: {health}");
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth); // Asegurarse de que la salud no baje de 0
        Debug.Log($"{gameObject.name} Salud después: {health}");
        UpdateHealthBar();

        // Instanciar partículas de daño si están asignadas
        if (hitParticlesPrefab != null)
        {
            Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        }

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
            healthBar.value = health; // Actualizar el valor de la barra de salud
        }
    }

    void Die()
    {
        // Instanciar partículas de muerte si están asignadas
        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }

        // Crear un objeto temporal para reproducir el sonido de muerte
        if (deathSound != null)
        {
            GameObject tempAudioSource = new GameObject("DeathSound");
            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
            audioSource.clip = deathSound;

            // Aplicar el volumen desde la variable pública
            audioSource.volume = deathSoundVolume;
            audioSource.Play();

            // Destruir el objeto temporal después de que termine el sonido
            Destroy(tempAudioSource, deathSound.length);
        }

        // Añadir directamente las monedas con el valor de la recompensa
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(reward);
        }
        else
        {
            Debug.LogError("GameManager instance is null. Coins cannot be added.");
        }

        // Destruir el enemigo inmediatamente
        Destroy(gameObject);
    }
}
