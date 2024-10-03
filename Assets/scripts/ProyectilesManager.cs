using UnityEngine;
using System.Collections.Generic;

public class ProyectilesManager : MonoBehaviour
{
    public static ProyectilesManager Instance { get; private set; }

    public enum TipoProyectil
    {
        Flecha,
        Bala,
        BolaDeHierro
    }

    [Header("Prefabs de Proyectiles")]
    public GameObject proyectilFlechaPrefab;
    public GameObject proyectilBalaPrefab;
    public GameObject proyectilBolaDeHierroPrefab;

    private Dictionary<TipoProyectil, GameObject> proyectiles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InicializarProyectiles();
            Debug.Log("ProyectilesManager inicializado.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InicializarProyectiles()
    {
        proyectiles = new Dictionary<TipoProyectil, GameObject>
        {
            { TipoProyectil.Flecha, proyectilFlechaPrefab },
            { TipoProyectil.Bala, proyectilBalaPrefab },
            { TipoProyectil.BolaDeHierro, proyectilBolaDeHierroPrefab }
        };
        Debug.Log("Proyectiles inicializados: " + proyectiles.Count);
    }

    public GameObject ObtenerProyectilPrefab(TipoProyectil tipoProyectil)
    {
        if (proyectiles.TryGetValue(tipoProyectil, out GameObject prefab))
        {
            Debug.Log("Obtenido prefab de proyectil: " + prefab.name);
            return prefab;
        }
        else
        {
            Debug.LogWarning("Tipo de proyectil no encontrado: " + tipoProyectil);
            return null;
        }
    }
}
