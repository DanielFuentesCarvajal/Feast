using UnityEngine;

public class ProyectilBolaDeHierro : Proyectil
{
    [Header("Configuración de la Bola de Hierro")]
    [SerializeField] private float dañoBolaDeHierro = 150f;
    [SerializeField] private float velocidadBolaDeHierro = 3f;

    void Awake()
    {
        daño = dañoBolaDeHierro;
        velocidad = velocidadBolaDeHierro;
        // Asigna partículas específicas si es necesario
    }

    public override void Iniciar(Vector3 dirección)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = dirección * velocidad;
        }
    }
}
