using UnityEngine;
using UnityEngine.Tilemaps;

public class TorretaPlacementManager : MonoBehaviour
{
    public Tilemap tilemapTerreno;
    private ButtonSelectionManager buttonSelectionManager; // Referencia al script de botones

    void Start()
    {
        buttonSelectionManager = FindObjectOfType<ButtonSelectionManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            posicionMouse.z = 0;

            Vector3Int posicionTile = tilemapTerreno.WorldToCell(posicionMouse);
            TileBase tileEnPosicion = tilemapTerreno.GetTile(posicionTile);

            if (tileEnPosicion != null)
            {
                Vector3 posicionMundo = tilemapTerreno.GetCellCenterWorld(posicionTile);
                Vector3 ajuste = new Vector3(0, 0.5f, 0);

                // Obtiene el prefab seleccionado
                GameObject prefabTorreta = buttonSelectionManager.GetSelectedPrefab();

                if (prefabTorreta != null && !HayTorretaEnPosicion(posicionMundo))
                {
                    // Obtener el coste de la torreta
                    Turret turretScript = prefabTorreta.GetComponent<Turret>();
                    if (turretScript != null)
                    {
                        int cost = turretScript.cost;
                        // Intentar restar el coste
                        if (GameManager.Instance.SubtractCoins(cost))
                        {
                            // Instanciar la torreta si se pudo restar el coste
                            Instantiate(prefabTorreta, posicionMundo + ajuste, Quaternion.identity);
                            // Desactiva la selección después de colocar la torreta
                            buttonSelectionManager.SelectPrefab(null);
                        }
                        else
                        {
                            // Opcional: Mostrar mensaje de falta de monedas
                            Debug.Log("No tienes suficientes monedas para esta torreta.");
                        }
                    }
                    else
                    {
                        // Si el prefab no tiene el script Turret, instanciar de igual forma
                        Instantiate(prefabTorreta, posicionMundo + ajuste, Quaternion.identity);
                        // Desactiva la selección después de colocar la torreta
                        buttonSelectionManager.SelectPrefab(null);
                    }
                }
            }
        }
    }

    bool HayTorretaEnPosicion(Vector3 posicion)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(posicion);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Torreta"))
            {
                return true;
            }
        }
        return false;
    }
}
