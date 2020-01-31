using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{/*
    public GameObject objTxtBestChoices;
    Text txtBestChoices;

    public GameObject objTimer;
    TimerUI scrTimer;

    public GameObject objSendScore;
    SendScore sendScoreScript;

    bool startedQuery;

    // Use this for initialization
    void Awake()
    {
        txtBestChoices = objTxtBestChoices.GetComponent<Text>();
        scrTimer = objTimer.GetComponent<TimerUI>();
        sendScoreScript = objSendScore.GetComponent<SendScore>();
    }

    // Use this for setting values
    void Start()
    {
        scrTimer.Activate(Gamechoose.choose.collecting_results_duration_ms);

        startedQuery = false;

        UpdateScores();

        sendScoreScript.SendWordSelectionAllRounds();
    }

    void UpdateScores()
    {
        string bestWordsForWords = "";
        for (int i = 0; i < Gamechoose.choose.number_of_rounds; i++)
        {
            bestWordsForWords += (i + 1) + ": " + Gamechoose.bestChoices[i] + " " + Gamechoose.choose.words[i].word + "\n"; 
        }
        txtBestChoices.text = bestWordsForWords;
    }

    void Update()
    {
        if (scrTimer.time <= 0)
        {
            SceneSwitcher.LoadScene2(GameSettings.SCOREBOARD);
        }
    }*/
}
