using UnityEngine;
using UnityEngine.UI;

public class DragResultPrefab : MonoBehaviour
{
    public GameObject correctObj, incorrectObj, wordNameObj, wordScoreObj, helperLineObj, bonusObj;

    [HideInInspector]
    public Text wordNameText;
    [HideInInspector]
    public Text wordScoreText;

    private void Awake()
    {
        wordNameText = wordNameObj.GetComponent<Text>();
        wordScoreText = wordScoreObj.GetComponent<Text>();
    }
}
