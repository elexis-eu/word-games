using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPlayerPrefab : MonoBehaviour
{
    public GameObject positionImageObj, playerNameTextObj, playerScoreTextObj, helperLineObj, levelImageObj;

    [HideInInspector]
    public Image positionImage;
    [HideInInspector]
    public Text positionText;
    [HideInInspector]
    public Text playerNameText;
    [HideInInspector]
    public Text playerScoreText;
    [HideInInspector]
    public Image levelImage;
    [HideInInspector]
    public Text levelText;

    private void Awake()
    {
        positionImage = positionImageObj.GetComponent<Image>();
        positionText = positionImageObj.GetComponentInChildren<Text>();
        playerNameText = playerNameTextObj.GetComponent<Text>();
        playerScoreText = playerScoreTextObj.GetComponent<Text>();
        levelText = levelImageObj.GetComponentInChildren<Text>();
    }
}
