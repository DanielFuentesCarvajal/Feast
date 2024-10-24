using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target; // Objetivo actual del proyectil
    private float speed;
    private float damage;
    private float areaDeRebote; // Área para rebote del proyectil
    private float areaDeEfecto; // Área para daño en área
    private int remainingBounces; // Contador de rebotes restantes

    public ParticleSystem impactParticles; // Partículas al impactar
    public ParticleSystem shootParticles; // Partículas al disparar

    private List<Transform> hitTargets = new List<Transform>(); // Lista de enemigos golpeados

    // Método para inicializar el proyectil
    public void Initialize(GameObject targetEnemy, ProjectileConfig config)
    {
        if (targetEnemy == null || config == null)
        {
            Debug.LogError("Initialize llamado con targetEnemy o config nulo.");
            return;
        }

        target = targetEnemy.transform;
        speed = config.speed;
        damage = config.damage;
        areaDeRebote = config.areaDeRebote; // Usar el área de rebote del ScriptableObject
        areaDeEfecto = config.areaDeEfecto; // Usar el área de efecto del ScriptableObject
        remainingBounces = config.maxBounces; // Usar el valor de rebotes del ScriptableObject

        Debug.Log($"Proyectil inicializado con objetivo: {target.name}, Daño: {damage}, Velocidad: {speed}, Rebotes: {remainingBounces}");

        impactParticles = config.impactParticles;
        shootParticles = config.shootParticles;

        // Activar partículas de disparo si están asignadas
        if (shootParticles != null)
        {
            ParticleSystem particles = Instantiate(shootParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }

        // Orientar el proyectil hacia el objetivo
        Vector3 direction = (target.position - transform.position).normalized;
        transform.right = direction;
    }

    void Update()
    {
        if (target == null)
        {
            Debug.Log("El objetivo es nulo. Destruyendo proyectil.");
            Destroy(gameObject); // Destruir el proyectil si no hay objetivo
            return;
        }

        // Mover el proyectil hacia el objetivo
        Vector3 direction = (target.position - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        // Verificar si el proyectil alcanzará el objetivo en este frame
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= distanceThisFrame)
        {
            Debug.Log("Proyectil ha alcanzado el objetivo.");
            HitTarget();
            return;
        }

        transform.Translate(direction * distanceThisFrame, Space.World);
    }

    // Método para manejar el impacto con el objetivo
    void HitTarget()
    {
        Debug.Log("HitTarget() llamado.");

        // Registrar que el objetivo ha sido golpeado
        hitTargets.Add(target);

        // Generar partículas de impacto
        if (impactParticles != null)
        {
            ParticleSystem particles = Instantiate(impactParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }

        // Si el área de efecto es mayor que 0, aplicar daño en área
        if (areaDeEfecto > 0)
        {
            ApplyAreaDamage();
        }
        else
        {
            // Si el área de efecto es 0, aplicar daño solo al objetivo principal
            if (target.CompareTag("Enemigo"))
            {
                Enemigo enemy = target.GetComponent<Enemigo>();
                if (enemy != null)
                {
                    Debug.Log($"Infligiendo {damage} de daño a {target.name}.");
                    enemy.TakeDamage(damage);
                }
            }
        }

        // Si quedan rebotes disponibles, buscar el enemigo más cercano
        if (remainingBounces > 0)
        {
            Transform nextTarget = FindNearestEnemy();
            if (nextTarget != null)
            {
                target = nextTarget;
                remainingBounces--;
                Debug.Log($"Rebotando al siguiente objetivo: {target.name}. Rebotes restantes: {remainingBounces}");

                // Reorientar el proyectil hacia el nuevo objetivo
                Vector3 direction = (target.position - transform.position).normalized;
                transform.right = direction;

                return; // Continuar con el siguiente objetivo
            }
        }

        // Destruir el proyectil después de rebotar o si no hay más enemigos
        Destroy(gameObject);
    }

    // Método para infligir daño en área a todos los enemigos dentro del área de efecto
    void ApplyAreaDamage()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, areaDeEfecto, LayerMask.GetMask("Enemigo"));

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            Enemigo enemy = enemyCollider.GetComponent<Enemigo>();
            if (enemy != null)
            {
                Debug.Log($"Infligiendo {damage} de daño en área a {enemy.name}.");
                enemy.TakeDamage(damage);
            }
        }
    }

    // Método para encontrar el enemigo más cercano que no haya sido golpeado
    Transform FindNearestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, areaDeRebote, LayerMask.GetMask("Enemigo"));
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            Transform enemyTransform = enemyCollider.transform;
            if (!hitTargets.Contains(enemyTransform)) // Excluir enemigos ya golpeados
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemyTransform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    nearestEnemy = enemyTransform;
                    shortestDistance = distanceToEnemy;
                }
            }
        }

        return nearestEnemy;
    }

    // Opcional: Visualización del área de efecto y rebote en el Editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, target.position);
        }

        // Dibujar el área de rebote
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaDeRebote);

        // Dibujar el área de efecto
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, areaDeEfecto);
    }
}
