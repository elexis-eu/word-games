using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class FillerWordCol : MonoBehaviour
{
    public Text wordText;
    private float timeLeft;
    private bool dataLoaded = false;
    private float timeLeftDefault = 2.0f;

    // Use this for initialization
    void Awake()
    {
        timeLeft = timeLeftDefault;
    }

    void Start()
    {
        //wordText.text = "Loading...";
        this.dataLoaded = false;
        wordText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_WORD_FILLER");
        this.GetGameInfoQuery();
    }

    
    void Update()
    {

        if (this.dataLoaded) {

            if (GameInfoCollocation.currentGame.currentGameType == GameSettings.GAME_MODE_CHOOSE) {
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_CHOOSE;
                //SceneSwitcher.LoadScene2(GameSettings.SCENE_CHOOSE_COL);
                SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_CHOOSE_SOLO);
            } else if (GameInfoCollocation.currentGame.currentGameType == GameSettings.GAME_MODE_INSERT) {
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_INSERT;
                //SceneSwitcher.LoadScene2(GameSettings.SCENE_INSERT_COL);
                SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_INSERT_SOLO);
            } else if (GameInfoCollocation.currentGame.currentGameType == GameSettings.GAME_MODE_DRAG) {
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_DRAG;
                //SceneSwitcher.LoadScene2(GameSettings.SCENE_DRAG_COL);
                SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_DRAG_SOLO);
            } else {
                //ERROR
                GameSettings.MyDebug("Unknown game mode selected!");
            }

        }

    }

    private void OnApplicationPause(bool pause)
    {

    }

    public void GetGameInfoQuery()
    {
        string url = GameSettings.GETGameInfoCollocationURL + GameSettings.GetUserFBToken() + "&game_id=" + GameInfoCollocation.currentGame.currentCollocationLevelID + "&type=" + GameInfoCollocation.info.gameMode;

        GameSettings.MyDebug(url);

        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string url)
    {

        UnityWebRequest uwr = UnityWebRequest.Get(url);

        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();

        uwr.timeout = 10;
        yield return uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
        {
            GameSettings.MyDebug("Error While Sending: " + uwr.error);
            //SceneSwitcher.LoadScene2(GameSettings.MENU_LEADERBOARDS);
        }
        else
        {
            GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);
            GameInfoCollocation.currentGame.currentGameData = uwr.downloadHandler.text;
            //levelInfo = JsonUtility.FromJson<LevelInfo>(uwr.downloadHandler.text);
            this.dataLoaded = true;
        }
    }
}
