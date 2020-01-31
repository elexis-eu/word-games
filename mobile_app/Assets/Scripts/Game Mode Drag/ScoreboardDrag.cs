﻿using UnityEngine;
using UnityEngine.UI;

public class ScoreboardDrag : MonoBehaviour
{
    public GameObject objTimer;
    TimerUI scrTimer;

    public GameObject objScrollViewTxtScoreboard, contentObj;
    ScrollRect scrollRectScorboard;
    RectTransform contentTrans;

    public GameObject prefabPlayer, thisPlayerObj;
    private float prefabPlayerHeight;

    private readonly Color colorBlue = new Color(3 / 255f, 169 / 255f, 244 / 255f);

    private LoadLeaderboard leaderboard;

    // Use this for initialization
    void Awake()
    {
        scrTimer = objTimer.GetComponent<TimerUI>();

        scrollRectScorboard = objScrollViewTxtScoreboard.GetComponent<ScrollRect>();
        contentTrans = contentObj.GetComponent<RectTransform>();

        thisPlayerObj.SetActive(false);

        leaderboard = (LoadLeaderboard)GameObject.FindObjectOfType(typeof(LoadLeaderboard));
    }

    void Start()
    {
        long timeToNextGame = GameInfoDrag.info.game_start - GameInfoDrag.info.current_time;
        scrTimer.Activate(timeToNextGame);
        GameSettings.MyDebug("When is next game really: " + timeToNextGame);
        GameSettings.MyDebug("When I think it is: " + GameInfoDrag.info.show_results_duration_ms);

        prefabPlayerHeight = prefabPlayer.GetComponent<RectTransform>().rect.height;

        leaderboard.CreateLeaderboard(contentTrans, ScoreboardInfo.info.scoreboard.Length, thisPlayerObj, false, GameInfoDrag.info.max_round_score * GameInfoDrag.info.number_of_rounds, null, ScoreboardInfo.info);
    }

    private void Update()
    {
        if (scrTimer.time <= 0)
        {
            ChangeScene();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            ExitScene();
        }
    }

    public void ChangeScene()
    {
        if (GameSettings.user == null)
            GameSettings.username = GameInfoDrag.info.player_id;

        leaderboard.BeOnDestroy();

        SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_DRAG);
    }

    public void ExitScene()
    {
        leaderboard.BeOnDestroy();

        SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
    }
}
