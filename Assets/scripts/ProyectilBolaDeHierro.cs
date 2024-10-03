using UnityEngine;

public class ProyectilBolaDeHierro : Proyectil
{
    [Header("Configuraci�n de la Bola de Hierro")]
    [SerializeField] private float da�oBolaDeHierro = 150f;
    [SerializeField] private float velocidadBolaDeHierro = 3f;

    void Awake()
    {
        da�o = da�oBolaDeHierro;
        velocidad = velocidadBolaDeHierro;
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
