using UnityEngine;

public class ProyectilBala : Proyectil
{
    [Header("Configuraci�n de la Bala")]
    [SerializeField] private float da�oBala = 5f;
    [SerializeField] private float velocidadBala = 10f;

    void Awake()
    {
        da�o = da�oBala;
        velocidad = velocidadBala;
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
