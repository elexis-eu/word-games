using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoundScoreDrag: MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject soloButtonObj;

    public GameObject sumPointsObj, rightOrderScoreObj;
    private Text sumPointsText, rightOrderScoreText;

    public GameObject[] chosenWordsObj, chosenWordsScoreObj;
    private Text[] chosenWordsText, chosenWordsScoreText;

    public GameObject objSendScore;
    private SendScore scrSendScore;

    public GameObject objGetScoreboard;
    private GetScoreboard scrGetScoreboard;

    public GameObject objConnect;
    private Connect scrConnect;

    private int saveRound;
    private int maxRounds;

    public GameObject objScrollViewTxtWordsObj, contentObj;
    ScrollRect scrollRectWords;
    RectTransform contentTrans;

    public GameObject wordsPrefab;
    private float wordsPrefabHeight;

    public GameObject objProgressCircle;
    public GameObject canvasObj;

    // Use this for initialization
    void Awake()
    {
        canvasObj.SetActive(false);

        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
            soloButtonObj.SetActive(true);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        scrollRectWords = objScrollViewTxtWordsObj.GetComponent<ScrollRect>();
        contentTrans = contentObj.GetComponent<RectTransform>();

        scrSendScore = objSendScore.GetComponent<SendScore>();

        sumPointsText = sumPointsObj.GetComponent<Text>();

        scrConnect = objConnect.GetComponent<Connect>();

        scrGetScoreboard = objGetScoreboard.GetComponent<GetScoreboard>();

        /*rightOrderScoreText = rightOrderScoreObj.GetComponent<Text>();

        chosenWordsText = new Text[3];
        chosenWordsScoreText = new Text[3];
        for (int i = 0; i < 3; i++)
        {
            chosenWordsText[i] = chosenWordsObj[i].GetComponent<Text>();
            chosenWordsScoreText[i] = chosenWordsScoreObj[i].GetComponent<Text>();

            chosenWordsText[i].text = "";
            chosenWordsScoreText[i].text = "";
        }
        
        saveRound = GameInfoChoose.currentRound;
        maxRounds = GameInfoChoose.info.number_of_rounds;*/

        //setUpWords();
    }

    void SetUpScrollViewArea()
    {
        int numOfWords  = GameInfoDrag.chosenButtonsNames.Count;

        contentTrans.sizeDelta = new Vector2(0, numOfWords * wordsPrefabHeight);

        int score = 0;

        for (int i = 0; i < numOfWords; i++)
        {
            GameObject newWordsObj = Instantiate(wordsPrefab, new Vector3(0, -i * wordsPrefabHeight, 0), Quaternion.identity);
            newWordsObj.transform.SetParent(contentTrans, false);
            DragResultPrefab newPlayerScript = newWordsObj.GetComponent<DragResultPrefab>();

            int wordPosition = GameInfoDrag.info.words[GameInfoDrag.currentRound].position;
            //newPlayerScript.wordNameText.text = GameInfoDrag.chosenButtonsNames[i];

            string writeWord = GameInfoDrag.chosenButtonsNames[i];
            if (writeWord.Length == 0)
                writeWord = "/";

            newPlayerScript.wordNameText.text = wordPosition == 2 ? writeWord + " " + GameInfoDrag.info.words[i].word :
                                                          GameInfoDrag.info.words[i].word + " " + writeWord;

            //newPlayerScript.wordScoreText.text = ""+GameInfoDrag.chosenButtonsScores[i] + " " + LanguageText.Translate("score");
            newPlayerScript.wordScoreText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_DRAG_SCORE_POINTS").Replace("{{POINTS}}", GameInfoDrag.chosenButtonsScores[i].ToString());

            score += GameInfoDrag.chosenButtonsScores[i];

            if (GameInfoDrag.chosenButtonsScores[i] == 0)
            {
                newPlayerScript.correctObj.SetActive(false);
                newPlayerScript.incorrectObj.SetActive(true);
            } else
            {
                newPlayerScript.correctObj.SetActive(true);
                newPlayerScript.incorrectObj.SetActive(false);
            }

            if (i == numOfWords-1)
            {
                newPlayerScript.helperLineObj.SetActive(false);
            }
        }

        //sumPointsText.text = score + "/" + GameInfoDrag.info.max_round_score + " " + LanguageText.Translate("score");
        sumPointsText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_DRAG_SCORE_POINTS_MAX").Replace("{{POINTS}}", score.ToString()).Replace("{{MAXPOINTS}}", GameInfoDrag.info.max_round_score.ToString());
    }

    void Start()
    {
        wordsPrefabHeight = wordsPrefab.GetComponent<RectTransform>().rect.height;

        scrTimer.Activate(GameInfoDrag.info.round_pause_duration_ms + GameInfoDrag.timeLeft);
        if (GameInfoDrag.currentRound == (GameInfoDrag.info.number_of_rounds - 1))
        {
            scrTimer.Activate(GameInfoDrag.info.round_pause_duration_ms + GameInfoDrag.info.collecting_results_duration_ms);
        }

        if (GameSettings.THEMATIC)
            scrSendScore.SendSelection1RoundOfDrag(GameSettings.GAME_MODE_THEMATIC);
        else
            scrSendScore.SendSelection1RoundOfDrag(GameSettings.GAME_MODE_DRAG);

        sumPointsText.text = ""+GameInfoDrag.score;
        GameInfoDrag.score = 0;
    }

    /*void setUpWords()
    {
        int score = 0;

        string previewWord = GameInfoDrag.info.words[GameInfoDrag.currentRound].word;

        int currentRound = GameInfoDrag.currentRound;

        int wordPosition = GameInfoDrag.info.words[currentRound].position;

        for (int i = 0; i < GameInfoDrag.chosenButtons.Count; i++)
        {
            chosenWordsText[i].text = wordPosition == 1 ? previewWord + " " + GameInfoDrag.chosenButtons[i].word :
                                                          GameInfoDrag.chosenButtons[i].word + " " + previewWord;
            chosenWordsScoreText[i].text = "" + GameInfoDrag.chosenButtons[i].score + " " + TEXT_TOCK;
            score += GameInfoDrag.chosenButtons[i].score;
        }

        sumPointsText.text = score + " " + TEXT_TOCK;
        rightOrderScoreText.text = "0" + " " + TEXT_TOCK;
    }*/

    private bool callLeaderboard = false;
    private bool dataReceived = false;

    private bool soloButtonPressed = false;
    // Update is called once per frame
    void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            TimeOver();
        }

        if (soloButtonPressed)
        {
            //TimeOver();
        }

        if (scrSendScore.dataReceived && !dataReceived)
        {
            dataReceived = true;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        canvasObj.SetActive(true);
        objProgressCircle.SetActive(false);

        SetUpScrollViewArea();
    }


    public void SoloButton()
    {
        TimeOver();
        soloButtonPressed = true;
    }

    void TimeOver()
    {

            if (!callLeaderboard)
            {
                if (!GameSettings.SOLO)
                    scrGetScoreboard.StartGETQueryDrag();
                scrConnect.ConnectToServer(GameSettings.GAME_MODE_DRAG);

                callLeaderboard = true;
            }
            else
            {
                if (!GameSettings.SOLO && scrGetScoreboard.dataReceived && scrConnect.dataReceived)
                    SceneSwitcher.LoadScene2(GameSettings.SCOREBOARD_DRAG);
                else if (GameSettings.SOLO && scrConnect.dataReceived)
                {
                    SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_DRAG);
                }
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
