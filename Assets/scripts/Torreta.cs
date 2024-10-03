using UnityEngine;

public class Torreta : MonoBehaviour
{
    [Header("Configuración de la Torreta")]
    public ProyectilesManager.TipoProyectil tipoProyectil; // Lista desplegable en el Inspector
    public float rangoMinimo = 1f;
    public float rangoMaximo = 5f;
    public float tiempoEntreDisparos = 1f;

    private Transform objetivo;
    private float siguienteDisparo = 0f;
    private ProyectilesManager proyectilesManager;

    void Start()
    {
        Debug.Log("Torreta iniciada.");
        proyectilesManager = ProyectilesManager.Instance;
        if (proyectilesManager == null)
        {
            Debug.LogError("ProyectilesManager no se ha encontrado.");
        }
        else
        {
            Debug.Log("ProyectilesManager encontrado.");
        }
    }

    void Update()
    {
        Debug.Log("Update llamado.");
        BuscarObjetivo();

        if (objetivo != null)
        {
            float distancia = Vector3.Distance(transform.position, objetivo.position);
            Debug.Log("Distancia al objetivo: " + distancia);

            // Verifica si el enemigo está dentro del rango mínimo y máximo
            if (distancia >= rangoMinimo && distancia <= rangoMaximo)
            {
                Debug.Log("El enemigo está dentro del rango.");
                // Dispara solo si ha pasado el tiempo suficiente entre disparos
                if (Time.time >= siguienteDisparo)
                {
                    Debug.Log("Es hora de disparar.");
                    Disparar();
                    siguienteDisparo = Time.time + tiempoEntreDisparos;
                }
                else
                {
                    Debug.Log("Esperando al próximo disparo.");
                }
            }
            else
            {
                Debug.Log("El enemigo está fuera del rango.");
            }
        }
        else
        {
            Debug.Log("No se ha encontrado ningún objetivo.");
        }
    }

    void BuscarObjetivo()
    {
        Debug.Log("Buscando objetivo.");
        // Busca enemigos dentro del rango máximo
        Collider2D[] enemigosEnRango = Physics2D.OverlapCircleAll(transform.position, rangoMaximo);
        Debug.Log("Enemigos encontrados: " + enemigosEnRango.Length);

        Transform enemigoCercano = null;
        float distanciaCercana = Mathf.Infinity;

        foreach (Collider2D collider in enemigosEnRango)
        {
            if (collider.CompareTag("Enemigo"))
            {
                float distancia = Vector3.Distance(transform.position, collider.transform.position);
                Debug.Log("Distancia al enemigo: " + distancia);
                if (distancia < distanciaCercana)
                {
                    distanciaCercana = distancia;
                    enemigoCercano = collider.transform;
                }
            }
        }

        objetivo = enemigoCercano;
        if (objetivo != null)
        {
            Debug.Log("Objetivo asignado: " + objetivo.name);
        }
        else
        {
            Debug.Log("No se ha asignado ningún objetivo.");
        }
    }

    void Disparar()
    {
        Debug.Log("Intentando disparar proyectil.");
        if (proyectilesManager != null && objetivo != null)
        {
            Debug.Log("Intentando disparar proyectil al objetivo: " + objetivo.name);
            GameObject proyectilPrefab = proyectilesManager.ObtenerProyectilPrefab(tipoProyectil);
            if (proyectilPrefab != null)
            {
                Debug.Log("Proyectil prefab encontrado: " + proyectilPrefab.name);
                GameObject proyectilObj = Instantiate(proyectilPrefab, transform.position, Quaternion.identity);
                Proyectil proyectilScript = proyectilObj.GetComponent<Proyectil>();
                if (proyectilScript != null)
                {
                    // Direcciona el proyectil hacia el enemigo
                    Vector3 direccion = (objetivo.position - transform.position).normalized;
                    Debug.Log("Disparando proyectil en la dirección: " + direccion);
                    proyectilScript.Iniciar(direccion);
                }
                else
                {
                    Debug.LogError("El proyectil instanciado no tiene el script Proyectil.");
                }
            }
            else
            {
                Debug.LogError("No se pudo obtener el prefab del proyectil.");
            }
        }
        else
        {
            if (proyectilesManager == null)
            {
                Debug.LogError("ProyectilesManager es nulo.");
            }
            if (objetivo == null)
            {
                Debug.LogError("Objetivo es nulo.");
            }
        }
    }
}
