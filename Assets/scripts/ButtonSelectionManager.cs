using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Para TextMeshPro
using System.Collections;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonSelectionManager : MonoBehaviour
{
    public Button buttonCatapulta;
    public Button buttonEscorpion;
    public Button buttontorreta3;

    public GameObject catapultaPrefab;
    public GameObject escorpionPrefab;
    public GameObject torreta3prefab;

    public TextMeshProUGUI catapultaCostText;
    public TextMeshProUGUI escorpionCostText;
    public TextMeshProUGUI torreta3CostText;

    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public Button NextLevelButton;
    public Button RetryButton;

    public TextMeshProUGUI timerText;
    public float timerMinutes = 0f;
    public float timerSeconds = 0f;
    public int nextSceneIndex;

    private GameObject selectedPrefab;
    private bool gamePaused = false;
    private bool gameOver = false;
    private Color originalColor;

    private void Start()
    {
        buttonCatapulta.onClick.AddListener(() => SelectPrefab(catapultaPrefab));
        buttonEscorpion.onClick.AddListener(() => SelectPrefab(escorpionPrefab));
        buttontorreta3.onClick.AddListener(() => SelectPrefab(torreta3prefab));

        VictoryPanel.SetActive(false);
        DefeatPanel.SetActive(false);

        NextLevelButton.onClick.AddListener(GoToNextLevel);
        RetryButton.onClick.AddListener(RetryLevel);

        AddHoverEffect(buttonCatapulta, catapultaPrefab, catapultaCostText);
        AddHoverEffect(buttonEscorpion, escorpionPrefab, escorpionCostText);
        AddHoverEffect(buttontorreta3, torreta3prefab, torreta3CostText);

        StartCoroutine(StartTimer());
    }

    private void AddHoverEffect(Button button, GameObject prefab, TextMeshProUGUI costText)
    {
        Turret turretScript = prefab.GetComponent<Turret>();
        if (turretScript == null)
        {
            Debug.LogWarning("Prefab no tiene el componente Turret.");
            return;
        }

        originalColor = button.image.color;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => OnHoverEnter(button, turretScript.cost, costText));

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => OnHoverExit(button, costText));

        trigger.triggers.Add(pointerEnter);
        trigger.triggers.Add(pointerExit);
    }

    private void OnHoverEnter(Button button, int cost, TextMeshProUGUI costText)
    {
        button.image.color = new Color(originalColor.r * 0.7f, originalColor.g * 0.7f, originalColor.b * 0.7f);
        costText.text = cost.ToString();
        costText.gameObject.SetActive(true);
    }

    private void OnHoverExit(Button button, TextMeshProUGUI costText)
    {
        button.image.color = originalColor;
        costText.gameObject.SetActive(false);
    }

    public void SelectPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

    public GameObject GetSelectedPrefab()
    {
        return selectedPrefab;
    }

    public void Victory()
    {
        gamePaused = true;
        VictoryPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Defeat()
    {
        gamePaused = true;
        DefeatPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void GoToNextLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void RetryLevel()
    {
        Time.timeScale = 1;
        GameManager.Instance.RetryLevel();
    }

    IEnumerator StartTimer()
    {
        float totalTime = (timerMinutes * 60) + timerSeconds;

        while (totalTime > 0 && !gameOver)
        {
            if (!gamePaused)
            {
                totalTime -= Time.deltaTime;

                int minutes = Mathf.FloorToInt(totalTime / 60);
                int seconds = Mathf.FloorToInt(totalTime % 60);

                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            yield return null;
        }

        if (totalTime <= 0 && !gameOver)
        {
            Victory();
        }
    }

    public void CheckObjectiveHealth(float health)
    {
        if (health <= 0 && !gameOver)
        {
            Defeat();
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ButtonSelectionManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ButtonSelectionManager levelManager = (ButtonSelectionManager)target;

        // Obtén todas las escenas incluidas en Build Settings
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }

        // Crear un menú desplegable para seleccionar la escena
        levelManager.nextSceneIndex = EditorGUILayout.Popup("Next Scene", levelManager.nextSceneIndex, scenes);

        // Guardar los cambios en el script
        if (GUI.changed)
        {
            EditorUtility.SetDirty(levelManager);
        }

        DrawDefaultInspector();
    }
}
#endif
