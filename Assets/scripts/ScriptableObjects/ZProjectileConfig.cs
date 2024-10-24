using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileConfig", menuName = "Projectile/Config")]
public class ProjectileConfig : ScriptableObject
{
    public float damage = 10f;
    public float speed = 10f;
    public float areaDeRebote = 0f; // �rea para buscar el pr�ximo enemigo para el rebote
    public float areaDeEfecto = 0f; // �rea de da�o en el impacto
    public int maxBounces = 1; // Valor p�blico para definir cu�ntos enemigos puede golpear el proyectil
    public ParticleSystem impactParticles;
    public ParticleSystem shootParticles;
    public GameObject visualPrefab; // Opcional: Prefab visual espec�fico para el proyectil
}
