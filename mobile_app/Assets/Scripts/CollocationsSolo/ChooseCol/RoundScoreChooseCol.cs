using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class RoundScoreChooseCol : MonoBehaviour
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

    private LevelSaveScoreResponse LevelSaveScoreResponseMessage;
    private int sum_score;

    // Use this for initialization
    void Awake()
    {
        objTimer.SetActive(false);
        soloButtonObj.SetActive(true);
        soloButtonText = soloButtonObj.GetComponentInChildren<Text>();

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
        
        //soloButtonText.text = "Zaključi";

        UpdateUI();
        SaveScores();
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
                chosenWordsScoreText[i].text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CHOOSE_SCORE_POINTS").Replace("{{POINTS}}", "0");
                starsScripts[i].EnableStarsObjects(5, 0);
                continue;
            }


            chosenWordsText[i].text = wordPosition == 1 ? previewWord + " " + GameInfoChoose.chosenButtons[i].word:
                                                          GameInfoChoose.chosenButtons[i].word + " " + previewWord;
            //chosenWordsScoreText[i].text = ""+GameInfoChoose.chosenButtons[i].score + " " + LanguageText.Translate("score");
            chosenWordsScoreText[i].text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CHOOSE_SCORE_POINTS").Replace("{{POINTS}}", GameInfoChoose.chosenButtons[i].score.ToString());
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
            rightOrderText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CHOOSE_SCORE_CORRECT_ORDER_TEXT");
        } else
        {
            starsScripts[3].EnableStarsObjects(3, 0);            
            //rightOrderText.text = "Napačna razvrstitev!";
            rightOrderText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CHOOSE_SCORE_WRONG_ORDER_TEXT");
        }

        score += bonusPoints;
        sum_score = score;
        //sumPointsText.text = score + "/" + GameInfoChoose.info.max_round_score + " " + LanguageText.Translate("score");
        sumPointsText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CHOOSE_SCORE_POINTS_MAX").Replace("{{POINTS}}", score.ToString()).Replace("{{MAXPOINTS}}", GameInfoChoose.info.max_round_score.ToString());

        //rightOrderScoreText.text = bonusPoints + " " + LanguageText.Translate("score");
        rightOrderScoreText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CHOOSE_SCORE_POINTS").Replace("{{POINTS}}", bonusPoints.ToString());
    }

    private bool callLeaderboard = false;
    private bool dataReceived = false;

    private bool soloButtonPressed = false;
    // Update is called once per frame
    void Update()
    {
        if (soloButtonPressed)
        {
            TimeOver();
            return;
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
        /*
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
        */
    }

    void TimeOver()
    {
      SceneSwitcher.LoadScene2(GameSettings.SCENE_ROOM_PICKER_COL);
      return;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            //SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }

    public void SaveScores()
    {
        string url = GameSettings.POSTLevelSaveScoreDragSoloURL + GameSettings.GetUserFBToken() + "&level_id=" + GameInfoCollocation.info.currentLevel + "&type=" + GameInfoCollocation.info.gameMode;

        GameSettings.MyDebug(url);

        LevelSaveScoreRequest postData = new LevelSaveScoreRequest();

        postData.collocationLevelID = GameInfoCollocation.currentGame.currentCollocationLevelID;
        postData.score = sum_score;

        string json = JsonUtility.ToJson(postData);

        StartCoroutine(PostRequest(url, json));
    }

    IEnumerator PostRequest(string url, string json)
    {

        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        uwr.SetRequestHeader("Content-Type", "application/json");

        uwr.timeout = 5;
        yield return uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
        {
            GameSettings.MyDebug("Error While Sending: " + uwr.error);
            //SceneSwitcher.LoadScene2(GameSettings.MENU_LEADERBOARDS);
        }
        else
        {
            GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);
            LevelSaveScoreResponseMessage = JsonUtility.FromJson<LevelSaveScoreResponse>(uwr.downloadHandler.text);

        }
    }

    [System.Serializable]
    class LevelSaveScoreRequest
    {
        public int collocationLevelID;
        public int score;
    }

    [System.Serializable]
    class LevelSaveScoreResponse
    {
        public int score;
        public int collocationLevelID;
    }
}
