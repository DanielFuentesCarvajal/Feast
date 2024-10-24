using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class TorretaPlacementManager : MonoBehaviour
{
    public Tilemap tilemapTerreno;
    public GameObject panelTorreta;  // El panel de venta y mejora
    public Button buttonVenta;
    public Button buttonMejora;
    public TextMeshProUGUI precioVentaText;
    public TextMeshProUGUI precioMejoraText;
    public Canvas canvas; // Añade esta línea para declarar la variable
    private ButtonSelectionManager buttonSelectionManager; // Referencia al script de botones
    private GameObject torretaSeleccionada;  // Referencia a la torreta seleccionada actualmente

    void Start()
    {
        buttonSelectionManager = FindObjectOfType<ButtonSelectionManager>();

        // Asignar acciones a los botones
        buttonVenta.onClick.AddListener(VenderTorreta);
        buttonMejora.onClick.AddListener(MejorarTorreta);

        // Ocultar el panel inicialmente
        panelTorreta.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 posicionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            posicionMouse.z = 0;

            // Comprobar si se hizo clic fuera del panel
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelTorreta.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
            {
                panelTorreta.SetActive(false); // Ocultar el panel si se hizo clic fuera
            }

            Vector3Int posicionTile = tilemapTerreno.WorldToCell(posicionMouse);
            TileBase tileEnPosicion = tilemapTerreno.GetTile(posicionTile);

            if (tileEnPosicion != null)
            {
                Vector3 posicionMundo = tilemapTerreno.GetCellCenterWorld(posicionTile);
                Vector3 ajuste = new Vector3(0, 0.5f, 0);

                if (HayTorretaEnPosicion(posicionMundo))
                {
                    SeleccionarTorretaEnPosicion(posicionMundo);
                }
                else
                {
                    ColocarNuevaTorreta(posicionMundo + ajuste);
                }
            }
        }
    }


    void ColocarNuevaTorreta(Vector3 posicionMundo)
    {
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
                    Instantiate(prefabTorreta, posicionMundo, Quaternion.identity);
                    buttonSelectionManager.SelectPrefab(null);
                }
                else
                {
                    Debug.Log("No tienes suficientes monedas para esta torreta.");
                }
            }
        }
    }

    void SeleccionarTorretaEnPosicion(Vector3 posicion)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(posicion);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Torreta"))
            {
                torretaSeleccionada = collider.gameObject;
                MostrarPanelTorreta();
                break;
            }
        }
    }

    void MostrarPanelTorreta()
    {
        if (torretaSeleccionada != null)
        {
            // Obtener los datos de la torreta seleccionada
            Turret turretScript = torretaSeleccionada.GetComponent<Turret>();
            if (turretScript != null)
            {
                // Actualizar los textos de precio en el panel
                precioVentaText.text = turretScript.precioVenta.ToString();

                // Verificar si la torreta tiene una mejora disponible
                if (turretScript.prefabTorretaMejorada != null)
                {
                    // Mostrar el botón de mejora y actualizar el precio de mejora
                    buttonMejora.gameObject.SetActive(true);
                    precioMejoraText.text = turretScript.precioMejora.ToString();
                }
                else
                {
                    // Ocultar el botón de mejora si no hay mejora disponible
                    buttonMejora.gameObject.SetActive(false);
                }

                // Colocar el panel cerca de la torreta seleccionada
                Vector3 posicionPanel = CalcularPosicionPanelCanvas(torretaSeleccionada.transform.position);
                panelTorreta.GetComponent<RectTransform>().anchoredPosition = posicionPanel;

                // Mostrar el panel
                panelTorreta.SetActive(true);
            }
        }
    }


    Vector3 CalcularPosicionPanelCanvas(Vector3 posicionTorreta)
    {
        // Convertir la posición de la torreta (en el mundo) a una posición de pantalla
        Vector3 screenPos = Camera.main.WorldToScreenPoint(posicionTorreta);

        // Convertir la posición de pantalla a coordenadas del canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera, out localPoint);

        // Obtener el tamaño del panel para calcular ajustes si es necesario
        RectTransform panelRect = panelTorreta.GetComponent<RectTransform>();
        Vector2 panelSize = panelRect.sizeDelta;

        // Inicializar la posición del panel en la esquina superior derecha
        Vector2 posicionPanel = new Vector2(localPoint.x + panelSize.x / 2, localPoint.y + panelSize.y / 2);

        // Comprobar si el panel se sale por arriba
        if (posicionPanel.y > canvasRect.rect.height / 2)
        {
            // Cambiar a la esquina inferior derecha
            posicionPanel.y = localPoint.y - panelSize.y / 2;

            // Comprobar si se sale por la derecha
            if (posicionPanel.x > canvasRect.rect.width / 2)
            {
                // Cambiar a la esquina inferior izquierda
                posicionPanel.x = localPoint.x - panelSize.x / 2;
            }
        }
        // Comprobar si el panel se sale por abajo
        else if (posicionPanel.y < -canvasRect.rect.height / 2)
        {
            // Cambiar a la esquina inferior derecha
            posicionPanel.y = localPoint.y - panelSize.y / 2;

            // Comprobar si se sale por la derecha
            if (posicionPanel.x > canvasRect.rect.width / 2)
            {
                // Cambiar a la esquina inferior izquierda
                posicionPanel.x = localPoint.x - panelSize.x / 2;
            }
        }
        // Verificar si el panel se sale por la derecha
        else if (posicionPanel.x > canvasRect.rect.width / 2)
        {
            // Cambiar a la esquina superior izquierda
            posicionPanel.x = localPoint.x - panelSize.x / 2;
            posicionPanel.y = localPoint.y + panelSize.y / 2;

            // Comprobar si se sale por arriba
            if (posicionPanel.y > canvasRect.rect.height / 2)
            {
                // Cambiar a la esquina inferior izquierda
                posicionPanel.y = localPoint.y - panelSize.y / 2;
            }
        }

        // Ajustar la posición final en el rectángulo del canvas
        // Comprobar que el panel no esté parcialmente fuera del canvas
        if (posicionPanel.x + panelSize.x / 2 > canvasRect.rect.width / 2) // Fuera por la derecha
        {
            posicionPanel.x = canvasRect.rect.width / 2 - panelSize.x / 2; // Ajustar al borde
        }
        if (posicionPanel.x - panelSize.x / 2 < -canvasRect.rect.width / 2) // Fuera por la izquierda
        {
            posicionPanel.x = -canvasRect.rect.width / 2 + panelSize.x / 2; // Ajustar al borde
        }
        if (posicionPanel.y + panelSize.y / 2 > canvasRect.rect.height / 2) // Fuera por arriba
        {
            posicionPanel.y = canvasRect.rect.height / 2 - panelSize.y / 2; // Ajustar al borde
        }
        if (posicionPanel.y - panelSize.y / 2 < -canvasRect.rect.height / 2) // Fuera por abajo
        {
            posicionPanel.y = -canvasRect.rect.height / 2 + panelSize.y / 2; // Ajustar al borde
        }

        return new Vector3(posicionPanel.x, posicionPanel.y, 0f);
    }

    Vector3 CalcularPosicionPanel(Vector3 posicionTorreta)
    {
        Vector3 posicionPanel = posicionTorreta + new Vector3(1f, 1f, 0f); // Posición por defecto (arriba a la derecha)

        // Comprobar si hay suficiente espacio en la vista (ajustar si es necesario)
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(posicionPanel);
        if (viewportPos.x > 1f || viewportPos.y > 1f)  // Si está fuera a la derecha o arriba
        {
            posicionPanel = posicionTorreta + new Vector3(-1f, -1f, 0f);  // Mover abajo a la izquierda
        }
        return posicionPanel;
    }

    void VenderTorreta()
    {
        if (torretaSeleccionada != null)
        {
            Turret turretScript = torretaSeleccionada.GetComponent<Turret>();
            if (turretScript != null)
            {
                GameManager.Instance.AddCoins(turretScript.precioVenta);  // Añadir monedas por la venta
                Destroy(torretaSeleccionada);  // Eliminar la torreta
                panelTorreta.SetActive(false);  // Ocultar el panel
            }
        }
    }

    void MejorarTorreta()
    {
        if (torretaSeleccionada != null)
        {
            Turret turretScript = torretaSeleccionada.GetComponent<Turret>();
            if (turretScript != null && turretScript.prefabTorretaMejorada != null)
            {
                if (GameManager.Instance.SubtractCoins(turretScript.precioMejora))
                {
                    // Mejorar la torreta
                    Instantiate(turretScript.prefabTorretaMejorada, torretaSeleccionada.transform.position, Quaternion.identity);
                    Destroy(torretaSeleccionada);  // Eliminar la torreta antigua
                    panelTorreta.SetActive(false);  // Ocultar el panel
                }
                else
                {
                    Debug.Log("No tienes suficientes monedas para mejorar la torreta.");
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
