using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

/* Connect to server, receive information for a specific game mode, 
 * pass this information to Networking/GameInfo.cs script */
public class Connect : MonoBehaviour
{
    [HideInInspector]
    public bool dataReceived;
    [HideInInspector]
    public bool dataReceivedCurrent;
    [HideInInspector]
    public bool dataFailed;

    public bool thematicExists;

    // Use this for setting values
    void Awake()
    {
        dataReceived = false;
        thematicExists = false;
        dataFailed = false;
    }

    public void ConnectToServer(string chosenMode)
    {
        string url = "";
        if (!GameSettings.SOLO)
        {
            url = GameSettings.GETConnectURL
                + GameSettings.GetUserFBToken()
                + GameSettings.URL_END_TYPE
                + chosenMode;
        } else
        {
            url = GameSettings.GETConnectURLSolo
                + GameSettings.GetUserFBToken()
                + GameSettings.URL_END_TYPE
                + chosenMode;
        }

        StartCoroutine(GetRequest(url, chosenMode, false));
    }

    public void ConnectToServerImmediate(string chosenMode)
    {
        string url = "";
        if (!GameSettings.SOLO)
        {
            url = GameSettings.GETConnectImmediateURL
                + GameSettings.GetUserFBToken()
                + GameSettings.URL_END_TYPE
                + chosenMode;
        } else
        {
            url = GameSettings.GETConnectImmediateURLSolo
                + GameSettings.GetUserFBToken()
                + GameSettings.URL_END_TYPE
                + chosenMode;
        }

        StartCoroutine(GetRequest(url, chosenMode, false));
    }

    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all Certificates are accepted
        return true;
    }

    IEnumerator GetRequest(string url, string chosenMode, bool current)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        GameSettings.MyDebug(url);

        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();

        uwr.timeout = 5;
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError)
        {
            dataFailed = true;
            GameSettings.MyDebug("Error While Sending: " + uwr.error);
        }
        else
        {
            GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);

            switch (chosenMode)
            {
                case GameSettings.GAME_MODE_CHOOSE:
                    if (!current)
                    {
                        GameInfoChoose.SetRoundStartInfo(uwr.downloadHandler.text, false);
                    }
                    else
                    {
                        GameInfoChoose.SetRoundStartInfo(uwr.downloadHandler.text, true);
                    }

                    GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_CHOOSE;
                    break;
                case GameSettings.GAME_MODE_INSERT:
                    if (!current)
                    {
                        GameInfoInsert.SetRoundStartInfo(uwr.downloadHandler.text, false);
                    }
                    else
                    {
                        GameInfoInsert.SetRoundStartInfo(uwr.downloadHandler.text, true);
                    }

                    GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_INSERT;
                    break;
                case GameSettings.GAME_MODE_SYNONYM:
                    if (!current)
                    {
                        GameInfoSynonym.SetRoundStartInfo(uwr.downloadHandler.text, false);
                    }
                    else
                    {
                        GameInfoSynonym.SetRoundStartInfo(uwr.downloadHandler.text, true);
                    }

                    GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_SYNONYM;
                    break;
                case GameSettings.GAME_MODE_DRAG:
                    if (!current)
                    {
                        GameInfoDrag.SetRoundStartInfo(uwr.downloadHandler.text, false);
                    }
                    else
                    {
                        GameInfoDrag.SetRoundStartInfo(uwr.downloadHandler.text, true);
                    }

                    GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_DRAG;
                    break;
                case GameSettings.GAME_MODE_THEMATIC:
                    if (!current)
                    {
                        GameSettings.MyDebug(uwr.downloadHandler.text);
                        if (uwr.downloadHandler.text.Length > 0)
                        {
                            GameInfoThematic.SetRoundStartInfo(uwr.downloadHandler.text, false);


                            Thematic(uwr.downloadHandler.text, false);
                        }
                    }
                    else
                    {
                        GameSettings.MyDebug(uwr.downloadHandler.text);
                        if (uwr.downloadHandler.text.Length > 0)
                        {
                            GameInfoThematic.SetRoundStartInfo(uwr.downloadHandler.text, true);



                            Thematic(uwr.downloadHandler.text, true);
                        }
                    }
                    break;
            }

            if (!current)
            {
                dataReceived = true;
            }

            if (current)
            {
                dataReceivedCurrent = true;
            }
                
            /*
            * Cancel connection if thematic is wrong (for safety reasons, if users try
            * to do stuff without internet and such stuff...)
            */
            if (GameSettings.THEMATIC)
            {
                if (GameInfoThematic.info.thematic_name == null || GameInfoThematic.info.thematic_name.Length == 0)
                {
                    dataReceived = false;
                    dataReceivedCurrent = false;
                } else if (GameInfoThematic.info.current_time < GameInfoThematic.info.start_of_thematic)
                {
                    dataReceived = false;
                    dataReceivedCurrent = false;
                }

                else if (GameInfoThematic.info.next_round >= GameInfoThematic.info.number_of_rounds)
                {
                    dataReceived = false;
                    dataReceivedCurrent = false;
                }
                
            }
        }
    }

    void Thematic(string json, bool current)
    {
        string choice = GameInfoThematic.info.game_type.ToLower();
        switch (choice)
        {
            case GameSettings.GAME_MODE_CHOOSE:
                GameSettings.MyDebug("GAME_MODE_CHOOSE");
                GameInfoChoose.SetRoundStartInfo(json, current);
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_CHOOSE;
                break;
            case GameSettings.GAME_MODE_DRAG:
                GameSettings.MyDebug("GAME_MODE_DRAG");
                GameInfoDrag.SetRoundStartInfo(json, current);
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_DRAG;
                break;
            case GameSettings.GAME_MODE_INSERT:
                GameSettings.MyDebug("GAME_MODE_INSERT");
                GameInfoInsert.SetRoundStartInfo(json, current);
                GameSettings.CURRENT_MODE = GameSettings.GAME_MODE_INSERT;
                break;
        }
    }
}