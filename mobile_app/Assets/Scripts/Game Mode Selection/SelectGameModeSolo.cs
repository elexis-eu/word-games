using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ModeSelector : MonoBehaviour
{
    public GameObject objConnect;
    private Connect scrConnect;

    private string chosenMode;

    // Use this for initialization
    void Awake()
    {
        scrConnect = objConnect.GetComponent<Connect>();
    }

    public void ChooseMode()
    {
        ConnectToServerWithMode(GameSettings.GAME_MODE_CHOOSE);
    }

    public void InsertMode()
    {
        ConnectToServerWithMode(GameSettings.GAME_MODE_INSERT);
    }

    public void DragMode()
    {
        //ConnectToServerWithMode(GameSettings.GAME_MODE_DRAG);
        this.chosenMode = GameSettings.GAME_MODE_DRAG;
        scrConnect.ConnectToServer(chosenMode);
    }


    void ConnectToServerWithMode(string chosenMode)
    {
        this.chosenMode = chosenMode;
        scrConnect.ConnectToServerImmediate(chosenMode);
        //scrConnect.ConnectToServer(chosenMode);
    }

    void StartNewGameOfMode()
    {
        GameSettings.SOLO = true;
        int chosenModeSceneId = 0;
        switch(chosenMode)
        {
            case GameSettings.GAME_MODE_CHOOSE:
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_CHOOSE;
                chosenModeSceneId = GameSettings.WAIT_A_CHOOSE_INFO_CHOOSE;
                break;
            case GameSettings.GAME_MODE_INSERT:
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_INSERT;
                chosenModeSceneId = GameSettings.WAIT_A_INSERT_INFO_INSERT;
                break;
            case GameSettings.GAME_MODE_SYNONYM:
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_SYNONYM;
                chosenModeSceneId = GameSettings.WAIT_A_INSERT_INFO_SYNONYM;
                break;
            case GameSettings.GAME_MODE_DRAG:
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_DRAG;
                chosenModeSceneId = GameSettings.WAIT_A_DRAG_INFO_DRAG;
                break;
        }

        SceneSwitcher.LoadScene2(chosenModeSceneId);
    }

    // Update is called once per frame
    void Update()
    {
        if (scrConnect.dataReceived)
        {
            StartNewGameOfMode();
        }
    }
}