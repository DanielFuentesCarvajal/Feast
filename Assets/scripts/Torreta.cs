using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Configuración de la Torreta")]
    public ProjectileConfig projectileConfig; // Configuración del proyectil
    public float range = 5f; // Rango de detección de enemigos
    public float fireRate = 1f; // Tasa de disparo (disparos por segundo)

    [Header("Coste")]
    public int cost = 50; // Coste en monedas para colocar la torreta
    public int precioVenta = 25; // Precio de venta de la torreta
    public int precioMejora = 75; // Precio de mejora de la torreta

    [Header("Mejora")]
    public GameObject prefabTorretaMejorada; // Prefab de la torreta mejorada

    [Header("Efectos de disparo")]
    public GameObject shootingParticlesPrefab; // Prefab del sistema de partículas
    public AudioClip shootingSound; // Sonido de disparo
    public float shootingSoundVolume = 1f; // Volumen del sonido de disparo

    private AudioSource audioSource; // Componente de audio para reproducir el sonido
    private float fireCountdown = 0f;

    void Start()
    {
        // Crear un componente de AudioSource si no está presente
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = shootingSound;
        audioSource.volume = shootingSoundVolume;
    }

    void Update()
    {
        // Reducir el contador de tiempo para el disparo
        if (fireCountdown > 0f)
            fireCountdown -= Time.deltaTime;

        // Buscar el enemigo más cercano dentro del rango
        GameObject target = GetClosestEnemy();

        if (target != null && fireCountdown <= 0f)
        {
            Shoot(target);
            fireCountdown = 1f / fireRate;
        }
    }

    // Método para encontrar el enemigo más cercano dentro del rango
    GameObject GetClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemigo");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPos, enemy.transform.position);
            if (distance < minDistance && distance <= range)
            {
                minDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    // Método para disparar un proyectil hacia el objetivo
    void Shoot(GameObject target)
    {
        // Instanciar el proyectil en la posición de la torreta
        GameObject projectile = Instantiate(projectileConfig.visualPrefab != null ?
            projectileConfig.visualPrefab : ProjectileManager.Instance.defaultProjectilePrefab,
            transform.position, Quaternion.identity);

        // Obtener el script del proyectil y configurar sus propiedades
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(target, projectileConfig);
        }

        // Instanciar las partículas de disparo si están asignadas
        if (shootingParticlesPrefab != null)
        {
            Instantiate(shootingParticlesPrefab, transform.position, Quaternion.identity);
        }

        // Reproducir el sonido de disparo
        PlayShootingSound();
    }

    // Método para reproducir el sonido de disparo
    void PlayShootingSound()
    {
        if (shootingSound != null)
        {
            // Calcular el tiempo entre disparos
            float timeBetweenShots = 1f / fireRate;

            // Obtener la duración del audio
            float audioDuration = shootingSound.length;

            // Verificar si la duración del audio es menor que el tiempo entre disparos
            if (audioDuration < timeBetweenShots)
            {
                // Reproducir el sonido normalmente
                audioSource.pitch = fireRate;
                audioSource.Play();
            }
            else
            {
                // Calcular el pitch necesario para que la duración del audio sea igual al 90% del tiempo entre disparos
                float targetDuration = timeBetweenShots * 0.9f;
                audioSource.pitch = audioDuration / targetDuration;
                audioSource.Play();
            }
        }
    }

    // Método para mejorar la torreta
    public void MejorarTorreta()
    {
        if (prefabTorretaMejorada != null)
        {
            Instantiate(prefabTorretaMejorada, transform.position, Quaternion.identity);
            Destroy(gameObject); // Destruir la torreta actual
        }
    }

    // Método para vender la torreta
    public void VenderTorreta()
    {
        GameManager.Instance.AddCoins(precioVenta); // Añadir monedas al jugador
        Destroy(gameObject); // Destruir la torreta
    }

    // Visualización del rango en el Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
