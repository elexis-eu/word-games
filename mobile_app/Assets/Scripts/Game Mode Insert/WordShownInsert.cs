using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WordShownInsert : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject previewWordObj;
    private PreviewWordPrefab scrPrWPre;

    public GameObject whatSortObj;
    private Text whatSortText;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        if (GameInfoInsert.currentRoundTimeLeft != 0)
        {
            timeToNextGame = GameInfoInsert.currentRoundTimeLeft;
            GameInfoInsert.currentRoundTimeLeft = 0;
        }
        else
        {
            timeToNextGame = GameInfoInsert.info.round_duration_ms;
        }

        GameInfoInsert.SetNewRound();

        scrPrWPre = previewWordObj.GetComponent<PreviewWordPrefab>();

        whatSortText = whatSortObj.GetComponent<Text>();
    }

    // Use this if you need initialization in other scripts to happen first
    void Start()
    {
        TimeUntilNextGameRound();

        if (GameInfoInsert.info.words[GameInfoInsert.currentRound].position == 1)
        {
            scrPrWPre.wordText.text = GameInfoInsert.info.words[GameInfoInsert.currentRound].word + " + ";
            scrPrWPre.leftImageObj.SetActive(false);
        }
        else
        {
            scrPrWPre.wordText.text = " + " + GameInfoInsert.info.words[GameInfoInsert.currentRound].word;
            scrPrWPre.rightImageObj.SetActive(false);
        }

        whatSortText.text = GameInfoInsert.info.words[GameInfoInsert.currentRound].structure_text;
    }

    long timeToNextGame;
    void TimeUntilNextGameRound()
    {
        scrTimer.Activate(timeToNextGame);
    }

    void TimeOver()
    {
        GameInfoInsert.timeLeft = scrTimer.GetTimeLeft();
        SceneSwitcher.LoadScene2(GameSettings.INSERT_INSERT);
    }

    public void CallTimeOver()
    {
        TimeOver();
    }

    void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_INSERT);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
