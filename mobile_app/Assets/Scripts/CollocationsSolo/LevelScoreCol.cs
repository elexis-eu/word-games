using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LevelScoreCol : MonoBehaviour
{

    public Text correctAnswerNumber;
    public Text wrongAnswerNumber;
    public Text sumPoints;
    public Text mainScore;
    public Text sumPosition;

    public GameObject campaignScore;

    private LevelScoreResponse LevelScoreResponseMessage;

    // Use this for initialization
    void Awake()
    {

        if (GameInfoCollocation.info.gameMode != "campaign")
        {
            campaignScore.SetActive(false);
        }

        GetLevelScoreQuery();
    }

    void Start()
    {

    }

    
    void Update()
    {

    }

    private void OnApplicationPause(bool pause)
    {

    }

    public void GoNextLevel()
    {

        if (GameInfoCollocation.info.gameMode == "campaign")
        {
            if (GameInfoCollocation.info.currentLevel < GameSettings.SYNONYM_MAX_LEVELS)
            {
                GameInfoCollocation.info.currentLevel++;
                SceneSwitcher.LoadScene2(GameSettings.SCENE_FILLER_LEVEL_COL);
            }
            else
            {
                SceneSwitcher.LoadScene2(GameSettings.SCENE_GAME_MODE_COL);
            }
        }
        else
        {
            SceneSwitcher.LoadScene2(GameSettings.SCENE_LEVEL_PICKER_COL);
        }
    }

    GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            return null;
        }
    }


    public void parseLevelScore() {

        correctAnswerNumber.text = LevelScoreResponseMessage.levelScore.ToString()+"x";
        wrongAnswerNumber.text = (30 - LevelScoreResponseMessage.levelScore).ToString() + "x";
        sumPoints.text = (LevelScoreResponseMessage.levelScore * 100).ToString()+" točk";
        mainScore.text = LevelScoreResponseMessage.mainScore.ToString()+ " točk";
        sumPosition.text = LevelScoreResponseMessage.mainPosition.ToString() + ". mesto";

    }

    public void GetLevelScoreQuery()
    {
        string url = GameSettings.GETLevelScoreURL + GameSettings.GetUserFBToken() + "&level_id=" + GameInfoCollocation.info.currentLevel + "&type=" + GameInfoCollocation.info.gameMode;

        GameSettings.MyDebug(url);

        StartCoroutine(GetRequest(url));
    }

    IEnumerator GetRequest(string url)
    {

        UnityWebRequest uwr = UnityWebRequest.Get(url);

        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();

        uwr.timeout = 5;
        yield return uwr.SendWebRequest();

        if (uwr.isHttpError || uwr.isNetworkError)
        {
            GameSettings.MyDebug("Error While Sending: " + uwr.error);
            //SceneSwitcher.LoadScene2(GameSettings.MENU_LEADERBOARDS);
        }
        else
        {
            GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);
            LevelScoreResponseMessage = JsonUtility.FromJson<LevelScoreResponse>(uwr.downloadHandler.text);

            parseLevelScore();
        }
    }

    [System.Serializable]
    class LevelScoreResponse
    {
        public int levelScore;
        public int mainScore;
        public int mainPosition;
    }


}