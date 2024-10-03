using UnityEngine;

public abstract class Proyectil : MonoBehaviour
{
    [Header("Propiedades del Proyectil")]
    public float daño; // Daño del proyectil
    public float velocidad; // Velocidad del proyectil
    public float areaEfecto; // Área de efecto del proyectil

    public ParticleSystem particulaSistemaImpacto; // Partículas al impactar
    public ParticleSystem particulaSistemaDisparo; // Partículas al disparar

    // Marcar el método como virtual para que pueda ser sobrescrito
    public virtual void Iniciar(Vector3 direccion)
    {
        // Lógica base para disparar el proyectil, que será sobreescrita en las subclases.
        ActivarParticulasDisparo(); // Solo se activan las partículas cuando se dispara.
    }

    protected virtual void Impacto()
    {
        if (particulaSistemaImpacto != null)
        {
            ParticleSystem particula = Instantiate(particulaSistemaImpacto, transform.position, Quaternion.identity);
            particula.Play();
            Destroy(particula.gameObject, particula.main.duration);
        }

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(transform.position, areaEfecto);

        foreach (Collider2D enemigo in enemigos)
        {
            if (enemigo.CompareTag("Enemigo"))
            {
                Enemigo enemigoScript = enemigo.GetComponent<Enemigo>();
                if (enemigoScript != null)
                {
                    enemigoScript.TakeDamage(daño);
                }
            }
        }

        Destroy(gameObject); // Destruir el proyectil después del impacto
    }

    private void ActivarParticulasDisparo()
    {
        if (particulaSistemaDisparo != null)
        {
            ParticleSystem particula = Instantiate(particulaSistemaDisparo, transform.position, Quaternion.identity);
            particula.Play();
            Destroy(particula.gameObject, particula.main.duration);
        }
    }
}
