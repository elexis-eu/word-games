﻿    using UnityEngine;

public class WaitRoomSynonym : MonoBehaviour
{
    public GameObject objTimer, objTimerHelper;
    private TimerUI scrTimer, scrTimerHelper;

    public GameObject soloButtonObj;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
            soloButtonObj.SetActive(true);
        }

        scrTimerHelper = objTimerHelper.GetComponent<TimerUI>();

        scrTimer = objTimer.GetComponent<TimerUI>();
        GameSettings.CURRENTLY_RUNNING_GAME_MODE = GameSettings.GAME_MODE_SYNONYM;
    }

    // Use this if you need initialization in other scripts to happen first
    void Start()
    {
        TimeUntilNextGameRound();
    }

    void TimeUntilNextGameRound()
    {
        long timeToNextGame = (long)(GameInfoSynonym.timeLeft);
        scrTimer.Activate(timeToNextGame);
        scrTimerHelper.Activate(timeToNextGame);
    }

    void TimeOver()
    {
        SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_SYNONYM);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            TimeOver();
        }
    }

    public void SoloButton()
    {
        TimeOver();
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
