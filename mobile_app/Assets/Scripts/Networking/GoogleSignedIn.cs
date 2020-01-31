namespace SignInSample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Firebase.Auth;
    using Firebase.Unity.Editor;
    using Google;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;

    public class GoogleSignedIn : MonoBehaviour
    {
        private FirebaseAuth auth;
        private FirebaseUser user;

        public GameObject objProgressCircle;

        public Text statusText;

        public string webClientId = "836198449315-4nom68qv8q1mgaf2a4qnnt4r0goqj2fb.apps.googleusercontent.com";

        public string location;

        public GameObject btnLogin, btnGuest;

#if UNITY_ANDROID
        private GoogleSignInConfiguration configuration;
#endif

        private ThematicTimre thematicTimer;

        // Defer the configuration creation until Awake so the web Client ID
        // Can be set via the property inspector in the Editor.
        void Awake()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

#if UNITY_ANDROID
            if (GoogleSignIn.Configuration == null)
            {
                configuration = new GoogleSignInConfiguration
                {
                    WebClientId = webClientId,
                    RequestIdToken = true
                };
            }
#endif


            //auth.StateChanged += AuthStateChanged;
            //AuthStateChanged(this, null);
        }

        void Start()
        {
            ShowButtons();

            thematicTimer = GameObject.FindGameObjectWithTag("ScriptPersistence").GetComponent<ThematicTimre>();

            if (location.Equals("introScene"))
            {
                if (PlayerPrefs.GetInt("signedIn", 0) == 1 || GameSettings.fromGameSettings)
                {
                    SignInPressed();
                    GameSettings.fromGameSettings = false;
                }
            }
        }

        void DefaultSettings()
        {

        }

        public void PlayAsGuest()
        {
            //GameSettings.username = "";
            GameSettings.email = "";
            GameSettings.user = null;
            GameSettings.usertype = "guest";
            //SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
            SceneSwitcher.LoadScene2(GameSettings.SCENE_GAME_MODE);
        }

        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (auth.CurrentUser != null)
            {
                bool signedIn = !user.UserId.Equals(auth.CurrentUser);
                if (!signedIn && user != null)
                {
                    GameSettings.MyDebug("Signed out " + user.UserId);
                    AddStatusText("sign out " + user.UserId.ToString());
                }
                //FBuser = auth.CurrentUser;
                if (signedIn)
                {
                    GameSettings.MyDebug("Signed in " + user.UserId);
                    AddStatusText("sign in " + user.UserId.ToString());
                }
            }
        }

        public void SignInPressed()
        {
#if UNITY_EDITOR
            GameSettings.MyDebug("Unity Editor");
            GameSettings.MyDebug("Unity Editor");

#elif UNITY_IOS
            GameSettings.MyDebug("iOS");
            GameSettings.MyDebug("iOS");
            iOSSignIn();

#elif UNITY_ANDROID
            GameSettings.MyDebug("Android");
            GameSettings.MyDebug("Android"); 
            OnSignIn();

#endif
        }

        void iOSSignIn()
        {
            HideButtons();
            Social.localUser.Authenticate(success => {
                if (success)
                {
                    GameSettings.MyDebug("success");
                    GameCenterAuthProvider.GetCredentialAsync().ContinueWith(OnAuthenticationFinishediOS);
                }
                else
                {
                    ShowButtons();
                    GameSettings.MyDebug("Failed to authenticate");
                    PlayerPrefs.SetInt("signedIn", 0);
                }
            });
        }

        internal void OnAuthenticationFinishediOS(Task<Credential> task)
        {
            if (task.IsCanceled)
            {
                ShowButtons();
                GameSettings.MyDebug("SignInWithCredentialAsync was canceled.");
            } else if (task.IsFaulted)
            {
                ShowButtons();
                GameSettings.MyDebug("SignInWithCredentialAsync encountered an error: " + task.Exception);
            } else
            {
                auth.SignInWithCredentialAsync(task.Result).ContinueWith(task2 => {
                    if (task2.IsCanceled)
                    {
                        ShowButtons();
                        GameSettings.MyDebug("SignInWithCredentialAsync was canceled.");
                        PlayerPrefs.SetInt("signedIn", 1);
                        return;
                    }
                    if (task2.IsFaulted)
                    {
                        ShowButtons();
                        GameSettings.MyDebug("SignInWithCredentialAsync encountered an error: " + task2.Exception);
                        PlayerPrefs.SetInt("signedIn", 1);
                        return;
                    }

                    user = task2.Result;
                    GameSettings.user = user;
                    GameSettings.MyDebug("User signed in successfully: {0} ({1}) " + user.UserId + " - " + user.UserId);

                    CallServer();
                });
            }
        }


#if UNITY_ANDROID
        public void OnSignIn()
        {
            if (PlayerPrefs.GetInt("signedIn", 0) == 1)
            {
                OnSignInSilently();
                return;
            }

            HideButtons();

            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;
            AddStatusText("Calling SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
        }

        void OnSignOutiOS()
        {
            // I'm sorry, it's not possible
        }
#endif

        public void OnSignOut()
        {
            GameSettings.user = null;
            AddStatusText("Calling SignOut");
            PlayerPrefs.SetInt("signedIn", 0);
#if UNITY_ANDROID
            GoogleSignIn.DefaultInstance.SignOut();
#endif
        }

#if UNITY_ANDROID
        public void OnDisconnect()
        {
            AddStatusText("Calling Disconnect");
            GoogleSignIn.DefaultInstance.Disconnect();
        }

        internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
        {
            if (task.IsFaulted)
            {
                ShowButtons();
                using (IEnumerator<System.Exception> enumerator =
                        task.Exception.InnerExceptions.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        ShowButtons();
                        GoogleSignIn.SignInException error =
                                (GoogleSignIn.SignInException)enumerator.Current;
                        AddStatusText("Got Error: " + error.Status + " " + error.Message);
                        OnSignOut();
                        GameSettings.MyDebug(statusText.text);
                    }
                    else
                    {
                        ShowButtons();
                        AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                        OnSignOut();
                        GameSettings.MyDebug(statusText.text);
                    }
                }
            }
            else if (task.IsCanceled)
            {
                ShowButtons();
                OnSignOut();
                AddStatusText("Canceled");
                GameSettings.MyDebug(statusText.text);
            }
            else
            {
                AddStatusText("Welcome: " + task.Result.DisplayName + "!" + task.Result);
                GameSettings.MyDebug(statusText.text);

                Firebase.Auth.Credential credential =
                    Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(task2 => {
                    if (task2.IsCanceled)
                    {
                        ShowButtons();
                        GameSettings.MyDebug("SignInWithCredentialAsync was canceled.");
                        OnSignOut();
                        return;
                    }
                    if (task2.IsFaulted)
                    {
                        ShowButtons();
                        GameSettings.MyDebug("SignInWithCredentialAsync encountered an error: " + task2.Exception);
                        OnSignOut();
                        return;
                    }
                    
                    user = task2.Result;
                    GameSettings.user = user;
                    GameSettings.MyDebug("User signed in successfully: {0} ({1}) "+user.UserId + " - " +user.UserId);

                    CallServer();
                });
            }
        }
#endif

        void CallServer()
        {
            GameSettings.user.TokenAsync(true).ContinueWith(task3 => {
                if (task3.IsCanceled)
                {
                    ShowButtons();
                    GameSettings.MyDebug("TokenAsync was canceled.");
                    return;
                }

                if (task3.IsFaulted)
                {
                    ShowButtons();
                    GameSettings.MyDebug("TokenAsync encountered an error: " + task3.Exception);
                    return;
                }

                string idToken = task3.Result;
                GameSettings.user_id = idToken;
                PlayerPrefs.SetInt("signedIn", 1);
                //objProgressCircle.SetActive(false);

                // Send token to backend via HTTPS
                // ...
                StartCoroutine(PostRequest(GameSettings.POSTLogin, CreateJSONUser(idToken)));
            });
        }

        void ShowButtons()
        {
            objProgressCircle.SetActive(false);
            if (btnGuest != null) {
                btnGuest.SetActive(true);
                btnLogin.SetActive(true);
            }
        }

        void HideButtons()
        {
            objProgressCircle.SetActive(true);
            if (btnGuest != null)
            {
                btnGuest.SetActive(false);
                btnLogin.SetActive(false);
            }
        }

#if UNITY_ANDROID
        public void OnSignInSilently()
        {
            HideButtons();

            if (GameSettings.user != null)
            {
                CallServer();
                return;
            }

            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            GoogleSignIn.Configuration.RequestEmail = true;
            AddStatusText("Calling SignIn Silently");

            GoogleSignIn.DefaultInstance.SignInSilently()
                  .ContinueWith(OnAuthenticationFinished);
        }

        void DoThis()
        {

        }

        public void OnGamesSignIn()
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = true;
            GoogleSignIn.Configuration.RequestIdToken = false;

            AddStatusText("Calling Games SignIn");

            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
              OnAuthenticationFinished);
        }
#endif

        string CreateJSONUser(string idToken)
        {
            UserStatus userStatus = new UserStatus();
            userStatus.user_id = idToken;

            string json = JsonUtility.ToJson(userStatus);

            return json;
        }

        System.Collections.IEnumerator PostRequest(string url, string json)
        {
            var uwr = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();

            uwr.timeout = 3;
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                GameSettings.MyDebug("Error While Sending: " + uwr.error);
                OnSignOut();
                ShowButtons();
            }
            else
            {
                GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);

                UserStatusRes userStatusRes = JsonUtility.FromJson<UserStatusRes>(uwr.downloadHandler.text);

                if (userStatusRes.success)
                {
                    GameSettings.username = userStatusRes.display_name;
                    GameSettings.email = (userStatusRes.email == null || userStatusRes.email.Length == 0) ? null : userStatusRes.email; // not sure what im getting, and cant check currently
                    GameSettings.usertype = "google";
                    // Get all needed stuff from login
                    thematicTimer.ProccessThematic(userStatusRes);
                    //SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
                    SceneSwitcher.LoadScene2(GameSettings.SCENE_GAME_MODE);
                }
                else
                {
                    SceneSwitcher.LoadScene2(GameSettings.REGISTER);
                }
            }
        }

        private List<string> messages = new List<string>();
        void AddStatusText(string text)
        {
            return;
            /*if (messages.Count == 5)
            {
                messages.RemoveAt(0);
            }
            messages.Add(text);
            string txt = "";
            foreach (string s in messages)
            {
                txt += "\n" + s;
            }
            statusText.text = txt;*/
        }
    }
}

[System.Serializable]
class UserStatus
{
    public string user_id;
}

[System.Serializable]
public class UserStatusRes
{
    public string display_name;
    public string email;
    public bool success;

    public string game_type;
    public int next_round;
    public int number_of_rounds;
    public string thematic_name;
    public long start_of_thematic;
    public long current_time;
    public long end_of_thematic;

    public ThematicInfoRes[] past_and_current_games_info;
}

[System.Serializable]
public class ThematicInfoRes
{
    public int id;
    public string name;
}
