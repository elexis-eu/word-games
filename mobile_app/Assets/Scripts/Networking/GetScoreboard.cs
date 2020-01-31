using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/* INFORMATION received from these script get's saved into Networking/ScoreboardInfo.cs script */

public class GetScoreboard : MonoBehaviour
{
    [HideInInspector]
    public bool dataReceived;
    [HideInInspector]
    public bool dataReceivedFailed;

    int MAX_ATTEMPTS = 1;

    string url;

    // Use this for setting values
    void Start()
    {
        dataReceived = false;

        ScoreboardInfo.DeleteScoreboardDataInHolder();
    }

    public void StartGETQuery(string mode)
    {
        url = GameSettings.GETScoreboardURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoChoose.info.cycle_id + GameSettings.URL_END_TYPE + mode;

        StartCoroutine(GetRequest(url, 0));
    }


    public void StartGETQueryChoose()
    {
        string url = GameSettings.GETScoreboardURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoChoose.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.GAME_MODE_CHOOSE;

        StartCoroutine(GetRequest(url, 0));
    }

    public void StartGETQueryInsert()
    {
        string url = GameSettings.GETScoreboardURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoInsert.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.GAME_MODE_INSERT;

        StartCoroutine(GetRequest(url, 0));
    }

    public void StartGETQuerySynonym()
    {
        string url = GameSettings.GETScoreboardURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoSynonym.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.GAME_MODE_SYNONYM;

        StartCoroutine(GetRequest(url, 0));
    }

    public void StartGETQueryDrag()
    {
        string url = GameSettings.GETScoreboardURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoDrag.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.GAME_MODE_DRAG;

        StartCoroutine(GetRequest(url, 0));
    }

    public void StartGETQueryChooseGlobal()
    {
        string url = GameSettings.GETScoreboardGlobalURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoChoose.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.GAME_MODE_CHOOSE;

        StartCoroutine(GetRequest(url, 1));
    }

    public void StartGETQueryInsertGlobal()
    {
        string url = GameSettings.GETScoreboardGlobalURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoInsert.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.GAME_MODE_INSERT;

        StartCoroutine(GetRequest(url, 2));
    }

    public void StartGETQuerySynonymGlobal()
    {
        string url = GameSettings.GETScoreboardGlobalURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoSynonym.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.GAME_MODE_SYNONYM;

        StartCoroutine(GetRequest(url, 5));
    }

    public void StartGETQueryDragGlobal()
    {
        string url = GameSettings.GETScoreboardGlobalURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoDrag.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.GAME_MODE_DRAG;

        StartCoroutine(GetRequest(url, 3));
    }

    public void StartGETQueryAllGlobal()
    {
        string url = GameSettings.GETScoreboardGlobalURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_CYCLE + GameInfoDrag.info.cycle_id + GameSettings.URL_END_TYPE + GameSettings.MODE_SUM;

        StartCoroutine(GetRequest(url, 4));
    }


    bool thematic = false;
    public void StartGETQueryThematic(int mode, int id)
    {
        thematic = true;
        string url = GameSettings.GETScoreboardGlobalThematicURL + GameSettings.GetUserFBToken() + "&thematic_id=" + id;

        StartCoroutine(GetRequest(url, mode));
    }

    IEnumerator GetRequest(string url, int mode)
    { 
        if (ScoreboardInfo.infoHolder[mode] != null)
        {
            ScoreboardInfo.info = ScoreboardInfo.infoHolder[mode];
            dataReceived = true;
            yield break;
        }

        UnityWebRequest uwr = UnityWebRequest.Get(url);

        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        dataReceivedFailed = false;

        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            uwr.timeout = 5;
            yield return uwr.SendWebRequest();

            if (uwr.isHttpError || uwr.isNetworkError)
            {
                GameSettings.MyDebug("Error While Sending: " + uwr.error);
                //SceneSwitcher.LoadScene2(GameSettings.MENU_LEADERBOARDS);
                dataReceivedFailed = true;
                continue;
            }
            else
            {
                GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);

                /*if (thematic)
                {
                    GameInfoThematic.SetRoundStartInfo(uwr.downloadHandler.text, true);
                }*/ // wtf id dis here for? i must have lost my mind aready, $#&%

                ScoreboardInfo.SetScoreboardInfoForHolders(uwr.downloadHandler.text, mode);

                ScoreboardInfo.info = ScoreboardInfo.infoHolder[mode];

                dataReceived = true;
                break;
            }
        }
    }
}