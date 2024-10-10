using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;

    public GameObject defaultProjectilePrefab; // Prefab predeterminado si no se asigna uno espec�fico

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
