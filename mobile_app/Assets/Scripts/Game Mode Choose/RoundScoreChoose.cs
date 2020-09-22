using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class RoundScoreChoose : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject soloButtonObj;
    private Text soloButtonText;

    public GameObject sumPointsObj, rightOrderScoreObj, rightOrderTextObj;
    private Text sumPointsText, rightOrderScoreText, rightOrderText;

    public GameObject[] chosenWordsObj, chosenWordsScoreObj;
    private Text[] chosenWordsText, chosenWordsScoreText;

    //private readonly string TEXT_NEXT = "naprej";
    //private readonly string TEXT_END = "končaj";

    public GameObject objSendScore;
    private SendScore scrSendScore;

    public GameObject objGetScoreboard;
    private GetScoreboard scrGetScoreboard;

    public GameObject objConnect;
    private Connect scrConnect;

    private int saveRound;
    private int maxRounds;

    public GameObject[] starsObj;
    private StarsPrefab[] starsScripts;

    public GameObject canvasObj;

    public GameObject objProgressCircle;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
            soloButtonObj.SetActive(true);
            soloButtonText = soloButtonObj.GetComponentInChildren<Text>();

            /*soloButtonText.text = TEXT_NEXT;
            if (GameInfoChoose.currentRound == (GameInfoChoose.info.number_of_rounds - 1))
            {
                soloButtonText.text = TEXT_END;
            }*/
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        starsScripts = new StarsPrefab[starsObj.Length];
        for (int i = 0; i < starsObj.Length; i++)
        {
            starsScripts[i] = starsObj[i].GetComponent<StarsPrefab>();
        }

        sumPointsText = sumPointsObj.GetComponent<Text>();
        rightOrderScoreText = rightOrderScoreObj.GetComponent<Text>();
        rightOrderText = rightOrderTextObj.GetComponent<Text>();

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

        saveRound = GameInfoChoose.currentRound;
        maxRounds = GameInfoChoose.info.number_of_rounds;

        canvasObj.SetActive(false);
        objProgressCircle.SetActive(true);
    }

    void Start()
    {
        scrTimer.Activate(GameInfoChoose.info.round_pause_duration_ms);
        if (GameInfoChoose.currentRound == (GameInfoChoose.info.number_of_rounds - 1))
        {
            scrTimer.Activate(GameInfoChoose.info.round_pause_duration_ms + GameInfoChoose.info.collecting_results_duration_ms);
        }

        if (GameSettings.THEMATIC)
        {
            scrSendScore.SendSelection1RoundOfChoose(GameSettings.GAME_MODE_THEMATIC);
            if (saveRound == (maxRounds - 1))
            {
   
            }
        }
        else
        {
            scrSendScore.SendSelection1RoundOfChoose(GameSettings.GAME_MODE_CHOOSE);
        }

        if (saveRound == (maxRounds - 1))
        {
            //soloButtonText.text = "Zaključi";
            soloButtonText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_CHOOSE_SCORE_END_BUTTON");
        }
    }

    void setUpWords()
    {
        int score = 0;

        string previewWord = GameInfoChoose.info.words[GameInfoChoose.currentRound].word;

        int currentRound = GameInfoChoose.currentRound;

        int wordPosition = GameInfoChoose.info.words[currentRound].position;

        for (int i = 0; i < 3; i++)
        {
            if (i >= GameInfoChoose.chosenButtons.Count)
            {
                chosenWordsText[i].text = "/";
                //chosenWordsScoreText[i].text = "0" + " " + LanguageText.Translate("score");
                chosenWordsScoreText[i].text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_CHOOSE_SCORE_POINTS").Replace("{{POINTS}}", "0");
                starsScripts[i].EnableStarsObjects(5, 0);
                continue;
            }


            chosenWordsText[i].text = wordPosition == 1 ? previewWord + " " + GameInfoChoose.chosenButtons[i].word:
                                                          GameInfoChoose.chosenButtons[i].word + " " + previewWord;
            //chosenWordsScoreText[i].text = ""+GameInfoChoose.chosenButtons[i].score + " " + LanguageText.Translate("score");
            chosenWordsScoreText[i].text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_CHOOSE_SCORE_POINTS").Replace("{{POINTS}}", GameInfoChoose.chosenButtons[i].score.ToString());
            score += GameInfoChoose.chosenButtons[i].score;

            int pos = Array.IndexOf(GameInfoChoose.info.scoring, GameInfoChoose.chosenButtons[i].score);
            starsScripts[i].EnableStarsObjects(5, 5-(2*pos));
        }


        int bonusPoints = GameInfoChoose.info.bonus_points;
        int biggestNumber = -1;
        for (int i = 0; i < GameInfoChoose.classyfiedBtns.Count; i++)
        {
            if (GameInfoChoose.classyfiedBtns[i].choose_position <= biggestNumber)
            {
                bonusPoints = 0;
                break;
            }

            biggestNumber = GameInfoChoose.classyfiedBtns[i].choose_position;
        }

        if (GameInfoChoose.classyfiedBtns.Count != 3)
        {
            bonusPoints = 0;
        }

        if (bonusPoints != 0)
        {
            starsScripts[3].EnableStarsObjects(3, 3);
            //rightOrderText.text = "Pravilna razvrstitev!";
            rightOrderText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_CHOOSE_SCORE_CORRECT_ORDER_TEXT");
        } else
        {
            starsScripts[3].EnableStarsObjects(3, 0);
            //rightOrderText.text = "Napačna razvrstitev!";
            rightOrderText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_CHOOSE_SCORE_WRONG_ORDER_TEXT");
        }

        score += bonusPoints;
        //sumPointsText.text = score + "/" + GameInfoChoose.info.max_round_score + " " + LanguageText.Translate("score");
        sumPointsText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_INSERT_SCORE_POINTS_MAX").Replace("{{POINTS}}", score.ToString()).Replace("{{MAXPOINTS}}", GameInfoChoose.info.max_round_score.ToString());

        //rightOrderScoreText.text = bonusPoints + " " + LanguageText.Translate("score");
        rightOrderScoreText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_CHOOSE_SCORE_POINTS").Replace("{{POINTS}}", bonusPoints.ToString());
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
            return;
        }

        if (soloButtonPressed)
        {
            //TimeOver();
            //return;
        }

        // fake additional time, to make sure people can't cheat
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

        setUpWords();
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
                } else
                {
                    SceneSwitcher.LoadScene2(GameSettings.THEMATIC_LEADERBOARDS);
                }
                
                return;
            }

            if (!callLeaderboard)
            {
                if (!GameSettings.SOLO)
                    scrGetScoreboard.StartGETQueryChoose();
                scrConnect.ConnectToServer(GameSettings.GAME_MODE_CHOOSE);
                callLeaderboard = true;
            } else
            {
                if (!GameSettings.SOLO && scrGetScoreboard.dataReceived && scrConnect.dataReceived)
                {
                    SceneSwitcher.LoadScene2(GameSettings.SCOREBOARD_CHOOSE);
                    return;
                }
                else if (GameSettings.SOLO && scrConnect.dataReceived)
                {
                    SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_CHOOSE);
                    return;
                }
            }
        } else
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_CHOOSE);
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
