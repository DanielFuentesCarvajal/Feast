using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Función para cerrar el juego
    public void ExitGame()
    {
        Debug.Log("El juego se ha cerrado."); // Esto aparecerá en la consola del editor.
        Application.Quit();
    }

    // Función para cargar una nueva escena
    public void PlayGame()
    {
        SceneManager.LoadScene("Nivel1"); // Cambia "GameScene" por el nombre de la escena a la que quieras ir.
    }
}
