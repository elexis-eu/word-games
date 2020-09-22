using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ScoreBoardCampaignCol : MonoBehaviour
{

    public GameObject objScrollViewTxtScoreboard, contentObj;
    ScrollRect scrollRectScorboard;
    RectTransform contentTrans;

    public GameObject prefabPlayer, thisPlayerObj;

    private LeaderBoardResponse LeaderBoardResponseMessage;
    private float prefabPlayerHeight;

    private readonly Color colorBlue = new Color(3 / 255f, 169 / 255f, 244 / 255f);
    private readonly Color colorGray = new Color(193 / 255f, 200 / 255f, 210 / 255f);
    private readonly Color colorRed = new Color(244 / 255f, 67 / 255f, 54 / 255f);

    // Use this for initialization
    void Awake()
    {
        thisPlayerObj.SetActive(false);
    }

    void Start()
    {
        GetLeaderBoardQuery();
    }

    
    void Update()
    {

    }

    private void OnApplicationPause(bool pause)
    {

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


    public void parseLeaderBoard() {

        if (GameSettings.usertype == "guest") {
            thisPlayerObj.SetActive(false);
        } else {

            try {
                thisPlayerObj.SetActive(true);

                LeaderboardPlayerPrefab thisPlayerScript = thisPlayerObj.GetComponent<LeaderboardPlayerPrefab>();
                thisPlayerScript.positionText.text = "" + LeaderBoardResponseMessage.player.mainPosition.ToString();
                thisPlayerScript.positionImage.color = colorBlue;
                thisPlayerScript.playerNameText.text = "" + GameSettings.username;
                thisPlayerScript.levelText.text = "" + LeaderBoardResponseMessage.player.maxLevel.ToString();
                //thisPlayerScript.playerScoreText.text = "" + LeaderBoardResponseMessage.player.mainScore.ToString();
                thisPlayerScript.playerScoreText.text = "" + GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_SCOREBOARD_POINTS").Replace("{{POINTS}}", LeaderBoardResponseMessage.player.mainScore.ToString());
            }
            catch (Exception ex) {
                GameSettings.MyDebug(ex.Message);
            }
        }

        int i = 0;
        RectTransform contentTransform = contentObj.GetComponent<RectTransform>();
        prefabPlayerHeight = prefabPlayer.GetComponent<RectTransform>().rect.height;
        contentTransform.sizeDelta = new Vector2(0, LeaderBoardResponseMessage.leaderboard.Length * prefabPlayerHeight);

        foreach (BoardPlayer player in LeaderBoardResponseMessage.leaderboard) {
            GameObject newPlayerObj = Instantiate(prefabPlayer, new Vector3(0, -i * prefabPlayerHeight, 0), Quaternion.identity);
            newPlayerObj.transform.SetParent(contentTransform, false);
            LeaderboardPlayerPrefab newPlayerScript = newPlayerObj.GetComponent<LeaderboardPlayerPrefab>();
            newPlayerObj.SetActive(true);

            newPlayerScript.positionText.text = "" + player.rank_position.ToString();
            newPlayerScript.levelText.text = "" + player.max_level.ToString();
            newPlayerScript.playerNameText.text = player.display_name;
            //newPlayerScript.playerScoreText.text = "" + player.col_solo_score.ToString() + " " + LanguageText.Translate("score");
            newPlayerScript.playerScoreText.text = "" + GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_SCOREBOARD_POINTS").Replace("{{POINTS}}", player.col_solo_score.ToString());

            if (player.display_name == GameSettings.username) {
                newPlayerScript.positionImage.color = colorBlue;
            } else {
                newPlayerScript.positionImage.color = colorRed;
            }

            i++;
        }

    }

    public void GetLeaderBoardQuery()
    {
        string url = GameSettings.GETCollocationLeaderBoardCampaignURL + GameSettings.GetUserFBToken() + "&type=" + GameInfoCollocation.info.gameMode;

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
            LeaderBoardResponseMessage = JsonUtility.FromJson<LeaderBoardResponse>(uwr.downloadHandler.text);

            parseLeaderBoard();
        }
    }

    [System.Serializable]
    class LeaderBoardResponse
    {
        public BoardPlayer[] leaderboard;
        public PlayerMe player;
    }

    [System.Serializable]
    class BoardPlayer {
        public string uid;
        public string display_name;
        public int col_solo_score;
        public int rank_position;
        public int max_level;
    }

    [System.Serializable]
    class PlayerMe
    {
        public int mainScore;
        public int mainPosition;
        public int maxLevel;
    }


}