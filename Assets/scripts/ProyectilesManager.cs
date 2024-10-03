using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;

    public GameObject defaultProjectilePrefab; // Prefab predeterminado si no se asigna uno específico

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: Mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
