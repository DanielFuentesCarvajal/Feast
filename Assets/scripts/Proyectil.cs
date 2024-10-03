using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target; // Objetivo del proyectil
    private float speed;
    private float damage;
    private float areaOfEffect;

    public ParticleSystem impactParticles; // Part�culas al impactar
    public ParticleSystem shootParticles; // Part�culas al disparar

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
        areaOfEffect = config.areaOfEffect;

        Debug.Log($"Proyectil inicializado con objetivo: {target.name}, Da�o: {damage}, Velocidad: {speed}");

        // Asignar part�culas de impacto y disparo
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
    // M�todo para manejar el impacto con el objetivo
    // M�todo para manejar el impacto con el objetivo
    void HitTarget()
    {
        Debug.Log("HitTarget() llamado.");

        if (impactParticles != null)
        {
            ParticleSystem particles = Instantiate(impactParticles, transform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particles.main.duration);
        }

        // Infligir da�o al objetivo
        if (target.CompareTag("Enemigo"))
        {
            Enemigo enemy = target.GetComponent<Enemigo>();
            if (enemy != null)
            {
                Debug.Log($"Infligiendo {damage} de da�o a {target.name}.");
                enemy.TakeDamage(damage); // Llamar a TakeDamage del enemigo
            }
        }
        else if (areaOfEffect > 0f)
        {
            // Infligir da�o en �rea de efecto
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, areaOfEffect, LayerMask.GetMask("Enemigo"));
            foreach (Collider2D hitEnemy in enemies)
            {
                Enemigo enemyScript = hitEnemy.GetComponent<Enemigo>();
                if (enemyScript != null)
                {
                    Debug.Log($"Infligiendo {damage} de da�o a {hitEnemy.name} en �rea de efecto.");
                    enemyScript.TakeDamage(damage); // Llamar a TakeDamage del enemigo
                }
            }
        }

        Destroy(gameObject); // Destruir el proyectil despu�s del impacto
    }



    // Opcional: Visualizaci�n del trayecto del proyectil en el Editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
