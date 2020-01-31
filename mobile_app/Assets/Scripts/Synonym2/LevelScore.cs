using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LevelScore : MonoBehaviour
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

        if (GameInfoSynonym2.info.gameMode != "campaign")
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

        if (GameInfoSynonym2.info.gameMode == "campaign")
        {
            if (GameInfoSynonym2.info.currentLevel < GameSettings.SYNONYM_MAX_LEVELS)
            {
                GameInfoSynonym2.info.currentLevel++;
                SceneSwitcher.LoadScene2(GameSettings.SCENE_FILLER_LEVEL);
            }
            else
            {
                SceneSwitcher.LoadScene2(GameSettings.SCENE_GAME_MODE);
            }
        }
        else
        {
            SceneSwitcher.LoadScene2(GameSettings.SCENE_LEVEL_PICKER);
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
        //sumPoints.text = (LevelScoreResponseMessage.levelScore * 100).ToString()+" točk";
        sumPoints.text = GameSettings.localizationManager.GetTextForKey("SYNONYM_SOLO_SCORE_SUM_POINTS").Replace("{{POINTS}}", (LevelScoreResponseMessage.levelScore * 100).ToString());

        //mainScore.text = LevelScoreResponseMessage.mainScore.ToString()+ " točk";
        mainScore.text = GameSettings.localizationManager.GetTextForKey("SYNONYM_SOLO_SCORE_MAIN_SCORE").Replace("{{POINTS}}", LevelScoreResponseMessage.mainScore.ToString());

        //sumPosition.text = LevelScoreResponseMessage.mainPosition.ToString() + ". mesto";
        sumPosition.text = GameSettings.localizationManager.GetTextForKey("SYNONYM_SOLO_SCORE_MAIN_POSITION").Replace("{{POSITION}}", LevelScoreResponseMessage.mainPosition.ToString());

    }

    public void GetLevelScoreQuery()
    {
        string url = GameSettings.GETLevelScoreURL + GameSettings.GetUserFBToken() + "&level_id=" + GameInfoSynonym2.info.currentLevel + "&type=" + GameInfoSynonym2.info.gameMode;

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