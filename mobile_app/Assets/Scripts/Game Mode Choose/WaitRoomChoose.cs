using UnityEngine;

public class WaitRoomChoose : MonoBehaviour
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
        GameSettings.CURRENTLY_RUNNING_GAME_MODE = GameSettings.GAME_MODE_CHOOSE;
    }

    // Use this if you need initialization in other scripts to happen first
    void Start()
    {
        TimeUntilNextGameRound();
    }

    void TimeUntilNextGameRound()
    {
        //long timeToNextGame = GameInfoChoose.info.game_start - GameInfoChoose.info.current_time;
        long timeToNextGame = (long)(GameInfoChoose.timeLeft); 
        scrTimer.Activate(timeToNextGame);
        scrTimerHelper.Activate(timeToNextGame);
    }

    void TimeOver()
    {
        SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_CHOOSE);
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
