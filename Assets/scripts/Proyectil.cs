using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target; // Objetivo del proyectil
    private float speed;
    private float damage;
    private float areaOfEffect;

    public ParticleSystem impactParticles; // Partículas al impactar
    public ParticleSystem shootParticles; // Partículas al disparar

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
        areaOfEffect = config.areaOfEffect;

        Debug.Log($"Proyectil inicializado con objetivo: {target.name}, Daño: {damage}, Velocidad: {speed}");

        // Asignar partículas de impacto y disparo
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
    // Método para manejar el impacto con el objetivo
    // Método para manejar el impacto con el objetivo
    void HitTarget()
    {
        Debug.Log("HitTarget() llamado.");

        if (impactParticles != null)
        {
            ParticleSystem particles = Instantiate(impactParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }

        // Infligir daño al objetivo
        if (target.CompareTag("Enemigo"))
        {
            Enemigo enemy = target.GetComponent<Enemigo>();
            if (enemy != null)
            {
                Debug.Log($"Infligiendo {damage} de daño a {target.name}.");
                enemy.TakeDamage(damage); // Llamar a TakeDamage del enemigo
            }
        }
        else if (areaOfEffect > 0f)
        {
            // Infligir daño en área de efecto
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, areaOfEffect, LayerMask.GetMask("Enemigo"));
            foreach (Collider2D hitEnemy in enemies)
            {
                Enemigo enemyScript = hitEnemy.GetComponent<Enemigo>();
                if (enemyScript != null)
                {
                    Debug.Log($"Infligiendo {damage} de daño a {hitEnemy.name} en área de efecto.");
                    enemyScript.TakeDamage(damage); // Llamar a TakeDamage del enemigo
                }
            }
        }

        Destroy(gameObject); // Destruir el proyectil después del impacto
    }



    // Opcional: Visualización del trayecto del proyectil en el Editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
