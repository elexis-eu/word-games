using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalLeaderboard : MonoBehaviour
{
    public GameObject chooseBtnObj, insertBtnObj, dragBtnObj, allBtnObj, synBtnObj;
    private Button chooseBtn, insertBtn, dragBtn, allBtn, synBtn;
    private Text chooseTxt, insertTxt, dragTxt, allTxt, synTxt;

    public GameObject objScrollViewTxtScoreboard, contentObj;
    ScrollRect scrollRectScorboard;
    RectTransform contentTrans;

    public GameObject prefabPlayer, thisPlayerObj;
    private float prefabPlayerHeight;

    public GameObject getScoreboardObj;
    private GetScoreboard getScoreboard;

    private readonly Color colorBlue = new Color(3 / 255f, 169 / 255f, 244 / 255f);
    private readonly Color colorGray = new Color(193/ 255f, 200 / 255f, 210 / 255f);
    private readonly Color colorRed = new Color(244 / 255f, 67 / 255f, 54 / 255f);
    private readonly Color colorWhite = new Color(1, 1, 1);

    //public GameObject objProgressCircle;

    public int currentTab = -1;

    public Sprite fullButton, emptyButton;

    LoadLeaderboard leaderboard;

    public GameObject thisPlayerOverallObj;

    List<GameObject> contentAreaObjects;
    List<LeaderboardPlayerPrefab> leaderboardPlayerPrefabsScr;
    void Awake()
    {
        getScoreboard = getScoreboardObj.GetComponent<GetScoreboard>();

        chooseBtn = chooseBtnObj.GetComponent<Button>();
        insertBtn = insertBtnObj.GetComponent<Button>();
        dragBtn = dragBtnObj.GetComponent<Button>();
        allBtn = allBtnObj.GetComponent<Button>();
        synBtn = synBtnObj.GetComponent<Button>();

        chooseTxt = chooseBtnObj.GetComponentInChildren<Text>();
        insertTxt = insertBtnObj.GetComponentInChildren<Text>();
        dragTxt = dragBtnObj.GetComponentInChildren<Text>();
        allTxt = allBtnObj.GetComponentInChildren<Text>();
        synTxt = synBtnObj.GetComponentInChildren<Text>();

        contentAreaObjects = new List<GameObject>();
        leaderboardPlayerPrefabsScr = new List<LeaderboardPlayerPrefab>();

        scrollRectScorboard = objScrollViewTxtScoreboard.GetComponent<ScrollRect>();
        contentTrans = contentObj.GetComponent<RectTransform>();

        thisPlayerObj.SetActive(false);

        leaderboard = (LoadLeaderboard)GameObject.FindObjectOfType(typeof(LoadLeaderboard));
    }

    void Start()
    {
        //prefabPlayerHeight = prefabPlayer.GetComponent<RectTransform>().rect.height;
        ScoreboardInfo.DeleteScoreboardDataInHolder();

        chooseBtn.onClick.Invoke();
    }

    private void ResetAndSelectButton(Button button, Text text)
    {
        chooseBtn.GetComponent<Image>().sprite = emptyButton;
        insertBtn.GetComponent<Image>().sprite = emptyButton;
        dragBtn.GetComponent<Image>().sprite = emptyButton;
        allBtn.GetComponent<Image>().sprite = emptyButton;
        synBtn.GetComponent<Image>().sprite = emptyButton;

        chooseTxt.color = colorWhite;
        insertTxt.color = colorWhite;
        dragTxt.color = colorWhite;
        allTxt.color = colorWhite;
        synTxt.color = colorWhite;

        button.GetComponent<Image>().sprite = fullButton;

        text.color = colorRed;
    }

    int lastPressed = 1;
    void ButtonClick(int buttonTab, Button button, Text text)
    {
        if (currentTab == buttonTab)
            return;
        lastPressed = buttonTab+1;
        ResetAndSelectButton(button, text);
        thisPlayerObj.SetActive(false);
        leaderboard.ClearContent();
        scrollRectScorboard.normalizedPosition = new Vector2(0, 1);
        currentTab = buttonTab;
    }

    public void ChooseButton()
    {
        ButtonClick(0, chooseBtn, chooseTxt);
        getScoreboard.StartGETQueryChooseGlobal();
    }

    public void InsertButton()
    {
        ButtonClick(1, insertBtn, insertTxt);
        getScoreboard.StartGETQueryInsertGlobal();
    }

    public void SynonymButton()
    {
        ButtonClick(4, synBtn, synTxt);
        getScoreboard.StartGETQuerySynonymGlobal();
    }

    public void DragButton()
    {
        ButtonClick(2, dragBtn, dragTxt);
        getScoreboard.StartGETQueryDragGlobal();
    }

    public void AllButton()
    {
        ButtonClick(3, allBtn, allTxt);
        getScoreboard.StartGETQueryAllGlobal();
    }

    // Update is called once per frame
    void Update()
    {
        if (getScoreboard.dataReceived)
        {
            getScoreboard.dataReceived = false;
            leaderboard.CreateLeaderboard(contentTrans, ScoreboardInfo.info.scoreboard.Length, thisPlayerObj, true, 0, thisPlayerOverallObj, ScoreboardInfo.infoHolder[lastPressed]);

            //objProgressCircle.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        
    }

    public void ChangeScene()
    {
        leaderboard.BeOnDestroy();

        SceneSwitcher.LoadScene2(GameSettings.MENU_LEADERBOARDS);
    }
}
