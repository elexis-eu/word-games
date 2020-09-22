using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ThematicTimre : MonoBehaviour
{
    private static GameObject instance;

    private IEnumerator loginCouroutine;

    private long REFRESH_LOGIN_TIME = 900;
    private long QUIT_AND_REFRESH_LOGIN_TIME = 1800;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = gameObject;
        } else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void StopRefreshMidGame()
    {
        if (loginCouroutine != null)
            StopCoroutine(loginCouroutine);
    }

    public void RefreshLoginMidGame(float time)
    {
        StopRefreshMidGame();

        loginCouroutine = RefreshLoginEnum(time);

        StartCoroutine(loginCouroutine);
    }

    IEnumerator RefreshLoginEnum(float time)
    {
        time = Math.Abs(time);
        GameSettings.MyDebug("Wait for: " + time);
        yield return new WaitForSecondsRealtime(time);

        if (GameSettings.user != null)
        {

            if (wasPaused && (Time.realtimeSinceStartup - pausedTime > QUIT_AND_REFRESH_LOGIN_TIME))
            {
                wasPaused = false;
                SceneSwitcher.LoadScene2(GameSettings.INTRO_SCREEN);
            }
            else
            {
                CallRefreshLogin();
            }
        }
    }

    private void CallRefreshLogin()
    {
        GameSettings.user.TokenAsync(true).ContinueWith(task3 => {
            if (task3.IsCanceled)
            {
                GameSettings.MyDebug("TokenAsync was canceled.");
                return;
            }

            if (task3.IsFaulted)
            {
                RefreshLoginMidGame(0.5f);
                GameSettings.MyDebug("TokenAsync encountered an error: " + task3.Exception);
                return;
            }

            string idToken = task3.Result;
            GameSettings.user_id = idToken;
            //objProgressCircle.SetActive(false);

            // Send token to backend via HTTPS
            // ...
            StartCoroutine(PostRequest(GameSettings.POSTLogin, CreateJSONUser(idToken)));
        });
    }

    public System.Collections.IEnumerator PostRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();

        uwr.timeout = 3;
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            RefreshLoginMidGame(0.5f);
            GameSettings.MyDebug("Error While Sending: " + uwr.error);
        }
        else
        {
            GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);

            UserStatusRes userStatusRes = JsonUtility.FromJson<UserStatusRes>(uwr.downloadHandler.text);

            if (userStatusRes.success)
            {
                GameSettings.username = userStatusRes.display_name;
                // Get all needed stuff from login
                ProccessThematic(userStatusRes);
                GameSettings.midGameThematicChange = true;
            }
            else
            {
                // what should I do with the drunken sailor,...
            }
        }
    }

    string CreateJSONUser(string idToken)
    {
        UserStatus userStatus = new UserStatus();
        userStatus.user_id = idToken;

        string json = JsonUtility.ToJson(userStatus);

        return json;
    }

    private float pausedTime;
    private bool wasPaused = false;
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            pausedTime = Time.realtimeSinceStartup;
            wasPaused = true;
        }
    }

    bool IsCurrentThematic(long start, long end, long curr)
    {
        // convert milliseconds to seconds
        start /= 1000;
        end /= 1000;
        curr /= 1000;

        long toStart = start - curr;
        long toEnd = end - curr;

        GameSettings.MyDebug("Times: " + toStart + " / " + toEnd + " / " + REFRESH_LOGIN_TIME);
        GameSettings.MyDebug("out time is :  "  + (toStart >= REFRESH_LOGIN_TIME ? REFRESH_LOGIN_TIME : toStart));

        if (toStart > 0)
        {
            RefreshLoginMidGame(toStart >= REFRESH_LOGIN_TIME ? REFRESH_LOGIN_TIME : toStart);
            return false;
        } else
        {
            RefreshLoginMidGame(toEnd >= REFRESH_LOGIN_TIME ? REFRESH_LOGIN_TIME : toEnd);
            return true;
        }
    }

    void DefaultThematicSettings()
    {
        GameSettings.thematicInfoExists = false;
        GameSettings.activateThematicMode = false;
        GameSettings.thematicName = "";
        GameSettings.fromToDate = "";
        GameSettings.numberOfRounds = 0;
        GameSettings.numberOfPlayedRounds = 0;
        GameSettings.midGameThematicChange = false;
        GameSettings.currentThematic = false;
    }

    public void ProccessThematic(UserStatusRes userStatusRes)
    {
        GameSettings.past_and_current_games_info = userStatusRes.past_and_current_games_info;
        GameSettings.MyDebug("THEMATICS: " + userStatusRes.past_and_current_games_info.Length);

        DefaultThematicSettings();

        if (userStatusRes.thematic_name != null && userStatusRes.thematic_name.Length > 0)
        {
            GameSettings.thematicInfoExists = true;

            GameSettings.thematicName = userStatusRes.thematic_name;

            FormatDate(userStatusRes);

            bool isCurrent = IsCurrentThematic(userStatusRes.start_of_thematic, userStatusRes.end_of_thematic, userStatusRes.current_time);

            if (isCurrent)
            {
                GameSettings.currentThematic = true;
                GameSettings.numberOfPlayedRounds = userStatusRes.next_round;
                GameSettings.numberOfRounds = userStatusRes.number_of_rounds;

                if (GameSettings.numberOfPlayedRounds < GameSettings.numberOfRounds)
                {
                    GameSettings.activateThematicMode = true;
                }
            }
        } else
        {
            RefreshLoginMidGame(REFRESH_LOGIN_TIME);
        }
    }

    private void FormatDate(UserStatusRes userStatusRes)
    {
        DateTime startDate = DateTimeOffset.FromUnixTimeMilliseconds(userStatusRes.start_of_thematic).DateTime.ToLocalTime();
        DateTime endDate = DateTimeOffset.FromUnixTimeMilliseconds(userStatusRes.end_of_thematic).DateTime.ToLocalTime();

        string startDateString = SloFormat(startDate);
        string endDateString = SloFormat(endDate);

        GameSettings.fromToDate = startDateString + "-" + endDateString;
        GameSettings.ThematicStartDate = startDate;
        GameSettings.ThematicEndDate = endDate;
    }

    private string SloFormat(DateTime dateTime)
    {
        string parse = String.Format("{0:d. M. yyyy HH:mm}", dateTime);

        return parse;
    }
}
