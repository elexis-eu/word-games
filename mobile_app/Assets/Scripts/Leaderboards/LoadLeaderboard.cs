using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLeaderboard : MonoBehaviour
{
    public GameObject prefabPlayer;
    private float prefabPlayerHeight;

    public List<GameObject> contentAreaObjects;
    List<LeaderboardPlayerPrefab> leaderboardPlayerPrefabsScr;

    private readonly Color colorBlue = new Color(3 / 255f, 169 / 255f, 244 / 255f);
    private readonly Color colorGray = new Color(193 / 255f, 200 / 255f, 210 / 255f);
    private readonly Color colorRed = new Color(244 / 255f, 67 / 255f, 54 / 255f);
    private readonly Color colorWhite = new Color(1, 1, 1);

    void Awake()
    {
        contentAreaObjects = new List<GameObject>();
        leaderboardPlayerPrefabsScr = new List<LeaderboardPlayerPrefab>();

        prefabPlayerHeight = prefabPlayer.GetComponent<RectTransform>().rect.height;
    }

    void Start()
    {

    }

    void CreateObjects(int i)
    {
        GameObject newPlayerObj = Instantiate(prefabPlayer, new Vector3(0, -i * prefabPlayerHeight, 0), Quaternion.identity);
        contentAreaObjects.Add(newPlayerObj);
        leaderboardPlayerPrefabsScr.Add(newPlayerObj.GetComponent<LeaderboardPlayerPrefab>());
        newPlayerObj.SetActive(false);
    }

    public void Test(RectTransform trf)
    {
        for (int i = 0; i < GameSettings.MAX_PLAYERS_LEADERBOARDS; i++)
        {
            contentAreaObjects[i].transform.SetParent(trf, false);
        }
    }


    private IEnumerator StaggeredSpawn(RectTransform contentTransform, int numOfPlayers, GameObject thisPlayerObj, bool global, int maxScore, GameObject thisPlayerOverallObj, ScoreboardInfo.Scoreboard scoreboardInfo)
    {
        int maxPerFrame = 10;
        int loopCounter = 0;

        if (global)
            SetUpPlayerViewArea(thisPlayerObj, thisPlayerOverallObj, scoreboardInfo);

        int previousScore = -1;
        int position = 1;
        int save = 0;
        bool hacked = true;
        for (int i = 0; i < numOfPlayers; i++)
        {
            if (i >= contentAreaObjects.Count) {
                CreateObjects(i);
            }
            contentAreaObjects[i].SetActive(true);
            contentAreaObjects[i].transform.SetParent(contentTransform, false);
            LeaderboardPlayerPrefab newPlayerScript = leaderboardPlayerPrefabsScr[i];
            if (global)
            {
                if (scoreboardInfo.scoreboard[i].display_name.Equals(scoreboardInfo.user_score.display_name) && GameSettings.user != null)
                {
                    coloredOne = i;
                    newPlayerScript.positionImage.color = colorBlue;
                }
            }
            else
            {
                if (scoreboardInfo.scoreboard[i].display_name.Equals(GameSettings.username))
                {
                    coloredOne = i;
                    newPlayerScript.positionImage.color = colorBlue;
                    SetUpPlayerViewArea(thisPlayerObj, i, maxScore, scoreboardInfo);
                    hacked = false;
                }
            }
            if (scoreboardInfo.scoreboard[i].score == previousScore)
            {
                position--;
                save++;
            }
            else
            {
                position += save;
                save = 0;
            }
            newPlayerScript.positionText.text = "" + (position++);
            newPlayerScript.playerNameText.text = scoreboardInfo.scoreboard[i].display_name;
            newPlayerScript.playerScoreText.text = "" + scoreboardInfo.scoreboard[i].score + " " + LanguageText.Translate("score");
            newPlayerScript.playerScoreText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_LEADERBOARD_POINTS").Replace("{{POINTS}}", scoreboardInfo.scoreboard[i].score.ToString());
            /*if (!global)
                newPlayerScript.playerScoreText.text += "/" + maxScore + " " + LanguageText.Translate("score");
            else
                newPlayerScript.playerScoreText.text += " " + LanguageText.Translate("score");*/


            previousScore = scoreboardInfo.scoreboard[i].score;

            if (i + 1 == numOfPlayers)
            {
                newPlayerScript.helperLineObj.SetActive(false);
            }

            loopCounter++;
            if (loopCounter >= maxPerFrame)
            {
                loopCounter = 0;
                yield return null;
            }
        }

        if (!global && hacked)
        {
            SetUpPlayerViewArea(thisPlayerObj, position, scoreboardInfo);
        } 
    }

    int coloredOne = 0;
    public void CreateLeaderboard(RectTransform contentTransform, int numOfPlayers, GameObject thisPlayerObj, bool global, int maxScore, GameObject thisPlayerOverallObj, ScoreboardInfo.Scoreboard scoreboardInfo)
    {
        if (numOfPlayers > GameSettings.MAX_PLAYERS_LEADERBOARDS)
        {
            numOfPlayers = GameSettings.MAX_PLAYERS_LEADERBOARDS;
        }

        contentTransform.sizeDelta = new Vector2(0, numOfPlayers * prefabPlayerHeight);

        StartCoroutine(StaggeredSpawn(contentTransform, numOfPlayers, thisPlayerObj, global, maxScore, thisPlayerOverallObj, scoreboardInfo)); // spawn laderboard players 10 per frame
    }

    void SetUpPlayerViewArea(GameObject thisPlayerObj, GameObject thisPlayerOverallObj, ScoreboardInfo.Scoreboard scoreboardInfo)
    {
        GameSettings.MyDebug(scoreboardInfo.user_score.display_name);
        if (scoreboardInfo.user_score.display_name.Equals("") || GameSettings.user == null)
        {
            thisPlayerObj.SetActive(false);
            return;
        }

        thisPlayerObj.SetActive(true);
        LeaderboardPlayerPrefab thisPlayerScript = thisPlayerObj.GetComponent<LeaderboardPlayerPrefab>();
        thisPlayerScript.positionText.text = "" + scoreboardInfo.user_score.position;
        thisPlayerScript.playerNameText.text = "" + scoreboardInfo.user_score.display_name;
        //thisPlayerScript.playerScoreText.text = "" + scoreboardInfo.user_score.score + " " + LanguageText.Translate("score");
        thisPlayerScript.playerScoreText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_LEADERBOARD_POINTS").Replace("{{POINTS}}", scoreboardInfo.user_score.score.ToString());
    }

    void SetUpPlayerViewArea(GameObject thisPlayerObj, int pos, int maxScore, ScoreboardInfo.Scoreboard scoreboardInfo)
    {
        thisPlayerObj.SetActive(true);
        LeaderboardPlayerPrefab thisPlayerScript = thisPlayerObj.GetComponent<LeaderboardPlayerPrefab>();
        thisPlayerScript.positionText.text = "" + (pos + 1);
        thisPlayerScript.playerNameText.text = scoreboardInfo.scoreboard[pos].display_name;
        //thisPlayerScript.playerScoreText.text = "" + scoreboardInfo.scoreboard[pos].score + " " + LanguageText.Translate("score");
        thisPlayerScript.playerScoreText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_LEADERBOARD_POINTS").Replace("{{POINTS}}", scoreboardInfo.scoreboard[pos].score.ToString());
    }

    // hacked + game isn't keeping sum of score, sadly
    void SetUpPlayerViewArea(GameObject thisPlayerObj, int pos, ScoreboardInfo.Scoreboard scoreboardInfo)
    {
        thisPlayerObj.SetActive(true);
        LeaderboardPlayerPrefab thisPlayerScript = thisPlayerObj.GetComponent<LeaderboardPlayerPrefab>();
        thisPlayerScript.positionText.text = "" + (pos + 1) + "+";
        thisPlayerScript.playerNameText.text = GameSettings.username;
        thisPlayerScript.playerScoreText.text = "<" + scoreboardInfo.scoreboard[scoreboardInfo.scoreboard.Length-1].score + " " + LanguageText.Translate("score");
        thisPlayerScript.playerScoreText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_MULTIPLAYER_LEADERBOARD_POINTS").Replace("{{POINTS}}", scoreboardInfo.scoreboard[scoreboardInfo.scoreboard.Length - 1].score.ToString());
    }

    public void ClearContent()
    {
        for (int i = 0; i < contentAreaObjects.Count; i++)
        {
            LeaderboardPlayerPrefab newPlayerScript = leaderboardPlayerPrefabsScr[i];
            newPlayerScript.positionImage.color = colorRed;
            newPlayerScript.helperLineObj.SetActive(true);
            contentAreaObjects[i].SetActive(false);
        }
    }

    public void BeOnDestroy()
    {
        /*for (int i = 0; i < contentAreaObjects.Count; i++)
        {
            contentAreaObjects[i].transform.SetParent(gameObject.transform, false);
            DontDestroyOnLoad(contentAreaObjects[i]);
        }*/
    }
}
