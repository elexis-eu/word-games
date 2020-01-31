using SignInSample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject btnLoginLogoutObj;
    private Button loginLogoutBtn;
    private Text loginLogoutTxt;

    public GameObject googleLoginObj;
    private GoogleSignedIn googleLoginScr;

    public GameObject progressCircleObj;

    void Awake()
    {
        loginLogoutBtn = btnLoginLogoutObj.GetComponent<Button>();
        loginLogoutTxt = btnLoginLogoutObj.GetComponent<Text>();

        googleLoginScr = googleLoginObj.GetComponent<GoogleSignedIn>();

        if (GameSettings.user == null)
        {
            //loginLogoutTxt.text = "Prijava";
            loginLogoutTxt.text = GameSettings.localizationManager.GetTextForKey("OPTIONS_LOGIN_BUTTON");
        } else
        {
#if UNITY_EDITOR
            GameSettings.MyDebug("Unity Editor");

#elif UNITY_IOS
            //loginLogoutTxt.text = "Prijavljen: " + GameSettings.username;
            loginLogoutTxt.text = GameSettings.localizationManager.GetTextForKey("OPTIONS_LOGIN_BUTTON").Replace("{{USER}}", GameSettings.username);

#elif UNITY_ANDROID
            //loginLogoutTxt.text = "Odjava (" + GameSettings.username + ")";
            loginLogoutTxt.text = GameSettings.localizationManager.GetTextForKey("OPTIONS_LOGIN_BUTTON").Replace("{{USER}}", GameSettings.username);

#endif

        }


    }

    public void LoginLogoutBtn()
    {
        if (GameSettings.user == null)
        {
            //googleLoginScr.OnSignIn();
            GameSettings.fromGameSettings = true;
            //PlayerPrefs.SetInt("signedIn", 0);
            SceneSwitcher.LoadScene2(GameSettings.INTRO_SCREEN);
        }
        else
        {
#if UNITY_EDITOR
            GameSettings.MyDebug("Unity Editor");

#elif UNITY_IOS
            // not possible

#elif UNITY_ANDROID
            googleLoginScr.OnSignOut();
            SceneSwitcher.LoadScene2(GameSettings.INTRO_SCREEN);

#endif
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
