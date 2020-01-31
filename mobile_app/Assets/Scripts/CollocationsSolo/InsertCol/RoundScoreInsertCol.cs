using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class RoundScoreInsertCol : MonoBehaviour
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

    private LevelCheckResponse LevelCheckResponseMessage;

    // Use this for initialization
    void Awake()
    {
        objTimer.SetActive(false);
        soloButtonObj.SetActive(true);

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

        CheckScores();
    }

    void Start()
    {
        //soloText.text = "Zaključi";
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
        if (soloButtonPressed)
        {
            TimeOver();
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

        int score = LevelCheckResponseMessage.score1 + LevelCheckResponseMessage.score2 + LevelCheckResponseMessage.score3;

        sumPointsText.text = "" + GameInfoInsert.score;
        objProgressCircle.SetActive(false);
        sumPointsObj.SetActive(true);

        SetScoreForPosition(0, LevelCheckResponseMessage.score1);
        SetScoreForPosition(1, LevelCheckResponseMessage.score2);
        SetScoreForPosition(2, LevelCheckResponseMessage.score3);

        //sumPointsText.text = score + "/" + GameInfoInsert.info.max_round_score + " " + LanguageText.Translate("score");
        sumPointsText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_INSERT_SCORE_POINTS_MAX").Replace("{{POINTS}}", score.ToString()).Replace("{{MAXPOINTS}}", GameInfoInsert.info.max_round_score.ToString());
    }

    void SetScoreForPosition(int i, int score) {

        string previewWord = GameInfoInsert.info.words[GameInfoInsert.currentRound].word;
        int wordPosition = GameInfoInsert.info.words[GameInfoInsert.currentRound].position;

        string writeWord = GameInfoInsert.chosenWords[i];

        if (writeWord.Length == 0)
            writeWord = "/";

        chosenWordsText[i].text = wordPosition == 1 ? previewWord + " " + writeWord :
                                                      writeWord + " " + previewWord;

        //chosenWordsText[i].text = GameInfoInsert.rec.data[0].scores[i].word;
        //chosenWordsScoreText[i].text = "" + score + " " + LanguageText.Translate("score");
        chosenWordsScoreText[i].text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_INSERT_SCORE_POINTS").Replace("{{POINTS}}", score.ToString());

        if (chosenWordsText[i].text.Length == 0)
            chosenWordsText[i].text = "/";

        int pos = Array.IndexOf(GameInfoInsert.info.scoring, score);
        if (pos == 4)
            pos = 5;
        starsScripts[i].EnableStarsObjects(5, GameInfoInsert.info.scoring.Length - pos);
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

    public void CheckScores()
    {
        string url = GameSettings.POSTLevelCheckInsertSoloURL + GameSettings.GetUserFBToken() + "&level_id=" + GameInfoCollocation.info.currentLevel + "&type=" + GameInfoCollocation.info.gameMode;

        GameSettings.MyDebug(url);

        LevelCheckRequest postData = new LevelCheckRequest();

        postData.collocationLevelID = GameInfoCollocation.currentGame.currentCollocationLevelID;
        postData.word1Text = GameInfoInsert.chosenWords[0];
        postData.word2Text = GameInfoInsert.chosenWords[1];
        postData.word3Text = GameInfoInsert.chosenWords[2];

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
            LevelCheckResponseMessage = JsonUtility.FromJson<LevelCheckResponse>(uwr.downloadHandler.text);

            UpdateUI();
        }
    }

    [System.Serializable]
    class LevelCheckRequest
    {
        public int collocationLevelID;
        public string word1Text;
        public string word2Text;
        public string word3Text;
    }

    [System.Serializable]
    class LevelCheckResponse
    {
        public int score1;
        public int score2;
        public int score3;
        public string suggestion;
    }
}
