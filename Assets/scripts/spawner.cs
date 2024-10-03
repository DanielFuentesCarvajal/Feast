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
        public int cantidadEnemigos;    
        public float tiempoDeSpawn;      
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

            if (scriptEnemigo.healthBar != null)
            {
                GameObject barraDeVida = Instantiate(scriptEnemigo.healthBar.gameObject, enemigo.transform);
                scriptEnemigo.healthBar = barraDeVida.GetComponentInChildren<Slider>();
            }
        }
        else
        {
            Debug.LogError("No se encontró el componente Enemigo en el prefab instanciado.");
        }
    }
}
