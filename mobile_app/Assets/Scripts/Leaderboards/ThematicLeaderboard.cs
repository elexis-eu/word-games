using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ThematicLeaderboard : MonoBehaviour
{
    public GameObject chooseBtnObj, insertBtnObj, dragBtnObj, allBtnObj;
    private Button chooseBtn, insertBtn, dragBtn, allBtn;
    private Text chooseTxt, insertTxt, dragTxt, allTxt;

    private ColorBlock chooseColor;

    public GameObject thematicNameObj;
    private Text thematicNameText;

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

    public GameObject thisPlayerOverallObj;

    //public GameObject objProgressCircle;

    public int currentTab = -1;

    public Sprite fullButton, emptyButton;

    List<GameObject> contentAreaObjects;
    List<LeaderboardPlayerPrefab> leaderboardPlayerPrefabsScr;

    private LoadLeaderboard leaderboard;
    void Awake()
    {
        chooseBtn = chooseBtnObj.GetComponent<Button>();
        insertBtn = insertBtnObj.GetComponent<Button>();
        dragBtn = dragBtnObj.GetComponent<Button>();
        allBtn = allBtnObj.GetComponent<Button>();

        chooseTxt = chooseBtnObj.GetComponentInChildren<Text>();
        insertTxt = insertBtnObj.GetComponentInChildren<Text>();
        dragTxt = dragBtnObj.GetComponentInChildren<Text>();
        allTxt = allBtnObj.GetComponentInChildren<Text>();

        getScoreboard = getScoreboardObj.GetComponent<GetScoreboard>();
        //textBtn.text = GameSettings.compitTheme;

        chooseColor = chooseBtn.colors;

        contentAreaObjects = new List<GameObject>();
        leaderboardPlayerPrefabsScr = new List<LeaderboardPlayerPrefab>();

        scrollRectScorboard = objScrollViewTxtScoreboard.GetComponent<ScrollRect>();
        contentTrans = contentObj.GetComponent<RectTransform>();

        thematicNameText = thematicNameObj.GetComponent<Text>();

        thisPlayerObj.SetActive(false);

        leaderboard = (LoadLeaderboard)GameObject.FindObjectOfType(typeof(LoadLeaderboard));
    }

    // I just copied global and dont care', sry, its called manual labor
    void Start()
    {
        for (int i = 0; i < GameSettings.past_and_current_games_info.Length; i++)
        {
            if (i == 4)
                break;

            string name = GameSettings.past_and_current_games_info[i].name;
            string[] parts = name.Split(' ');
            parts[0] = parts[0].ToUpper();
            if (parts.Length > 1)
                parts[0] += "...";

            if (i == 0)
            {
                chooseBtnObj.SetActive(true);
                chooseTxt.text = parts[0];
            } else if (i == 1)
            {
                insertBtnObj.SetActive(true);
                insertTxt.text = parts[0];
            }
            else if (i == 2)
            {
                dragBtnObj.SetActive(true);
                dragTxt.text = parts[0];
            }
            else if (i == 3)
            {
                allBtnObj.SetActive(true);
                allTxt.text = parts[0];
            }
        }

        prefabPlayerHeight = prefabPlayer.GetComponent<RectTransform>().rect.height;

        if (GameSettings.past_and_current_games_info.Length > 0)
            chooseBtn.onClick.Invoke();
    }

    private void AllColorsToGray()
    {
        chooseColor.normalColor = Color.red;
    }

    private void ColorToWhite(ColorBlock color)
    {
        color.normalColor = colorWhite;
    }

    private void ResetAndSelectButton(Button button, Text text)
    {
        chooseBtn.GetComponent<Image>().sprite = emptyButton;
        insertBtn.GetComponent<Image>().sprite = emptyButton;
        dragBtn.GetComponent<Image>().sprite = emptyButton;
        allBtn.GetComponent<Image>().sprite = emptyButton;

        chooseTxt.color = colorWhite;
        insertTxt.color = colorWhite;
        dragTxt.color = colorWhite;
        allTxt.color = colorWhite;

        button.GetComponent<Image>().sprite = fullButton;

        text.color = colorRed;
    }

    int lastPressed = 0;
    void ButtonClick(int buttonTab, Button button, Text text)
    {
        if (currentTab == buttonTab)
            return;
        lastPressed = buttonTab;
        ResetAndSelectButton(button, text);
        thisPlayerObj.SetActive(false);
        leaderboard.ClearContent();
        scrollRectScorboard.normalizedPosition = new Vector2(0, 1);
        currentTab = buttonTab;
    }

    public void ChooseButton()
    {
        ButtonClick(0, chooseBtn, chooseTxt);
        getScoreboard.StartGETQueryThematic(0, GameSettings.past_and_current_games_info[0].id);
    }

    public void InsertButton()
    {
        ButtonClick(1, insertBtn, insertTxt);
        getScoreboard.StartGETQueryThematic(1, GameSettings.past_and_current_games_info[1].id);
    }

    public void DragButton()
    {
        ButtonClick(2, dragBtn, dragTxt);
        getScoreboard.StartGETQueryThematic(2, GameSettings.past_and_current_games_info[2].id);
    }

    public void AllButton()
    {
        ButtonClick(3, allBtn, allTxt);
        getScoreboard.StartGETQueryThematic(3, GameSettings.past_and_current_games_info[3].id);
    }

    // Update is called once per frame
    void Update()
    {
        if (getScoreboard.dataReceived)
        {
            GameSettings.MyDebug("CALLED REFRESH BOOM");
            getScoreboard.dataReceived = false;
            thematicNameText.text = ScoreboardInfo.info.thematic_name;
            leaderboard.CreateLeaderboard(contentTrans, ScoreboardInfo.info.scoreboard.Length, thisPlayerObj, true, 0, thisPlayerOverallObj, ScoreboardInfo.infoHolder[lastPressed]);
            //objProgressCircle.SetActive(false);
            //textBtn.text = GameInfoThematic.info.game_type.ToUpper();
        }
    }

    public void ChangeScene()
    {
        leaderboard.BeOnDestroy();

        SceneSwitcher.LoadScene2(GameSettings.MENU_LEADERBOARDS);
    }
}
