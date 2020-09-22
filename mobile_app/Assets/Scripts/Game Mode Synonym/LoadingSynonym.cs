using UnityEngine;
using System.Collections;

public class LoadingSynonym : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject soloButtonObj;

    public GameObject objGetScoreboard;
    private GetScoreboard scrGetScoreboard;

    bool called;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
            soloButtonObj.SetActive(true);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        scrGetScoreboard = objGetScoreboard.GetComponent<GetScoreboard>();

        called = false;
    }

    void Start()
    {
        scrTimer.Activate(GameInfoSynonym.info.collecting_results_duration_ms);
    }

    
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

    void TimeOver()
    {
        if (!called)
        {
            scrGetScoreboard.StartGETQuery(GameSettings.GAME_MODE_SYNONYM);
            called = true;
        }
        else if (scrGetScoreboard.dataReceived)
        {
            SceneSwitcher.LoadScene2(GameSettings.SCOREBOARD_SYNONYM);
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
