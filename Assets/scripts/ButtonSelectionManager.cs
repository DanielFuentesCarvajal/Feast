using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectionManager : MonoBehaviour
{
    public Button buttonCatapulta;
    public Button buttonEscorpion;

    public GameObject catapultaPrefab;
    public GameObject escorpionPrefab;

    private GameObject selectedPrefab;

    private void Start()
    {
        buttonCatapulta.onClick.AddListener(() => SelectPrefab(catapultaPrefab));
        buttonEscorpion.onClick.AddListener(() => SelectPrefab(escorpionPrefab));
    }

    // Cambiar a público para que pueda ser accedido desde otros scripts
    public void SelectPrefab(GameObject prefab)
    {
        selectedPrefab = prefab;
    }

    public GameObject GetSelectedPrefab()
    {
        return selectedPrefab;
    }
}
