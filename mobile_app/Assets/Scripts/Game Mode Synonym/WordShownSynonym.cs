using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WordShownSynonym : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject previewWordObj;
    private PreviewWordSynonym scrPrWPre;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        if (GameInfoSynonym.currentRoundTimeLeft != 0)
        {
            timeToNextGame = GameInfoSynonym.currentRoundTimeLeft;
            GameInfoSynonym.currentRoundTimeLeft = 0;
        }
        else
        {
            timeToNextGame = GameInfoSynonym.info.round_duration_ms;
        }

        GameInfoSynonym.SetNewRound();

        scrPrWPre = previewWordObj.GetComponent<PreviewWordSynonym>();
    }

    // Use this if you need initialization in other scripts to happen first
    void Start()
    {
        TimeUntilNextGameRound();

        GameSettings.MyDebug(GameInfoSynonym.currentRound.ToString());

        scrPrWPre.wordText.text = GameInfoSynonym.info.words[GameInfoSynonym.currentRound].word;
    }

    long timeToNextGame;
    void TimeUntilNextGameRound()
    {
        scrTimer.Activate(timeToNextGame);
    }

    void TimeOver()
    {
        GameInfoSynonym.timeLeft = scrTimer.GetTimeLeft();
        SceneSwitcher.LoadScene2(GameSettings.SYNONYM_SYNONYM);
    }

    public void CallTimeOver()
    {
        TimeOver();
    }

    void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_SYNONYM);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            SceneSwitcher.LoadScene2Back2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
