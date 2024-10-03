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
    public float damage = 10f; // Da�o que inflige el enemigo
    public Slider healthBar; // barra de vida
    public Transform objetivo; 

    private Vector3Int currentTile;
    private Queue<Vector3Int> tileQueue = new Queue<Vector3Int>();
    private HashSet<Vector3Int> visitedTiles = new HashSet<Vector3Int>();

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
        UpdateHealthBar();
        StartCoroutine(MoveToNextTile());
    }

    IEnumerator MoveToNextTile()
    {
        while (tileQueue.Count > 0)
        {
            Vector3Int targetTile = tileQueue.Dequeue();
            Vector3 targetPosition = tilemap.GetCellCenterWorld(targetTile);

            Debug.Log($"Dirigi�ndose a: Tilemap Coordenadas: {targetTile}, World Coordenadas: {targetPosition}");

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // Comprobar si est� cerca del centro de la tile y mostrar las coordenadas
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    Debug.Log($"Llegado a Tile: Tilemap Coordenadas: {targetTile}, World Coordenadas: {targetPosition}");

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
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
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
}