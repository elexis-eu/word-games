﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WordShownDrag : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject previewWordObjTop, previewWordObjBot;
    private PreviewWordPrefab scrPrWPreTop, scrPrWPreBot;

    public GameObject whatSortObj1, whatSortObj2;
    private Text whatSortText1, whatSortText2;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        GameInfoDrag.SetNewRound();

        scrPrWPreTop = previewWordObjTop.GetComponent<PreviewWordPrefab>();
        scrPrWPreBot = previewWordObjBot.GetComponent<PreviewWordPrefab>();

        whatSortText1 = whatSortObj1.GetComponent<Text>();
        whatSortText2 = whatSortObj2.GetComponent<Text>();

        if (GameInfoDrag.currentRoundTimeLeft != 0)
        {
            timeToNextGame = (long)GameInfoDrag.currentRoundTimeLeft;
            GameInfoDrag.currentRoundTimeLeft = 0;
        }
        else
        {
            timeToNextGame = GameInfoDrag.info.round_duration_ms;
        }
    }

    // Use this if you need initialization in other scripts to happen first
    void Start()
    {
        TimeUntilNextGameRound();

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

        whatSortText1.text = GameInfoDrag.info.words[GameInfoDrag.currentRound].structure_text;
        whatSortText2.text = GameInfoDrag.info.words[GameInfoDrag.currentRound].structure_text;
        //scrPrWPreTop.text = GameInfoDrag.info.words[GameInfoDrag.currentRound].buttons[0].word;
        //scrPrWPreBot.text = GameInfoDrag.info.words[GameInfoDrag.currentRound].buttons[1].word;
    }

    long timeToNextGame;
    void TimeUntilNextGameRound()
    {
        scrTimer.Activate(timeToNextGame);
    }

    void TimeOver()
    {
        GameInfoDrag.timeLeft = scrTimer.GetTimeLeft();
        SceneSwitcher.LoadScene2(GameSettings.DRAG_DRAG);
    }

    public void CallTimeOver()
    {
        TimeOver();
    }

    void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_DRAG);
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
