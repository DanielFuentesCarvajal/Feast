using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Configuración de la Torreta")]
    public ProjectileConfig projectileConfig; // Configuración del proyectil
    public float range = 5f; // Rango de detección de enemigos
    public float fireRate = 1f; // Tasa de disparo (disparos por segundo)
    [Header("Coste")]
    public int cost = 50; // Coste en monedas para colocar la torreta
    private float fireCountdown = 0f;

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
    }

    // Opcional: Visualización del rango en el Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
