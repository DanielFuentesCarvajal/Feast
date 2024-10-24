using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileConfig", menuName = "Projectile/Config")]
public class ProjectileConfig : ScriptableObject
{
    public float damage = 10f;
    public float speed = 10f;
    public float areaDeRebote = 0f; // Área para buscar el próximo enemigo para el rebote
    public float areaDeEfecto = 0f; // Área de daño en el impacto
    public int maxBounces = 1; // Valor público para definir cuántos enemigos puede golpear el proyectil
    public ParticleSystem impactParticles;
    public ParticleSystem shootParticles;
    public GameObject visualPrefab; // Opcional: Prefab visual específico para el proyectil
}
