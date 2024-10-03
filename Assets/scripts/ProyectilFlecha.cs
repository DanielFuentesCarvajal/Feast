using UnityEngine;

public class ProyectilFlecha : Proyectil
{
    [Header("Configuraci�n de la Flecha")]
    [SerializeField] private float da�oFlecha = 100f;
    [SerializeField] private float velocidadFlecha = 5f;

    void Awake()
    {
        da�o = da�oFlecha;
        velocidad = velocidadFlecha;
        // Asigna part�culas espec�ficas si es necesario
    }

    public override void Iniciar(Vector3 direcci�n)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direcci�n * velocidad;
        }
    }
}
