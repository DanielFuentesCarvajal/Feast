using UnityEngine;
using UnityEngine.UI;
using TMPro; // Importar TextMeshPro
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; // Para cambiar de escenas

public class GameManager : MonoBehaviour
{
    // *** Singleton ***
    public static GameManager Instance { get; private set; }

    // *** Monedas ***
    public int startingCoins = 100; // Cantidad inicial de monedas
    private float currentCoins; // Ahora puede almacenar decimales

    [Header("UI")]
    public TextMeshProUGUI coinsText; // Referencia al TextMeshPro en el Canvas para mostrar las monedas

    // *** Spawner de Enemigos ***
    [System.Serializable]
    public class SpawnConfig
    {
        public GameObject prefabEnemigo; // Prefab del enemigo
        public Transform objetivo;       // Objetivo del enemigo
        public int cantidadEnemigos;     // Número de enemigos a spawnear
        public float tiempoDeSpawn;      // Tiempo entre enemigos
        public float tiempoDeEspera;     // Tiempo antes de comenzar a spawnear enemigos
        public List<int> spawnPointsIndices; // Índices de los puntos de spawn a usar
    }

    public List<SpawnConfig> spawnConfigs;

    // Lista dinámica de spawns como coordenadas
    public List<Vector3> spawnPoints; // Lista de coordenadas de puntos de spawn

    void Awake()
    {
        // Implementación del Singleton
        if (Instance == null)
        {
            Instance = this; // Asignar la instancia
        }
        else
        {
            Destroy(gameObject); // Destruir el GameManager existente
            return; // Asegurarse de que no se ejecute el resto de Awake
        }
    }

    void Start()
    {
        // Inicializar monedas
        currentCoins = startingCoins;
        UpdateCoinsUI();

        // Iniciar spawner de enemigos
        foreach (SpawnConfig config in spawnConfigs)
        {
            StartCoroutine(SpawnEnemies(config));
        }
    }

    // *** Gestión de Monedas ***
    public void AddCoins(float amount)
    {
        currentCoins += amount;
        UpdateCoinsUI();
    }

    public bool SubtractCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateCoinsUI();
            return true;
        }
        else
        {
            Debug.Log("No tienes suficientes monedas.");
            return false;
        }
    }

    void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            // Solo mostrar la parte entera de las monedas
            coinsText.text = Mathf.FloorToInt(currentCoins).ToString();
        }
    }

    public int GetCurrentCoins()
    {
        return Mathf.FloorToInt(currentCoins); // Retorna la parte entera
    }

    // *** Spawner de Enemigos ***
    IEnumerator SpawnEnemies(SpawnConfig config)
    {
        yield return new WaitForSeconds(config.tiempoDeEspera);

        float intervalo = config.tiempoDeSpawn / config.cantidadEnemigos;

        for (int i = 0; i < config.cantidadEnemigos; i++)
        {
            // Seleccionar un punto de spawn aleatorio desde los índices especificados en la configuración
            int spawnIndex = config.spawnPointsIndices[Random.Range(0, config.spawnPointsIndices.Count)];
            Vector3 spawnPosition = spawnPoints[spawnIndex];

            InstanciarEnemigo(config.prefabEnemigo, spawnPosition, config.objetivo);
            yield return new WaitForSeconds(intervalo);
        }
    }

    void InstanciarEnemigo(GameObject prefabEnemigo, Vector3 posicion, Transform objetivo)
    {
        GameObject enemigo = Instantiate(prefabEnemigo, posicion, Quaternion.identity);
        enemigo.tag = "Enemigo"; // Asignar el tag a los clones

        Enemigo scriptEnemigo = enemigo.GetComponent<Enemigo>();

        if (scriptEnemigo != null)
        {
            scriptEnemigo.objetivo = objetivo;

            Slider healthBar = enemigo.GetComponentInChildren<Slider>();
            if (healthBar != null)
            {
                scriptEnemigo.healthBar = healthBar;
                scriptEnemigo.healthBar.maxValue = scriptEnemigo.maxHealth;
                scriptEnemigo.healthBar.value = scriptEnemigo.health;
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

    // Método para reiniciar el nivel
    public void RetryLevel()
    {
        StartCoroutine(RetryLevelCoroutine());
    }

    private IEnumerator RetryLevelCoroutine()
    {
        // Destruir todos los enemigos clonados
        GameObject[] enemigosClonados = GameObject.FindGameObjectsWithTag("Enemigo");
        foreach (GameObject enemigo in enemigosClonados)
        {
            Destroy(enemigo);
        }

        // Esperar hasta que todos los enemigos sean destruidos
        while (GameObject.FindGameObjectsWithTag("Enemigo").Length > 0)
        {
            yield return null; // Esperar un frame
        }

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
