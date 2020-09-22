using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class DragDragCol : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject shownWordsObject1, shownWordsObject2;
    Text[] txtWordsShown;

    public GameObject[] shownWordScoreObj;
    Text[] txtWordsShownScore;

    public GameObject btnObject1, btnObject2;
    Text[] btnText;
    DragUIElementCol[] btnScript;

    List<DragWords> userWordsChoices;
    List<int> userChoicesGroup;

    public GameObject objSendScore;
    SendScore sendScore;

    public GameObject doublePointsRound;
    public GameObject bonusStars;
    public GameObject bonusText;

    private float saveY, saveX;

    private Color RED = new Color(244/255f, 67/255f, 54/255f);
    private Color BLUE = new Color(3/255f, 169/255f, 244/255f);

    public GameObject previewWordObjTop, previewWordObjBot;
    private PreviewWordPrefab scrPrWPreTop, scrPrWPreBot;

    private int multiplyscore = 1;

    public GameObject fake0Obj, fake1Obj, fake2Obj, fake3Obj, fake4Obj;

    private float timeLeft;
    private float timeLeftDefault = 0.5f;
    private bool hideBonusStars = false;

    private float timeLeftEnd;
    private float timeLeftEndDefault = 1.0f;
    private bool endGame = false;

    public GameObject structureTextObj;

    void Awake()
    {
        objTimer.SetActive(false);  

        scrTimer = objTimer.GetComponent<TimerUI>();

        scrPrWPreTop = previewWordObjTop.GetComponent<PreviewWordPrefab>();
        scrPrWPreBot = previewWordObjBot.GetComponent<PreviewWordPrefab>();

        txtWordsShown = new Text[2];
        btnScript = new DragUIElementCol[2];
        btnText = new Text[2];
        txtWordsShown[0] = shownWordsObject1.GetComponentInChildren<Text>();
        txtWordsShown[1] = shownWordsObject2.GetComponentInChildren<Text>();

        txtWordsShownScore = new Text[2];
        txtWordsShownScore[0] = shownWordScoreObj[0].GetComponent<Text>();
        txtWordsShownScore[1] = shownWordScoreObj[1].GetComponent<Text>();

        sendScore = objSendScore.GetComponent<SendScore>();

        btnText[0] = btnObject1.GetComponentInChildren<Text>();
        btnText[1] = btnObject2.GetComponentInChildren<Text>();

        btnScript[0] = btnObject1.GetComponent<DragUIElementCol>();
        btnScript[1] = btnObject2.GetComponent<DragUIElementCol>();

        userWordsChoices = new List<DragWords>();
        userChoicesGroup = new List<int>();

        structureTextObj.SetActive(true);
        structureTextObj.GetComponent<Text>().text = GameInfoDrag.info.words[GameInfoDrag.currentRound].structure_text;
    }

    private void Start()
    {
        //scrTimer.Activate(GameInfoDrag.timeLeft);
        ResetBonusStars();

        saveX = shownWordScoreObj[0].transform.position.x;
        saveY = shownWordScoreObj[0].transform.position.y;

        CreateRound();
    }

    public void CreateRound()
    {
        if (GameInfoDrag.info.words[GameInfoDrag.currentRound].position == 2)
        {
            scrPrWPreTop.wordText.text = GameInfoDrag.info.words[GameInfoDrag.currentRound].buttons[0].word + " + ";
            scrPrWPreTop.leftImageObj.SetActive(false);
            scrPrWPreBot.wordText.text = GameInfoDrag.info.words[GameInfoDrag.currentRound].buttons[1].word + " + ";
            scrPrWPreBot.leftImageObj.SetActive(false);
        }
        else
        {
            scrPrWPreTop.wordText.text = " + " + GameInfoDrag.info.words[GameInfoDrag.currentRound].buttons[0].word;
            scrPrWPreTop.rightImageObj.SetActive(false);
            scrPrWPreBot.wordText.text = " + " + GameInfoDrag.info.words[GameInfoDrag.currentRound].buttons[1].word;
            scrPrWPreBot.rightImageObj.SetActive(false);
        }

        //txtWordsShown[0].text = GameInfoDrag.info.words[userWordsChoices.Count].buttons[0].word;
        //txtWordsShown[1].text = GameInfoDrag.info.words[userWordsChoices.Count].buttons[1].word;

        CreateWordButton();
    }

    void CreateWordButton()
    {
        if (userWordsChoices.Count == GameInfoDrag.info.words.Length)
        {
            //GameOver();
            timeLeftEnd = timeLeftEndDefault;
            endGame = true;
            return;
        }

        if (userWordsChoices.Count == GameInfoDrag.info.double_points_round)
        {
            doublePointsRound.SetActive(true);
        }
        else
        {
            doublePointsRound.SetActive(false);
        }

        btnText[userWordsChoices.Count % 2].text = GameInfoDrag.info.words[userWordsChoices.Count].word;
        btnScript[userWordsChoices.Count % 2].CallInvoke();
    }

    public void GameOver()
    {
        //SendScoreToServer();
        GameInfoDrag.timeLeft = scrTimer.GetTimeLeft();
        GameInfoDrag.chosenButtonsGroups = userChoicesGroup;
        SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_DRAG_COL);
    }

    public void SendScoreToServer()
    {
        //SendScore.SendSelection1RoundOfDrag();
    }

    public void WordBelongsToUserChoice(int belongs)
    {
        ShowScore(belongs);

        userWordsChoices.Add(GameInfoDrag.info.words[userWordsChoices.Count]);
        userChoicesGroup.Add(belongs);

        CreateWordButton();
    }

    private float screenHeight0 = Screen.height / 1.8f;
    private float screenHeight1 = Screen.height / 3.1f;
    private float screenWidth = Screen.width / 1.2f;
    private float screenHeight2 = Screen.height / 2.8f;
    void ShowScore(int belongs)
    {

        if (userWordsChoices.Count == GameInfoDrag.info.double_points_round)
        {
            multiplyscore = 2;
        } else {
            multiplyscore = 1;
        }

        GameInfoDrag.chosenButtonsNames.Add(GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].word);
        GameInfoDrag.chosenButtonsScores.Add(GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score * multiplyscore);
        GameInfoDrag.chosenButtonsBonus.Add(multiplyscore);

        SendSetWeight(GameInfoDrag.info.words[userWordsChoices.Count].word, GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].word, GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].collocation_id, GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score * multiplyscore);

        txtWordsShownScore[userWordsChoices.Count % 2].text = "+" + GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score * multiplyscore;

        shownWordScoreObj[userWordsChoices.Count % 2].SetActive(true);

        txtWordsShownScore[userWordsChoices.Count % 2].CrossFadeAlpha(1.0f, 0f, false);

        GameInfoDrag.score += GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score * multiplyscore;

        if (GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score > 0)
        {
            txtWordsShownScore[userWordsChoices.Count % 2].color = BLUE;
        }
        else
        {
            txtWordsShownScore[userWordsChoices.Count % 2].color = RED;
        }

        if (GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].word != "")
        {
            if (GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score >= GameInfoDrag.info.scoring[0])
            {
                GameInfoDrag.correct_subsequently++;

                if (GameInfoDrag.correct_subsequently > GameInfoDrag.correct_subsequently_max)
                {
                    GameInfoDrag.correct_subsequently_max = GameInfoDrag.correct_subsequently;
                }
            }
            else
            {
                if (GameInfoDrag.correct_subsequently > GameInfoDrag.correct_subsequently_max)
                {
                    GameInfoDrag.correct_subsequently_max = GameInfoDrag.correct_subsequently;
                }

                GameInfoDrag.correct_subsequently = 0;

                ResetBonusStars();
            }
        }


        if (belongs == 0)
        {
            shownWordScoreObj[userWordsChoices.Count % 2].transform.position = new Vector3(fake1Obj.transform.position.x, fake1Obj.transform.position.y, 0);
        }
        else if (belongs == 1)
        {
            shownWordScoreObj[userWordsChoices.Count % 2].transform.position = new Vector3(fake2Obj.transform.position.x, fake2Obj.transform.position.y, 0);
        }
        else if (belongs == 2)
        {
            shownWordScoreObj[userWordsChoices.Count % 2].transform.position = new Vector3(fake0Obj.transform.position.x, fake0Obj.transform.position.y, 0);
        }


        if (GameInfoDrag.correct_subsequently_max == GameInfoDrag.info.bonus_condition && hideBonusStars == false)
        {
            txtWordsShownScore[userWordsChoices.Count % 2].text = "+" + (GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score * multiplyscore).ToString();

            bonusStars.SetActive(false);
            bonusText.SetActive(true);
            bonusText.GetComponent<Text>().text = "+ " + GameInfoDrag.info.bonus_condition_points;

            hideBonusStars = true;
            timeLeft = timeLeftDefault;
        } else {
            if (GameInfoDrag.correct_subsequently_max > GameInfoDrag.info.bonus_condition)
            {
                bonusStars.SetActive(false);
                bonusText.SetActive(false);
            }

            if (GameInfoDrag.correct_subsequently < GameInfoDrag.info.bonus_condition)
            {
                if (GameInfoDrag.correct_subsequently == 1)
                {
                    GameObject BonusStar = GetChildWithName(bonusStars, "1Star");
                    BonusStar.GetComponent<Image>().color = GameSettings.COLOR_GREEN;
                }

                if (GameInfoDrag.correct_subsequently == 2)
                {
                    GameObject BonusStar = GetChildWithName(bonusStars, "2Star");
                    BonusStar.GetComponent<Image>().color = GameSettings.COLOR_GREEN;
                }

                if (GameInfoDrag.correct_subsequently == 3)
                {
                    GameObject BonusStar = GetChildWithName(bonusStars, "3Star");
                    BonusStar.GetComponent<Image>().color = GameSettings.COLOR_GREEN;
                }

                if (GameInfoDrag.correct_subsequently == 4)
                {
                    GameObject BonusStar = GetChildWithName(bonusStars, "4Star");
                    BonusStar.GetComponent<Image>().color = GameSettings.COLOR_GREEN;
                }

                if (GameInfoDrag.correct_subsequently == 5)
                {
                    GameObject BonusStar = GetChildWithName(bonusStars, "5Star");
                    BonusStar.GetComponent<Image>().color = GameSettings.COLOR_GREEN;
                }
            }
        }

        txtWordsShownScore[userWordsChoices.Count % 2].CrossFadeAlpha(0.0f, 1f, false);
    }

    private void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            //GameOver();
        }
        
        if (hideBonusStars) {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                bonusText.SetActive(false);
            }
        }

        if (endGame)
        {
            timeLeftEnd -= Time.deltaTime;
            if (timeLeftEnd < 0)
            {
                GameOver();
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            //SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }

    public void SendSetWeight(string word_shown, string word_selected, int collocation_id, int score)
    {
        string url = GameSettings.POSTCollocationDragLogURL + GameSettings.GetUserFBToken() + "&level_id=" + GameInfoCollocation.info.currentLevel + "&type=" + GameInfoCollocation.info.gameMode+"&game_id=" + GameInfoCollocation.currentGame.currentCollocationLevelID;

        GameSettings.MyDebug(url);

        ColSetWeightRequest postData = new ColSetWeightRequest();

        postData.collocation_id = collocation_id;
        postData.game_mode = "drag";
        postData.word_shown = word_shown;
        postData.word_selected = word_selected;
        postData.score = score;

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
            //ColSetWeightResponseMessage = JsonUtility.FromJson<ColSetWeightResponse>(uwr.downloadHandler.text);

        }
    }

    [System.Serializable]
    class ColSetWeightRequest
    {
        public int collocation_id;
        public string game_mode;
        public string word_shown;
        public string word_selected;
        public int score;
    }

    [System.Serializable]
    class ColSetWeightResponse
    {
        public string message;
    }

    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }

    void ResetBonusStars() {
        GameObject BonusStar1 = GetChildWithName(bonusStars, "1Star");
        BonusStar1.GetComponent<Image>().color = GameSettings.COLOR_GRAY;

        GameObject BonusStar2 = GetChildWithName(bonusStars, "2Star");
        BonusStar2.GetComponent<Image>().color = GameSettings.COLOR_GRAY;

       GameObject BonusStar3 = GetChildWithName(bonusStars, "3Star");
       BonusStar3.GetComponent<Image>().color = GameSettings.COLOR_GRAY;
        
       GameObject BonusStar4 = GetChildWithName(bonusStars, "4Star");
       BonusStar4.GetComponent<Image>().color = GameSettings.COLOR_GRAY;

        GameObject BonusStar5 = GetChildWithName(bonusStars, "5Star");
        BonusStar5.GetComponent<Image>().color = GameSettings.COLOR_GRAY;
    }
}
