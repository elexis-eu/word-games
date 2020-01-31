using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WordShownChooseSolo : MonoBehaviour
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
        objTimer.SetActive(false);

        GameInfoChoose.SetRoundStartInfo(GameInfoCollocation.currentGame.currentGameData, false);

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

        GameInfoChoose.currentRound = 0;

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

    }

    void TimeOver()
    {
        SceneSwitcher.LoadScene2(GameSettings.SCENE_CHOOSE_COL);
    }

    public void CallTimeOver()
    {
        TimeOver();
    }

    void Update()
    {
    }

    public void SoloButton()
    {
        TimeOver();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            //SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
