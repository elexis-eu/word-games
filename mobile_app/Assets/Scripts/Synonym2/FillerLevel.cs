using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FillerLevel : MonoBehaviour
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
        levelText.text = GameSettings.localizationManager.GetTextForKey("SYNONYM_SOLO_LEVEL_FILLER").Replace("{{LEVEL}}", GameInfoSynonym2.info.currentLevel.ToString());
    }

    
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = timeLeftDefault;
            SceneSwitcher.LoadScene2(GameSettings.SCENE_ROOM_PICKER);
        }

    }

    private void OnApplicationPause(bool pause)
    {

    }
}
