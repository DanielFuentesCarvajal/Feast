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
    public float maxHealth = 100f; // Vida m�xima del enemigo
    public float damage = 10f; // Da�o que inflige el enemigo
    [Header("Recompensa")]
    public int reward = 10; // Valor en monedas al destruir al enemigo
    public Slider healthBar; // Barra de vida
    public Transform objetivo;

    private Vector3Int currentTile;
    private Queue<Vector3Int> tileQueue = new Queue<Vector3Int>();
    private HashSet<Vector3Int> visitedTiles = new HashSet<Vector3Int>();
    private Vector3 lastPosition; // Para almacenar la �ltima posici�n
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
        healthBar.maxValue = maxHealth; // Establecer el m�ximo
        healthBar.value = health; // Establecer el valor inicial
        UpdateHealthBar();
        lastPosition = transform.position; // Inicializar la �ltima posici�n
        StartCoroutine(MoveToNextTile());
    }


    IEnumerator MoveToNextTile()
    {
        while (tileQueue.Count > 0)
        {
            Vector3Int targetTile = tileQueue.Dequeue();
            Vector3 targetPosition = tilemap.GetCellCenterWorld(targetTile);

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // Determinar la direcci�n de movimiento y aplicar flip si es necesario
                Vector3 direction = transform.position - lastPosition;
                if (direction.x < 0) // Se est� moviendo hacia la izquierda
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (direction.x > 0) // Se est� moviendo hacia la derecha
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }

                // Actualizar la �ltima posici�n
                lastPosition = transform.position;

                // Comprobar si est� cerca del centro de la tile y mostrar las coordenadas
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    Collider2D collider = Physics2D.OverlapPoint(targetPosition);
                    if (collider != null && collider.CompareTag("Objetivo"))
                    {
                        Objetivo objetivoScript = collider.GetComponent<Objetivo>();
                        if (objetivoScript != null)
                        {
                            objetivoScript.TakeDamage(damage);
                        }
                        Die();
                    }
                }

                yield return null;
            }

            // A�adir los vecinos no visitados al queue
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

    public void TakeDamage(float amount)
    {
        Debug.Log($"{gameObject.name} recibi� {amount} de da�o. Salud antes: {health}");
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth); // Asegurarse de que la salud no baje de 0
        Debug.Log($"{gameObject.name} Salud despu�s: {health}");
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
            healthBar.value = health; // Actualizar el valor de la barra de salud
        }
    }
    void Die()
    {
        GameManager.Instance.AddCoins(reward);
        Destroy(gameObject);
    }
}
