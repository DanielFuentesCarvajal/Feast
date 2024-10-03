using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileConfig", menuName = "Projectile/Config")]
public class ProjectileConfig : ScriptableObject
{
    public float damage = 10f;
    public float speed = 10f;
    public float areaOfEffect = 0f;
    public ParticleSystem impactParticles;
    public ParticleSystem shootParticles;
    public GameObject visualPrefab; // Opcional: Prefab visual específico para el proyectil
}
