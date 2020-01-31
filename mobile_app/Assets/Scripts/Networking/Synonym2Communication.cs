using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

/* INFORMATION received from these script get's saved into Networking/ScoreboardInfo.cs script */

public class Synonym2Communication : MonoBehaviour
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

    }

    public void GetLevelInfoQuery(int levelId, Action<string> callback = null)
    {
        url = GameSettings.GETLevelInfoURL + GameSettings.GetUserFBToken() + "&level_id=" + levelId.ToString();

        StartCoroutine(GetRequest(url, callback));
    }

    IEnumerator GetRequest(string url, Action<string> callback = null)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);

        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        dataReceivedFailed = false;

        uwr.timeout = 5;
        yield return uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
        {
            GameSettings.MyDebug("Error While Sending: " + uwr.error);
            //SceneSwitcher.LoadScene2(GameSettings.MENU_LEADERBOARDS);
            dataReceivedFailed = true;
        }
        else
        {
            GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);

            if (callback != null)
            callback(uwr.downloadHandler.text);

            dataReceived = true;
        }
    }
}