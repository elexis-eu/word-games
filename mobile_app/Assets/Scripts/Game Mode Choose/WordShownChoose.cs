using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WordShownChoose : MonoBehaviour
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

        if (GameInfoChoose.currentRoundTimeLeft != 0)
        {
            timeToNextGame = GameInfoChoose.currentRoundTimeLeft;
            GameInfoChoose.currentRoundTimeLeft = 0;
        }
        else
        {
            timeToNextGame = GameInfoChoose.info.round_duration_ms;
        }

        GameInfoChoose.SetNewRound();

        scrPrWPre = previewWordObj.GetComponent<PreviewWordPrefab>();

        whatSortText = whatSortObj.GetComponent<Text>();
    }

    // Use this if you need initialization in other scripts to happen first
    void Start()
    {
        TimeUntilNextGameRound();

        GameSettings.MyDebug("Curr round: "+ GameInfoChoose.currentRound);
        if (GameInfoChoose.info.words[GameInfoChoose.currentRound].position == 1)
        {
            scrPrWPre.wordText.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].word + " + ";
            scrPrWPre.leftImageObj.SetActive(false);
        }
        else
        {
            scrPrWPre.wordText.text = " + " + GameInfoChoose.info.words[GameInfoChoose.currentRound].word;
            scrPrWPre.rightImageObj.SetActive(false);
        }

        whatSortText.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].structure_text;
    }

    long timeToNextGame;
    void TimeUntilNextGameRound()
    {
        scrTimer.Activate(timeToNextGame);
    }

    void TimeOver()
    {
        GameInfoChoose.timeLeft = scrTimer.GetTimeLeft();
        SceneSwitcher.LoadScene2(GameSettings.CHOOSE_CHOOSE);
    }

    public void CallTimeOver()
    {
        TimeOver();
    }

    void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_CHOOSE);
        }
    }

    public void SoloButton()
    {
        TimeOver();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
