using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FillerWord : MonoBehaviour
{
    public Text wordText;
    private float timeLeft;
    private float timeLeftDefault = 2.0f;

    // Use this for initialization
    void Awake()
    {        
        timeLeft = timeLeftDefault;
    }

    void Start()
    {
        wordText.text = GameSettings.localizationManager.GetTextForKey("SYNONYM_SOLO_WORD_FILLER").Replace("{{WORD}}", GameInfoSynonym2.currentGame.currentWord);
    }

    
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
        {
            timeLeft = timeLeftDefault;
            SceneSwitcher.LoadScene2(GameSettings.SCENE_INPUT_SYNONYM);
        }

    }

    private void OnApplicationPause(bool pause)
    {

    }
}
