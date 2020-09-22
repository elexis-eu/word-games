using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SelectGameMode : MonoBehaviour
{
    public GameObject objConnect;
    private Connect scrConnect;

    private string chosenMode;

    public GameObject thematicBtnObj;
    private Button thematicBtn;
    public GameObject thematicNameObj;
    private Text thematicName;
    public GameObject thematicDateObj;
    private Text thematicDate;
    public GameObject playedRoundsObj;
    private Text playedRounds;

    // Use this for initialization
    void Awake()
    {
        SceneSwitcher.SetMainScreen(-1);
        scrConnect = objConnect.GetComponent<Connect>();

        thematicBtn = thematicBtnObj.GetComponent<Button>();
        thematicName = thematicNameObj.GetComponent<Text>();
        thematicDate = thematicDateObj.GetComponent<Text>();
        playedRounds = playedRoundsObj.GetComponent<Text>();

        ThematicInfo();
    }

    void SetDefaultSettings()
    {
        thematicBtn.image.color = GameSettings.COLOR_WHITE;
        thematicBtn.interactable = false;
        thematicNameObj.SetActive(false);
        thematicDateObj.SetActive(false);
        playedRoundsObj.SetActive(false);
    }

    void ThematicInfo()
    {
        GameSettings.MyDebug("LET?S GET TO WORK");
        SetDefaultSettings();

        if (GameSettings.user != null)
        {
            if (GameSettings.thematicInfoExists)
            {
                thematicNameObj.SetActive(true);
                thematicName.text = GameSettings.thematicName;
                thematicDateObj.SetActive(true);
                thematicDate.text = GameSettings.fromToDate;

                if (GameSettings.currentThematic)
                {
                    if (GameSettings.numberOfPlayedRounds < GameSettings.numberOfRounds)
                    {
                        thematicBtn.image.color = GameSettings.COLOR_RED;
                        thematicBtn.interactable = true;
                    }

                    playedRoundsObj.SetActive(true);
                    if (GameSettings.numberOfPlayedRounds == 0)
                    {
                        playedRounds.text = "(" + GameSettings.numberOfRounds + " krogov)";
                    } else
                    {
                        playedRounds.text = "(odigral/-a si: " + GameSettings.numberOfPlayedRounds + "/" + GameSettings.numberOfRounds + " krogov)";
                    }
                }
            }
        }
    }

    bool solo;
    bool thematic;
    public void ChooseMode()
    {
        GameSettings.SOLO = false;
        GameSettings.THEMATIC = false;
        ConnectToServerWithMode(GameSettings.GAME_MODE_CHOOSE);
    }

    public void InsertMode()
    {
        GameSettings.SOLO = false;
        GameSettings.THEMATIC = false;
        ConnectToServerWithMode(GameSettings.GAME_MODE_INSERT);
    }

    public void DragMode()
    {
        GameSettings.SOLO = false;
        GameSettings.THEMATIC = false;
        ConnectToServerWithMode(GameSettings.GAME_MODE_DRAG);
        //this.chosenMode = GameSettings.GAME_MODE_DRAG;
        //scrConnect.ConnectToServer(chosenMode);
    }

    public void ThematicMode()
    {
        this.chosenMode = GameSettings.GAME_MODE_THEMATIC;
        GameSettings.THEMATIC = true;
        GameSettings.SOLO = true;
        scrConnect.ConnectToServer(chosenMode);
    }

    public void SynonymMode()
    {
        GameSettings.SOLO = false;
        GameSettings.THEMATIC = false;
        ConnectToServerWithMode(GameSettings.GAME_MODE_SYNONYM);
        //this.chosenMode = GameSettings.GAME_MODE_DRAG;
        //scrConnect.ConnectToServer(chosenMode);
    }

    void ConnectToServerWithMode(string chosenMode)
    {
        this.chosenMode = chosenMode;
        scrConnect.ConnectToServerImmediate(chosenMode);
        //scrConnect.ConnectToServer(chosenMode);
    }

    void StartNewGameOfMode()
    {
        switch(GameSettings.CURRENT_MODE)
        {
            case GameSettings.GAME_MODE_CHOOSE:
                ConnectToGameModeChoose();
                break;
            case GameSettings.GAME_MODE_INSERT:
                ConnectToGameModeInsert();
                break;
            case GameSettings.GAME_MODE_SYNONYM:
                ConnectToGameModeSynonym();
                break;
            case GameSettings.GAME_MODE_DRAG:
                ConnectToGameModeDrag();
                break;
        }
    }

    void ConnectToGameModeChoose()
    {
        if (GameSettings.user == null) 
            GameSettings.username = GameInfoChoose.info.player_id;
        GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_CHOOSE;
        int chosenModeSceneId = GameSettings.WAIT_A_CHOOSE_INFO_CHOOSE;

        if (GameSettings.THEMATIC && GameSettings.SOLO)
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_CHOOSE);
            return;
        }

        GameInfoChoose.timeLeft = TryToConnectToCurrentGameChoose();

        if (GameInfoChoose.timeLeft == -1)
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_CHOOSE);
        } else
        {
            SceneSwitcher.LoadScene2(chosenModeSceneId);
        }
    }

    void ConnectToGameModeDrag()
    {
        if (GameSettings.user == null)
            GameSettings.username = GameInfoDrag.info.player_id;
        GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_DRAG;
        int chosenModeSceneId = GameSettings.WAIT_A_DRAG_INFO_DRAG;

        if (GameSettings.THEMATIC && GameSettings.SOLO)
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_DRAG);
            return;
        }

        GameInfoDrag.timeLeft = TryToConnectToCurrentGameDrag();

        if (GameInfoDrag.timeLeft == -1)
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_DRAG);
        }
        else
        {
            SceneSwitcher.LoadScene2(chosenModeSceneId);
        }
    }

    void ConnectToGameModeInsert()
    {
        if (GameSettings.user == null)
            GameSettings.username = GameInfoInsert.info.player_id;
        GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_INSERT;
        int chosenModeSceneId = GameSettings.WAIT_A_INSERT_INFO_INSERT;

        if (GameSettings.THEMATIC && GameSettings.SOLO)
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_INSERT);
            return;
        }

        GameInfoInsert.timeLeft = TryToConnectToCurrentGameInsert();

        if (GameInfoInsert.timeLeft == -1)
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_INSERT);
        }
        else
        {
            SceneSwitcher.LoadScene2(chosenModeSceneId);
        }
    }

    void ConnectToGameModeSynonym()
    {
        if (GameSettings.user == null)
            GameSettings.username = GameInfoSynonym.info.player_id;
        GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_SYNONYM;
        int chosenModeSceneId = GameSettings.WAIT_A_INSERT_INFO_SYNONYM;

        if (GameSettings.THEMATIC && GameSettings.SOLO)
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_SYNONYM);
            return;
        }

        GameInfoSynonym.timeLeft = TryToConnectToCurrentGameSynonym();

        if (GameInfoSynonym.timeLeft == -1)
        {
            SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_SYNONYM);
        }
        else
        {
            SceneSwitcher.LoadScene2(chosenModeSceneId);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (scrConnect.dataReceived)
        {
            StartNewGameOfMode();
        }

        if (GameSettings.midGameThematicChange)
        {
            ThematicInfo();
            GameSettings.midGameThematicChange = false;
        }
    }



    /*
     * functions kept seperatly so that they can be (bad english incoming) future game modes can be different
     * REMINDER : CHANGE TO 1 function because PREVIOUS iterations needed multiple, and
     * I tried to predict the future of multiple again
     */
    long TryToConnectToCurrentGameChoose()
    {
        long timeLeft = GameInfoChoose.info.game_start - GameInfoChoose.info.current_time;
        GameSettings.MyDebug("Current or next game (- or +): " + timeLeft);

        if (timeLeft >= 0)
            return timeLeft;

        long gameLength = GameInfoChoose.info.number_of_rounds * (GameInfoChoose.info.round_duration_ms + GameInfoChoose.info.round_pause_duration_ms) +
            GameInfoChoose.info.collecting_results_duration_ms + GameInfoChoose.info.show_results_duration_ms;

        GameSettings.MyDebug("Total game length: " + gameLength);

        timeLeft = timeLeft + gameLength;

        GameSettings.MyDebug("Time till current game round: " + timeLeft);

        timeLeft = timeLeft - GameInfoChoose.info.collecting_results_duration_ms - GameInfoChoose.info.show_results_duration_ms;

        // Calculate current round
        int round = (int)(timeLeft / (GameInfoChoose.info.round_duration_ms + GameInfoChoose.info.round_pause_duration_ms));
        timeLeft = timeLeft % (GameInfoChoose.info.round_duration_ms + GameInfoChoose.info.round_pause_duration_ms);

        // Calculate if there is time left to join current round
        long currentRoundTimeLeft = timeLeft - GameInfoChoose.info.round_pause_duration_ms;
        if (currentRoundTimeLeft >= GameInfoChoose.info.connect_immediate_lock_time)
        {
            GameSettings.MyDebug("Joining CURRENT round!");
            round++;
            GameInfoChoose.currentRoundTimeLeft = currentRoundTimeLeft;
            timeLeft = -1;
        }

        GameInfoChoose.currentRound = GameInfoChoose.info.number_of_rounds - round - 1;
        GameSettings.MyDebug("Joining round: " + GameInfoChoose.currentRound);

        return timeLeft;
    }

    long TryToConnectToCurrentGameInsert()
    {
        long timeLeft = GameInfoInsert.info.game_start - GameInfoInsert.info.current_time;
        GameSettings.MyDebug("Current or next game (- or +): " + timeLeft);

        if (timeLeft >= 0)
            return timeLeft;

        long gameLength = GameInfoInsert.info.number_of_rounds * (GameInfoInsert.info.round_duration_ms + GameInfoInsert.info.round_pause_duration_ms) +
            GameInfoInsert.info.collecting_results_duration_ms + GameInfoInsert.info.show_results_duration_ms;

        GameSettings.MyDebug("Total game length: " + gameLength);

        timeLeft = timeLeft + gameLength;

        GameSettings.MyDebug("Time till current game round: " + timeLeft);

        timeLeft = timeLeft - GameInfoInsert.info.collecting_results_duration_ms - GameInfoInsert.info.show_results_duration_ms;

        // Calculate current round
        int round = (int)(timeLeft / (GameInfoInsert.info.round_duration_ms + GameInfoInsert.info.round_pause_duration_ms));
        timeLeft = timeLeft % (GameInfoInsert.info.round_duration_ms + GameInfoInsert.info.round_pause_duration_ms);

        // Calculate if there is time left to join current round
        long currentRoundTimeLeft = timeLeft - GameInfoInsert.info.round_pause_duration_ms;
        if (currentRoundTimeLeft >= GameInfoInsert.info.connect_immediate_lock_time)
        {
            GameSettings.MyDebug("Joining CURRENT round!");
            round++;
            GameInfoInsert.currentRoundTimeLeft = currentRoundTimeLeft;
            timeLeft = -1;
        }

        GameInfoInsert.currentRound = GameInfoInsert.info.number_of_rounds - round - 1;
        GameSettings.MyDebug("Joining round: " + GameInfoInsert.currentRound);

        return timeLeft;
    }

    long TryToConnectToCurrentGameSynonym()
    {
        long timeLeft = GameInfoSynonym.info.game_start - GameInfoSynonym.info.current_time;
        GameSettings.MyDebug("Current or next game (- or +): " + timeLeft);

        if (timeLeft >= 0)
            return timeLeft;

        long gameLength = GameInfoSynonym.info.number_of_rounds * (GameInfoSynonym.info.round_duration_ms + GameInfoSynonym.info.round_pause_duration_ms) +
            GameInfoSynonym.info.collecting_results_duration_ms + GameInfoSynonym.info.show_results_duration_ms;

        GameSettings.MyDebug("Total game length: " + gameLength);

        timeLeft = timeLeft + gameLength;

        GameSettings.MyDebug("Time till current game round: " + timeLeft);

        timeLeft = timeLeft - GameInfoSynonym.info.collecting_results_duration_ms - GameInfoSynonym.info.show_results_duration_ms;

        // Calculate current round
        int round = (int)(timeLeft / (GameInfoSynonym.info.round_duration_ms + GameInfoSynonym.info.round_pause_duration_ms));
        timeLeft = timeLeft % (GameInfoSynonym.info.round_duration_ms + GameInfoSynonym.info.round_pause_duration_ms);

        // Calculate if there is time left to join current round
        long currentRoundTimeLeft = timeLeft - GameInfoSynonym.info.round_pause_duration_ms;
        if (currentRoundTimeLeft >= GameInfoSynonym.info.connect_immediate_lock_time)
        {
            GameSettings.MyDebug("Joining CURRENT round!");
            round++;
            GameInfoSynonym.currentRoundTimeLeft = currentRoundTimeLeft;
            timeLeft = -1;
        }

        GameInfoSynonym.currentRound = GameInfoSynonym.info.number_of_rounds - round - 1;
        GameSettings.MyDebug("Joining round: " + GameInfoSynonym.currentRound);

        return timeLeft;
    }

    long TryToConnectToCurrentGameDrag()
    {
        long timeLeft = GameInfoDrag.info.game_start - GameInfoDrag.info.current_time;
        GameSettings.MyDebug("Current or next game (- or +): " + timeLeft);

        if (timeLeft >= 0)
            return timeLeft;

        long gameLength = GameInfoDrag.info.number_of_rounds * (GameInfoDrag.info.round_duration_ms + GameInfoDrag.info.round_pause_duration_ms) +
            GameInfoDrag.info.collecting_results_duration_ms + GameInfoDrag.info.show_results_duration_ms;

        GameSettings.MyDebug("Total game length: " + gameLength);

        timeLeft = timeLeft + gameLength;

        GameSettings.MyDebug("Time till current game round: " + timeLeft);

        timeLeft = timeLeft - GameInfoDrag.info.collecting_results_duration_ms - GameInfoDrag.info.show_results_duration_ms;

        // Calculate current round
        int round = (int)(timeLeft / (GameInfoDrag.info.round_duration_ms + GameInfoDrag.info.round_pause_duration_ms));
        timeLeft = timeLeft % (GameInfoDrag.info.round_duration_ms + GameInfoDrag.info.round_pause_duration_ms);

        // Calculate if there is time left to join current round
        long currentRoundTimeLeft = timeLeft - GameInfoDrag.info.round_pause_duration_ms;
        if (currentRoundTimeLeft >= GameInfoDrag.info.connect_immediate_lock_time)
        {
            GameSettings.MyDebug("Joining CURRENT round!");
            round++;
            GameInfoDrag.currentRoundTimeLeft = currentRoundTimeLeft;
            timeLeft = -1;
        }

        GameInfoDrag.currentRound = GameInfoDrag.info.number_of_rounds - round - 1;
        GameSettings.MyDebug("Joining round: " + GameInfoDrag.currentRound);

        return timeLeft;
    }
}