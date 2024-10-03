using UnityEngine;

public class ProyectilFlecha : Proyectil
{
    [Header("Configuración de la Flecha")]
    [SerializeField] private float dañoFlecha = 100f;
    [SerializeField] private float velocidadFlecha = 5f;

    void Awake()
    {
        daño = dañoFlecha;
        velocidad = velocidadFlecha;
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
