using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target; // Objetivo actual del proyectil
    private float speed;
    private float damage;
    private float areaDeRebote; // �rea para rebote del proyectil
    private float areaDeEfecto; // �rea para da�o en �rea
    private int remainingBounces; // Contador de rebotes restantes

    public ParticleSystem impactParticles; // Part�culas al impactar
    public ParticleSystem shootParticles; // Part�culas al disparar

    private List<Transform> hitTargets = new List<Transform>(); // Lista de enemigos golpeados

    // M�todo para inicializar el proyectil
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
        areaDeRebote = config.areaDeRebote; // Usar el �rea de rebote del ScriptableObject
        areaDeEfecto = config.areaDeEfecto; // Usar el �rea de efecto del ScriptableObject
        remainingBounces = config.maxBounces; // Usar el valor de rebotes del ScriptableObject

        Debug.Log($"Proyectil inicializado con objetivo: {target.name}, Da�o: {damage}, Velocidad: {speed}, Rebotes: {remainingBounces}");

        impactParticles = config.impactParticles;
        shootParticles = config.shootParticles;

        // Activar part�culas de disparo si est�n asignadas
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

        // Verificar si el proyectil alcanzar� el objetivo en este frame
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= distanceThisFrame)
        {
            Debug.Log("Proyectil ha alcanzado el objetivo.");
            HitTarget();
            return;
        }

        transform.Translate(direction * distanceThisFrame, Space.World);
    }

    // M�todo para manejar el impacto con el objetivo
    void HitTarget()
    {
        Debug.Log("HitTarget() llamado.");

        // Registrar que el objetivo ha sido golpeado
        hitTargets.Add(target);

        // Generar part�culas de impacto
        if (impactParticles != null)
        {
            ParticleSystem particles = Instantiate(impactParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }

        // Si el �rea de efecto es mayor que 0, aplicar da�o en �rea
        if (areaDeEfecto > 0)
        {
            ApplyAreaDamage();
        }
        else
        {
            // Si el �rea de efecto es 0, aplicar da�o solo al objetivo principal
            if (target.CompareTag("Enemigo"))
            {
                Enemigo enemy = target.GetComponent<Enemigo>();
                if (enemy != null)
                {
                    Debug.Log($"Infligiendo {damage} de da�o a {target.name}.");
                    enemy.TakeDamage(damage);
                }
            }
        }

        // Si quedan rebotes disponibles, buscar el enemigo m�s cercano
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

        // Destruir el proyectil despu�s de rebotar o si no hay m�s enemigos
        Destroy(gameObject);
    }

    // M�todo para infligir da�o en �rea a todos los enemigos dentro del �rea de efecto
    void ApplyAreaDamage()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, areaDeEfecto, LayerMask.GetMask("Enemigo"));

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            Enemigo enemy = enemyCollider.GetComponent<Enemigo>();
            if (enemy != null)
            {
                Debug.Log($"Infligiendo {damage} de da�o en �rea a {enemy.name}.");
                enemy.TakeDamage(damage);
            }
        }
    }

    // M�todo para encontrar el enemigo m�s cercano que no haya sido golpeado
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

    // Opcional: Visualizaci�n del �rea de efecto y rebote en el Editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, target.position);
        }

        // Dibujar el �rea de rebote
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaDeRebote);

        // Dibujar el �rea de efecto
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, areaDeEfecto);
    }
}
