using UnityEngine;

public class ProyectilBala : Proyectil
{
    [Header("Configuración de la Bala")]
    [SerializeField] private float dañoBala = 5f;
    [SerializeField] private float velocidadBala = 10f;

    void Awake()
    {
        daño = dañoBala;
        velocidad = velocidadBala;
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
