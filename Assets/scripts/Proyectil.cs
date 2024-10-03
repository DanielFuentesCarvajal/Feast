using UnityEngine;

public abstract class Proyectil : MonoBehaviour
{
    [Header("Propiedades del Proyectil")]
    public float da�o; // Da�o del proyectil
    public float velocidad; // Velocidad del proyectil
    public float areaEfecto; // �rea de efecto del proyectil

    public ParticleSystem particulaSistemaImpacto; // Part�culas al impactar
    public ParticleSystem particulaSistemaDisparo; // Part�culas al disparar

    // Marcar el m�todo como virtual para que pueda ser sobrescrito
    public virtual void Iniciar(Vector3 direccion)
    {
        // L�gica base para disparar el proyectil, que ser� sobreescrita en las subclases.
        ActivarParticulasDisparo(); // Solo se activan las part�culas cuando se dispara.
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
                    enemigoScript.TakeDamage(da�o);
                }
            }
        }

        Destroy(gameObject); // Destruir el proyectil despu�s del impacto
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
