using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ChooseChooseCol : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject choice1Obj, choice2Obj, choice3Obj;
    private Text choice1Text, choice2Text, choice3Text;

    public GameObject previewWordObj;
    private PreviewWordPrefab scrPrWPre;

    private int roundOfRound;

    private ChooseInfo info;

    public GameObject structureTextObj;

    // Use this for initialization
    void Awake()
    {

        objTimer.SetActive(false);

        scrTimer = objTimer.GetComponent<TimerUI>();

        scrPrWPre = previewWordObj.GetComponent<PreviewWordPrefab>();

        choice1Text = choice1Obj.GetComponentInChildren<Text>();
        choice2Text = choice2Obj.GetComponentInChildren<Text>();
        choice3Text = choice3Obj.GetComponentInChildren<Text>();

        roundOfRound = -1;

        //GameInfoChoose.info = JsonUtility.FromJson<ChooseInfo>(GameInfoCollocation.currentGame.currentGameData);
        //GameInfoChoose.SetNewRound();
        //GameInfoChoose.currentRound = 0;

        structureTextObj.SetActive(true);
        structureTextObj.GetComponent<Text>().text = GameInfoChoose.info.words[GameInfoChoose.currentRound].structure_text;

    }

    private void Start()
    {
        scrTimer.Activate(GameInfoChoose.timeLeft);

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

        CreateRoundOfRound();
    }

    void CreateRoundOfRound()
    {
        roundOfRound++;

        if (roundOfRound > 2)
        {
            ContinueToClassifying();
            return;
        }


        CreateButtons();
    }

    void CreateButtons()
    {
        choice1Text.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 0].word;
        choice2Text.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 1].word;
        choice3Text.text = GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 2].word;
    }

    public void Choice1ButtonClicked()
    {
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound] = (GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3*roundOfRound + 0]);
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound].group = -1;
        GameInfoChoose.chosenButtons.Add((GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 0]));

        SendSetWeight(GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 0].collocation_id, GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 0].word, GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 0].score);

        WordChoose();
    }

    public void Choice2ButtonClicked()
    {
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound] = (GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3*roundOfRound + 1]);
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound].group = -1;
        GameInfoChoose.chosenButtons.Add((GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 1]));

        SendSetWeight(GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 1].collocation_id, GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 1].word, GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 1].score);

        WordChoose();
    }

    public void Choice3ButtonClicked()
    {
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound] = (GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3*roundOfRound + 2]);
        GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, roundOfRound].group = -1;
        GameInfoChoose.chosenButtons.Add((GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 2]));

        SendSetWeight(GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 2].collocation_id, GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 2].word, GameInfoChoose.info.words[GameInfoChoose.currentRound].buttons[3 * roundOfRound + 2].score);

        WordChoose();
    }

    void WordChoose()
    {
        CreateRoundOfRound();
    }

    void ContinueToClassifying()
    {
        GameInfoChoose.timeLeft = scrTimer.GetTimeLeft();
        SceneSwitcher.LoadScene2(GameSettings.CLASSIFY_INFO_CHOOSE_COL);
    }

    void Update()
    {
        if (scrTimer.time <= 0)
        {
            //SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_CHOOSE);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            //SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }

    public void SendSetWeight(int collocation_id, string word_selected, int score)
    {
        string url = GameSettings.POSTCollocationSetWieghtSoloURL + GameSettings.GetUserFBToken() + "&level_id=" + GameInfoCollocation.info.currentLevel + "&type=" + GameInfoCollocation.info.gameMode + "&game_id="+GameInfoCollocation.currentGame.currentCollocationLevelID;

        GameSettings.MyDebug(url);

        ColSetWeightRequest postData = new ColSetWeightRequest();

        postData.collocation_id = collocation_id;
        postData.game_mode = "choose";
        postData.word_selected = word_selected;
        postData.score = score;
        postData.log_session = GameInfoChoose.info.log_session;

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
        public string word_selected;
        public int score;
        public string log_session;
    }

    [System.Serializable]
    class ColSetWeightResponse
    {
        public string message;
    }
}
