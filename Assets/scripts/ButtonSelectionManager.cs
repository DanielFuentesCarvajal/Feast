using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro
using UnityEngine.SceneManagement; // Para cambiar de escenas
using System.Collections;

public class ButtonSelectionManager : MonoBehaviour
{
    public Button buttonCatapulta;
    public Button buttonEscorpion;

    public GameObject catapultaPrefab;
    public GameObject escorpionPrefab;

    public GameObject VictoryPanel; // Panel de victoria
    public GameObject DefeatPanel; // Panel de derrota
    public Button NextLevelButton; // Botón para pasar al siguiente nivel
    public Button RetryButton; // Botón para reintentar nivel

    public TextMeshProUGUI timerText; // Cronómetro en TextMeshPro
    public float timerMinutes = 0f; // Minutos configurables
    public float timerSeconds = 0f; // Segundos configurables

    private GameObject selectedPrefab;
    private bool gamePaused = false;
    private bool gameOver = false; // Para evitar que se mueva el tiempo tras la derrota o victoria

    private void Start()
    {
        buttonCatapulta.onClick.AddListener(() => SelectPrefab(catapultaPrefab));
        buttonEscorpion.onClick.AddListener(() => SelectPrefab(escorpionPrefab));

        // Ocultar paneles al inicio
        VictoryPanel.SetActive(false);
        DefeatPanel.SetActive(false);

        // Asignar acciones a los botones de victoria y derrota
        NextLevelButton.onClick.AddListener(GoToNextLevel);
        RetryButton.onClick.AddListener(RetryLevel);

        // Iniciar el cronómetro
        StartCoroutine(StartTimer());
    }

    // Selección de prefab
    public void SelectPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

    public GameObject GetSelectedPrefab()
    {
        return selectedPrefab;
    }

    // Método para activar el panel de victoria
    public void Victory()
    {
        gamePaused = true;
        VictoryPanel.SetActive(true);
        Time.timeScale = 0; // Pausar el juego
    }

    // Método para activar el panel de derrota
    public void Defeat()
    {
        gamePaused = true;
        DefeatPanel.SetActive(true);
        Time.timeScale = 0; // Pausar el juego
    }

    // Ir al siguiente nivel
    public void GoToNextLevel()
    {
        Time.timeScale = 1; // Restaurar la escala de tiempo
        SceneManager.LoadScene("NextLevelScene"); // Cambia el nombre de la escena aquí
    }

    // Reintentar el nivel actual
    public void RetryLevel()
    {
        Time.timeScale = 1; // Restaurar la escala de tiempo
        GameManager.Instance.RetryLevel(); // Llamar al método del GameManager
    }


    // Cronómetro configurable
    IEnumerator StartTimer()
    {
        float totalTime = (timerMinutes * 60) + timerSeconds;

        while (totalTime > 0 && !gameOver)
        {
            if (!gamePaused)
            {
                totalTime -= Time.deltaTime;

                // Calcular minutos y segundos
                int minutes = Mathf.FloorToInt(totalTime / 60);
                int seconds = Mathf.FloorToInt(totalTime % 60);

                // Actualizar el texto del cronómetro
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            yield return null;
        }

        if (totalTime <= 0 && !gameOver)
        {
            Victory(); // Llamar a victoria cuando el tiempo llegue a 0
        }
    }

    // Método para detectar si el objetivo ha llegado a 0
    public void CheckObjectiveHealth(float health)
    {
        if (health <= 0 && !gameOver)
        {
            Defeat(); // Activar derrota si la vida del objetivo es 0
        }
    }
}