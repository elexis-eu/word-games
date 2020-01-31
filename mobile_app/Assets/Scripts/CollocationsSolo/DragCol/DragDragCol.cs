using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private float saveY, saveX;

    private Color RED = new Color(244/255f, 67/255f, 54/255f);
    private Color BLUE = new Color(3/255f, 169/255f, 244/255f);

    public GameObject previewWordObjTop, previewWordObjBot;
    private PreviewWordPrefab scrPrWPreTop, scrPrWPreBot;

    public GameObject fake0Obj, fake1Obj, fake2Obj, fake3Obj, fake4Obj;

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
    }

    private void Start()
    {
        //scrTimer.Activate(GameInfoDrag.timeLeft);

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
            GameOver();
            return;
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
        GameInfoDrag.chosenButtonsNames.Add(GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].word);
        GameInfoDrag.chosenButtonsScores.Add(GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score);

        txtWordsShownScore[userWordsChoices.Count % 2].text = "+"+GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score;

        shownWordScoreObj[userWordsChoices.Count % 2].SetActive(true);

        txtWordsShownScore[userWordsChoices.Count % 2].CrossFadeAlpha(1.0f, 0f, false);

        GameInfoDrag.score += GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score;

        if (GameInfoDrag.info.words[userWordsChoices.Count].buttons[belongs].score > 0)
            txtWordsShownScore[userWordsChoices.Count % 2].color = BLUE;
        else
            txtWordsShownScore[userWordsChoices.Count % 2].color = RED;

        if (belongs == 0)
        {
            shownWordScoreObj[userWordsChoices.Count % 2].transform.position = new Vector3(fake1Obj.transform.position.x, fake1Obj.transform.position.y, 0);
        } else if (belongs == 1)
        {
            shownWordScoreObj[userWordsChoices.Count % 2].transform.position = new Vector3(fake2Obj.transform.position.x, fake2Obj.transform.position.y, 0);
        } else if (belongs == 2)
        {
            shownWordScoreObj[userWordsChoices.Count % 2].transform.position = new Vector3(fake0Obj.transform.position.x, fake0Obj.transform.position.y, 0);
        }

        txtWordsShownScore[userWordsChoices.Count % 2].CrossFadeAlpha(0.0f, 1f, false);
    }

    private void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            //GameOver();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            //SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
