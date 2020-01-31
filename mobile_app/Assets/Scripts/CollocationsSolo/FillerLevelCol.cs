using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FillerLevelCol : MonoBehaviour
{
    public Text levelText;
    private float timeLeft;
    private float timeLeftDefault = 2.0f;

    // Use this for initialization
    void Awake()
    {        
        timeLeft = timeLeftDefault;
    }

    void Start()
    {
        //levelText.text = "Stopnja " + GameInfoCollocation.info.currentLevel;
        levelText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_LEVEL_FILLER").Replace("{{LEVEL}}", GameInfoCollocation.info.currentLevel.ToString());
    }

    
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = timeLeftDefault;
            SceneSwitcher.LoadScene2(GameSettings.SCENE_ROOM_PICKER_COL);
        }

    }

    private void OnApplicationPause(bool pause)
    {

    }
}
