using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RoundScoreInsert : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject soloButtonObj;
    private Text soloText;

    public GameObject canvasObj;

    public GameObject sumPointsObj;
    private Text sumPointsText;

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

    public GameObject objProgressCircle;

    public GameObject[] starsObj;
    private StarsPrefab[] starsScripts;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
            soloButtonObj.SetActive(true);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        starsScripts = new StarsPrefab[starsObj.Length];
        for (int i = 0; i < starsObj.Length; i++)
        {
            starsScripts[i] = starsObj[i].GetComponent<StarsPrefab>();
        }

        sumPointsText = sumPointsObj.GetComponent<Text>();

        soloText = soloButtonObj.GetComponentInChildren<Text>();

        chosenWordsText = new Text[3];
        chosenWordsScoreText = new Text[3];
        for (int i = 0; i < 3; i++)
        {
            chosenWordsText[i] = chosenWordsObj[i].GetComponent<Text>();
            chosenWordsScoreText[i] = chosenWordsScoreObj[i].GetComponent<Text>();

            chosenWordsText[i].text = "";
            chosenWordsScoreText[i].text = "";
        }

        scrSendScore = objSendScore.GetComponent<SendScore>();

        scrGetScoreboard = objGetScoreboard.GetComponent<GetScoreboard>();

        scrConnect = objConnect.GetComponent<Connect>();

        saveRound = GameInfoInsert.currentRound;
        maxRounds = GameInfoInsert.info.number_of_rounds;

        canvasObj.SetActive(false);
        objProgressCircle.SetActive(true);

        setUpWords();
    }

    void Start()
    {
        scrTimer.Activate(GameInfoInsert.info.round_pause_duration_ms);
        if (GameInfoInsert.currentRound == (GameInfoInsert.info.number_of_rounds - 1))
        {
            scrTimer.Activate(GameInfoInsert.info.round_pause_duration_ms + GameInfoInsert.info.collecting_results_duration_ms);
        }

        if (GameSettings.THEMATIC)
        {
            scrSendScore.SendSelection1RoundOfInsert(GameSettings.GAME_MODE_THEMATIC);
            if (saveRound == (maxRounds - 1))
            {

            }
        }
        else
        {
            scrSendScore.SendSelection1RoundOfInsert(GameSettings.GAME_MODE_INSERT);
        }

        if (saveRound == (maxRounds - 1))
        {
            //soloText.text = "Zaključi";
            soloText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_INSERT_SCORE_END_BUTTON");
        }
    }

    void setUpWords()
    {
        //int score = 0;

        for (int i = 0; i < GameInfoInsert.chosenWords.Length; i++)
        {
            chosenWordsText[i].text = GameInfoInsert.chosenWords[i];
            chosenWordsScoreText[i].text = "";
            //score += GameInfoInsert.chosenWords[i].score;
        }

        //sumPointsText.text = score + " " + TEXT_TOCK;
        //sumPointsText.text = ""+GameInfoInsert.score;
    }

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

        if (scrSendScore.dataReceived && !dataReceived)
        {
            dataReceived = true;
            UpdateUI();
        }

        if (soloButtonPressed)
        {
            //TimeOver();
        }
    }

    public void SoloButton()
    {
        TimeOver();
        soloButtonPressed = true;
    }

    public void QuitAndCheck()
    {
        if (saveRound == (maxRounds - 1))
        {
            if (GameSettings.email == null || GameSettings.email.Length == 0)
            {
                SceneSwitcher.LoadScene2(GameSettings.SEND_EMAIL);
            }
            else
            {
                SceneSwitcher.LoadScene2(GameSettings.THEMATIC_LEADERBOARDS);
            }
        }
    }

    public void OnApplicationQuit()
    {
        
    }

    void AmountOfStars()
    {

    }

    void UpdateUI()
    {
        canvasObj.SetActive(true);

        int score = 0;

        string previewWord = GameInfoInsert.info.words[GameInfoInsert.currentRound].word;
        int wordPosition = GameInfoInsert.info.words[GameInfoInsert.currentRound].position;

        sumPointsText.text = "" + GameInfoInsert.score;
        objProgressCircle.SetActive(false);
        sumPointsObj.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            string writeWord = GameInfoInsert.rec.data[0].scores[i].word;
            if (writeWord.Length == 0)
                writeWord = "/";

            chosenWordsText[i].text = wordPosition == 1 ? previewWord + " " + writeWord :
                                                          writeWord + " " + previewWord;

            //chosenWordsText[i].text = GameInfoInsert.rec.data[0].scores[i].word;
            //chosenWordsScoreText[i].text = ""+GameInfoInsert.rec.data[0].scores[i].score + " " + LanguageText.Translate("score");
            chosenWordsScoreText[i].text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_INSERT_SCORE_POINTS").Replace("{{POINTS}}", GameInfoInsert.rec.data[0].scores[i].score.ToString());

            score += GameInfoInsert.rec.data[0].scores[i].score;

            if (chosenWordsText[i].text.Length == 0)
                chosenWordsText[i].text = "/";

            int pos = Array.IndexOf(GameInfoInsert.info.scoring, GameInfoInsert.rec.data[0].scores[i].score);
            if (pos == 4)
                pos = 5;
            starsScripts[i].EnableStarsObjects(5, GameInfoInsert.info.scoring.Length - pos);
        }

        //sumPointsText.text = score + "/" + GameInfoInsert.info.max_round_score + " " + LanguageText.Translate("score");
        sumPointsText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_INSERT_SCORE_POINTS_MAX").Replace("{{POINTS}}", score.ToString()).Replace("{{MAXPOINTS}}", GameInfoInsert.info.max_round_score.ToString());
    }

    void TimeOver()
    {
        if (!scrSendScore.dataReceived)
        {
            SceneSwitcher.LoadScene2Back2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
            return;
        }

        Debug.Log("DESERVE: " + GameSettings.THEMATIC + " and " + GameSettings.email);
        if (saveRound == (maxRounds - 1))
        {
            if (GameSettings.THEMATIC)
            {
                // Scene switch to global leaderboard;
                if (GameSettings.email == null || GameSettings.email.Length == 0)
                {
                    SceneSwitcher.LoadScene2(GameSettings.SEND_EMAIL);
                }
                else
                {
                    SceneSwitcher.LoadScene2(GameSettings.THEMATIC_LEADERBOARDS);
                }
                return;
            }

            if (!callLeaderboard)
            {
                if (!GameSettings.SOLO)
                    scrGetScoreboard.StartGETQueryInsert();
                scrConnect.ConnectToServer(GameSettings.GAME_MODE_INSERT);

                callLeaderboard = true;
            }
            else
            {
                if (!GameSettings.SOLO && scrGetScoreboard.dataReceived && scrConnect.dataReceived)
                {
                    SceneSwitcher.LoadScene2(GameSettings.SCOREBOARD_INSERT);
                    return;
                }
                else if (GameSettings.SOLO && scrConnect.dataReceived)
                {
                    SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_INSERT);
                    return;
                }
            }
        }
        else
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_INSERT);
            return;
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
