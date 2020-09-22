using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class SendScore : MonoBehaviour
{
    private SendChoose sendChoose;
    private SendInsert sendInsert;
    private SendDrag sendDrag;
    private SendSynonym sendSynonym;

    public bool dataReceived;

    // Use this for initialization
    void Awake()
    {
        sendChoose = new SendChoose();
        sendInsert = new SendInsert();
        sendSynonym = new SendSynonym();
        sendDrag = new SendDrag();

        dataReceived = false;
    }

    // Functions are all duplicated because of unpredictable update ideas
    public void SendSelection1RoundOfChoose(string gameMode)
    {
        string url = GameSettings.POSTPublishScoreURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_TYPE + gameMode;
        sendChoose.cycle_id = GameInfoChoose.info.cycle_id;

        sendChoose.words[0] = new WordsChoose();
        sendChoose.words[0].word = GameInfoChoose.info.words[GameInfoChoose.currentRound].word;
        sendChoose.words[0].buttons = new ButtonsChoose[GameInfoChoose.chosenButtons.Count];

        for (int i = 0; i < GameInfoChoose.chosenButtons.Count; i++)
        {
            GameSettings.MyDebug(GameInfoChoose.chosenButtons[i].word + " / " + GameInfoChoose.chosenButtons[i].group);
            sendChoose.words[0].buttons[i] = new ButtonsChoose();
            sendChoose.words[0].buttons[i].word = GameInfoChoose.chosenButtons[i].word;
            sendChoose.words[0].buttons[i].position = GameInfoChoose.chosenButtons[i].group+1;
        }

        string json = JsonUtility.ToJson(sendChoose);

        GameSettings.MyDebug(json);

        StartCoroutine(PostRequest(url, json));
    }

    public void SendSelection1RoundOfInsert(string gameMode)
    {
        string url = GameSettings.POSTPublishScoreURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_TYPE + gameMode;
        sendInsert.cycle_id = GameInfoInsert.info.cycle_id;

        sendInsert.words[0] = new WordsInsert();
        sendInsert.words[0].word = GameInfoInsert.info.words[GameInfoInsert.currentRound].word;
        sendInsert.words[0].buttons = new string[GameInfoInsert.chosenWords.Length];

        for (int i = 0; i < GameInfoInsert.chosenWords.Length; i++)
        {
            sendInsert.words[0].buttons[i] = GameInfoInsert.chosenWords[i];
        }

        string json = JsonUtility.ToJson(sendInsert);
        GameSettings.MyDebug(url);
        GameSettings.MyDebug(json);

        StartCoroutine(PostRequest(url, json));
    }

    public void SendSelection1RoundOfInsertSolo(string gameMode)
    {
        string url = GameSettings.POSTPublishScoreURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_TYPE + gameMode;
        sendInsert.cycle_id = GameInfoInsert.info.cycle_id;

        sendInsert.words[0] = new WordsInsert();
        sendInsert.words[0].word = GameInfoInsert.info.words[GameInfoInsert.currentRound].word;
        sendInsert.words[0].buttons = new string[GameInfoInsert.chosenWords.Length];

        for (int i = 0; i < GameInfoInsert.chosenWords.Length; i++)
        {
            sendInsert.words[0].buttons[i] = GameInfoInsert.chosenWords[i];
        }

        string json = JsonUtility.ToJson(sendInsert);
        GameSettings.MyDebug(url);
        GameSettings.MyDebug(json);

        StartCoroutine(PostRequest(url, json));
    }

    public void SendSelection1RoundOfSynonym(string gameMode)
    {
        string url = GameSettings.POSTPublishScoreURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_TYPE + gameMode;
        sendSynonym.cycle_id = GameInfoSynonym.info.cycle_id;

        sendSynonym.words[0] = new WordsSynonym();
        sendSynonym.words[0].word = GameInfoSynonym.info.words[GameInfoSynonym.currentRound].word;
        sendSynonym.words[0].buttons = new string[GameInfoSynonym.chosenWords.Length];

        for (int i = 0; i < GameInfoSynonym.chosenWords.Length; i++)
        {
            sendSynonym.words[0].buttons[i] = GameInfoSynonym.chosenWords[i];
        }

        string json = JsonUtility.ToJson(sendSynonym);
        GameSettings.MyDebug(url);
        GameSettings.MyDebug(json);

        StartCoroutine(PostRequest(url, json));
    }

    public void SendSelection1RoundOfDrag(string gameMode)
    {
        string url = GameSettings.POSTPublishScoreURL + GameSettings.GetUserFBToken() + GameSettings.URL_END_TYPE + gameMode;
        sendDrag.cycle_id = GameInfoDrag.info.cycle_id;

        sendDrag.words = new WordsDrag[GameInfoDrag.chosenButtonsGroups.Count];
        for (int i = 0; i < GameInfoDrag.chosenButtonsGroups.Count; i++)
        {
            //GameSettings.MyDebug(GameInfoDrag.info.words[GameInfoDrag.currentRound].word + " / " + GameInfoDrag.info.words[GameInfoDrag.currentRound].buttons[GameInfoDrag.chosenButtonsGroups[i]].word);
            sendDrag.words[i] = new WordsDrag();
            sendDrag.words[i].word = GameInfoDrag.info.words[i].word;
            sendDrag.words[i].buttons[0] = new ButtonsDrag();
            sendDrag.words[i].buttons[0].word = GameInfoDrag.chosenButtonsNames[i];//GameInfoDrag.info.words[GameInfoDrag.currentRound].buttons[GameInfoDrag.chosenButtonsGroups[i]].word;
            //sendDrag.words[i].buttons[0].position = GameInfoChoose.chosenButtons[i].group_position;
        }

        string json = JsonUtility.ToJson(sendDrag);

        GameSettings.MyDebug(json);

        StartCoroutine(PostRequest(url, json));
    }




    /*public void SendWordSelectionAllRounds()
    {
        string url = GameSettings.POSTPublishScoreURL + GameSettings.username + "&type=" + GameSettings.selected_mode;
        sfAll.cycle_id = GameInfo.info.cycle_id;

        if (GameSettings.selected_mode.Equals(GameSettings.MODE_CHOOSE))
        {
            FillScoreFormAllRoundForChoose(sfAll);
        } else if(GameSettings.selected_mode.Equals(GameSettings.MODE_DRAG))
        {
            FillScoreFormAllRoundForDrag(sfAll);
        } else if(GameSettings.selected_mode.Equals(GameSettings.MODE_INSERT))
        {
            FillScoreFormAllRoundForInsert(sfAll);
        }
        
        string json = JsonUtility.ToJson(sfAll);
        GameSettings.MyDebug(json);

        StartCoroutine(PostRequest(url, json));
    }*/



    /*void FillScoreFormAllRoundForChoose(ScoreFormAll sf)
    {
        for (int i = 0; i < GameInfo.info.number_of_rounds; i++)
        {
            sf.words[i] = new WordFormAll();
            sf.words[i].word = GameInfo.info.words[i].word;
            sf.words[i].buttons = new string[GameInfo.info.max_select];
            for (int j = 0; j < GameInfo.info.max_select; j++)
            {
                sf.words[i].buttons[j] = GameInfo.selectedWords[i, j];
            }
        }
    }*/



    /*void FillScoreFormAllRoundForDrag(ScoreFormAll sf)
    {
        for (int i = 0; i < 2*GameInfo.info.number_of_rounds; i++)
        {
            sf.words[i] = new WordFormAll();
            sf.words[i].word = GameInfo.info.words[i].word;
            sf.words[i].buttons = new string[GameInfo.info.buttons_number];
            for (int j = 0; j < GameInfo.info.buttons_number; j++)
            {
                sf.words[i].buttons[j] = GameInfo.selectedWords[i, j];
            }

            
        }
    }*/



    /*void FillScoreFormAllRoundForInsert(ScoreFormAll sf)
    {
        for (int i = 0; i < GameInfo.info.number_of_rounds; i++)
        {
            sf.words[i] = new WordFormAll();
            sf.words[i].word = GameInfo.info.words[i].word;
            sf.words[i].buttons = new string[GameInfo.info.max_select];
            for (int j = 0; j < GameInfo.info.max_select; j++)
            {
                sf.words[i].buttons[j] = GameInfo.selectedWords[i, j];
            }
        }
    }*/


    IEnumerator PostRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        uwr.SetRequestHeader("Content-Type", "application/json");
        {
            //Send the request then wait here until it returns
            uwr.timeout = 5;
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                if (uwr.isHttpError)
                {
                    SceneSwitcher.LoadScene2Back2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
                }
                else
                {
                    StartCoroutine(PostRequest(url, json));
                    GameSettings.MyDebug("Error While Sending: " + uwr.error);
                }
            }
            else
            {
                if (GameSettings.THEMATIC)
                {
                    GameSettings.numberOfPlayedRounds += 1;
                }

                RecInfo recInfo = new RecInfo();
                recInfo = JsonUtility.FromJson<RecInfo>(uwr.downloadHandler.text);
                GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);

                /*int output = System.Int32.Parse(Regex.Replace(recInfo.success, "[^0-9]+", string.Empty));
                GameInfoInsert.score = output;*/
                if (GameSettings.CURRENT_MODE.Equals(GameSettings.GAME_MODE_INSERT))
                {
                    GameInfoInsert.SetRoundOverInfo(uwr.downloadHandler.text);

                    GameSettings.MyDebug(JsonUtility.ToJson(GameInfoInsert.rec));
                }

                if (GameSettings.CURRENT_MODE.Equals(GameSettings.GAME_MODE_SYNONYM))
                {
                    GameInfoSynonym.SetRoundOverInfo(uwr.downloadHandler.text);

                    GameSettings.MyDebug(JsonUtility.ToJson(GameInfoSynonym.rec));
                }

                dataReceived = true;
            }
        }
    }
}

[System.Serializable]
class RecInfo
{
    public int code;
    public string success;
}

// Send for choose
[System.Serializable]
class SendChoose
{
    public string cycle_id;
    public WordsChoose[] words = new WordsChoose[1];
}

[System.Serializable]
class WordsChoose
{
    public string word;
    public ButtonsChoose[] buttons = new ButtonsChoose[3 /*this is max cause of graphics currently*/];
}

[System.Serializable]
class ButtonsChoose
{
    public string word;
    public int position;
}
// ---------------------------------------------------
// ---------------------------------------------------
[System.Serializable]
class SendInsert
{
    public string cycle_id;
    public WordsInsert[] words = new WordsInsert[1];
}

[System.Serializable]
class WordsInsert
{
    public string word;
    public string[] buttons = new string[3 /*this is max cause of graphics currently*/];
}
// ---------------------------------------------------
// ---------------------------------------------------
[System.Serializable]
class SendDrag
{
    public string cycle_id;
    public WordsDrag[] words = new WordsDrag[1];
}

[System.Serializable]
class WordsDrag
{
    public string word;
    public ButtonsDrag[] buttons = new ButtonsDrag[1 /*this is max cause of graphics currently*/];
}

[System.Serializable]
class ButtonsDrag
{
    public string word;
    //public int position;
}
// ---------------------------------------------------
// ---------------------------------------------------
[System.Serializable]
class SendSynonym
{
    public string cycle_id;
    public WordsSynonym[] words = new WordsSynonym[1];
}

[System.Serializable]
class WordsSynonym
{
    public string word;
    public string[] buttons = new string[3 /*this is max cause of graphics currently*/];
}
// ---------------------------------------------------
// ---------------------------------------------------