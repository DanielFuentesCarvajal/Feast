using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnConfig
    {
        public GameObject prefabEnemigo; // Prefab del enemigo
        public Transform objetivo;       // Objetivo del enemigo
        public Vector3 esquina;          // punto de spawn
        public int cantidadEnemigos;     // Número de enemigos a spawnear
        public float tiempoDeSpawn;      // Tiempo entre enemigos
        public float tiempoDeEspera;     // Tiempo antes de comenzar a spawnear enemigos
    }

    public List<SpawnConfig> spawnConfigs;

    void Start()
    {
        foreach (SpawnConfig config in spawnConfigs)
        {
            StartCoroutine(SpawnEnemies(config));
        }
    }

    IEnumerator SpawnEnemies(SpawnConfig config)
    {
        // Espera el tiempo de espera especificado antes de comenzar a spawnear
        yield return new WaitForSeconds(config.tiempoDeEspera);

        float intervalo = config.tiempoDeSpawn / config.cantidadEnemigos;

        for (int i = 0; i < config.cantidadEnemigos; i++)
        {
            InstanciarEnemigo(config.prefabEnemigo, config.esquina, config.objetivo);
            yield return new WaitForSeconds(intervalo);
        }
    }

    void InstanciarEnemigo(GameObject prefabEnemigo, Vector3 posicion, Transform objetivo)
    {
        GameObject enemigo = Instantiate(prefabEnemigo, posicion, Quaternion.identity);
        Enemigo scriptEnemigo = enemigo.GetComponent<Enemigo>();

        if (scriptEnemigo != null)
        {
            scriptEnemigo.objetivo = objetivo;

            // Busca la barra de salud dentro del prefab
            Slider healthBar = enemigo.GetComponentInChildren<Slider>();
            if (healthBar != null)
            {
                // Asignar la barra de salud al enemigo
                scriptEnemigo.healthBar = healthBar;
                // Configurar la barra de salud
                scriptEnemigo.healthBar.maxValue = scriptEnemigo.maxHealth; // Establecer el máximo
                scriptEnemigo.healthBar.value = scriptEnemigo.health; // Establecer el valor inicial
            }
            else
            {
                Debug.LogError("No se encontró la barra de salud en el prefab instanciado.");
            }
        }
        else
        {
            Debug.LogError("No se encontró el componente Enemigo en el prefab instanciado.");
        }
    }
}
