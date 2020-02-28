using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RoundScoreDragCol: MonoBehaviour
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

    private LevelSaveScoreResponse LevelSaveScoreResponseMessage;
    private int sum_score;

    // Use this for initialization
    void Awake()
    {
        canvasObj.SetActive(false);

        objTimer.SetActive(false);
        soloButtonObj.SetActive(true);

        scrTimer = objTimer.GetComponent<TimerUI>();

        scrollRectWords = objScrollViewTxtWordsObj.GetComponent<ScrollRect>();
        contentTrans = contentObj.GetComponent<RectTransform>();

        //scrSendScore = objSendScore.GetComponent<SendScore>();

        scrSendScore = new SendScore();

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

        contentTrans.sizeDelta = new Vector2(0, (numOfWords+1) * wordsPrefabHeight);

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
            sum_score = score;

            if (GameInfoDrag.chosenButtonsScores[i] == 0)
            {
                newPlayerScript.correctObj.SetActive(false);
                newPlayerScript.incorrectObj.SetActive(true);
            } else
            {
                newPlayerScript.correctObj.SetActive(true);
                newPlayerScript.incorrectObj.SetActive(false);
            }

            if (i == numOfWords-1 && GameInfoDrag.correct_subsequently_max < GameInfoDrag.info.bonus_condition)
            {
                newPlayerScript.helperLineObj.SetActive(false);
            }
        }

        if (GameInfoDrag.correct_subsequently_max > GameInfoDrag.info.bonus_condition)
        {
            score += GameInfoDrag.info.bonus_condition_points;
            sum_score = score;

            GameObject newWordsObj = Instantiate(wordsPrefab, new Vector3(0, -numOfWords * wordsPrefabHeight, 0), Quaternion.identity);
            newWordsObj.transform.SetParent(contentTrans, false);
            DragResultPrefab newPlayerScript = newWordsObj.GetComponent<DragResultPrefab>();

            //newPlayerScript.wordNameText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_DRAG_BONUS_POINTS_TEXT");
            newPlayerScript.wordNameText.text = "BONUS";

            //newPlayerScript.wordScoreText.text = ""+GameInfoDrag.chosenButtonsScores[i] + " " + LanguageText.Translate("score");
            newPlayerScript.wordScoreText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_DRAG_SCORE_POINTS").Replace("{{POINTS}}", GameInfoDrag.info.bonus_condition_points.ToString());
            newPlayerScript.correctObj.SetActive(true);
            newPlayerScript.incorrectObj.SetActive(false);
            newPlayerScript.helperLineObj.SetActive(false);
        }


        //sumPointsText.text = score + "/" + GameInfoDrag.info.max_round_score + " " + LanguageText.Translate("score");
        sumPointsText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_DRAG_SCORE_POINTS_MAX").Replace("{{POINTS}}", score.ToString()).Replace("{{MAXPOINTS}}", (GameInfoDrag.info.max_round_score + GameInfoDrag.info.bonus_condition_points).ToString());
    }

    void Start()
    {
        wordsPrefabHeight = wordsPrefab.GetComponent<RectTransform>().rect.height;

        /*
        if (GameSettings.THEMATIC)
            scrSendScore.SendSelection1RoundOfDrag(GameSettings.GAME_MODE_THEMATIC);
        else
            scrSendScore.SendSelection1RoundOfDrag(GameSettings.GAME_MODE_DRAG);
        */

        sumPointsText.text = ""+GameInfoDrag.score;
        GameInfoDrag.score = 0;

        UpdateUI();
        SaveScores();
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
        if (soloButtonPressed)
        {
            TimeOver();
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
        SceneSwitcher.LoadScene2(GameSettings.SCENE_ROOM_PICKER_COL);
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
