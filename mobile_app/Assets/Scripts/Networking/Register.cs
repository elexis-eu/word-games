using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Google;

public class Register : MonoBehaviour
{
    public GameObject nicknameInputObj, languageDropdownObj, ageDropdownObj;

    private InputField nicknameInput;
    private Dropdown languageDropdown, ageDropdown;
    private Text language, age;
    
    public GameObject btnRegister, btnPrivacyPolicy;
    private Button btnRegisterBtn, btnPrivacyPolicyBtn;

    public GameObject objProgressCircle;

    // Translation
    public GameObject registerTopObj, registerBotObj, nicknameObj, nativeObj, ageObj, registerInfoObj;

    public GameObject objConnect;
    private Connect scrConnect;

    public GameObject nicknameTaken;

    private void Awake()
    {
        objProgressCircle.SetActive(false);
        btnRegisterBtn = btnRegister.GetComponent<Button>();
        btnRegisterBtn.image.color = GameSettings.COLOR_WHITE;
        btnRegisterBtn.interactable = false;

        nicknameInput = nicknameInputObj.GetComponent<InputField>();
        languageDropdown = languageDropdownObj.GetComponent<Dropdown>();
        ageDropdown = ageDropdownObj.GetComponent<Dropdown>();

        language = languageDropdownObj.GetComponentInChildren<Text>();
        age = ageDropdownObj.GetComponentInChildren<Text>();

        scrConnect = objConnect.GetComponent<Connect>();

        ageDropdown.value = 1;

        // Translation
        TranslationSpecial();
    }

    void Start()
    {
        if (GameSettings.user == null)
        {
            SceneSwitcher.LoadScene2(GameSettings.INTRO_SCREEN);
        }
    }

    private void TranslationSpecial()
    {
        AddNativeLanguageOptions();
    }

    void AddNativeLanguageOptions()
    {
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(LanguageText.nativeLanguages);
        languageDropdown.value = LanguageText.defaultOption;
    }

    public void OpenWebsitePrivacyPolicy()
    {
        Application.OpenURL("https://www.fdv.uni-lj.si/raziskovanje/raziskovalni-centri/oddelek-za-komunikologijo/center-za-druzboslovnoterminolosko-in-publicisticno-raziskovanje/obvestila/promocija-jezikovne-igralne-aplikacije-za-mobilne-naprave-");
    }

    public void OnBack()
    {
        GameSettings.user = null;
#if UNITY_ANDROID
        GoogleSignIn.DefaultInstance.SignOut();
#endif
        PlayerPrefs.SetInt("signedIn", 0);
        SceneSwitcher.LoadScene2(GameSettings.INTRO_SCREEN);
    }

    public void OnValueEdited()
    {
        if (nicknameInput.text.Length >= 3)
        {
            btnRegisterBtn.image.color = GameSettings.COLOR_BLUE;
            btnRegisterBtn.interactable = true;
        }
        else
        {
            btnRegisterBtn.image.color = GameSettings.COLOR_WHITE;
            btnRegisterBtn.interactable = false;
        }
    }

    private void Update()
    {
        if (scrConnect.dataReceived)
        {
            SceneSwitcher.LoadScene2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
            scrConnect.dataReceived = false;
        }

        if (scrConnect.dataFailed)
        {
            objProgressCircle.SetActive(false);
            btnRegister.SetActive(true);
            btnPrivacyPolicy.SetActive(true);
            scrConnect.dataFailed = false;
        }
    }

    string nickname;
    public void RegisterSend()
    {
        if (GameSettings.user == null)
        {
            SceneSwitcher.LoadScene2(GameSettings.INTRO_SCREEN);
        }

        nickname = nicknameInput.text;
        string language0 = language.text.ToString();
        string age0 = age.text.ToString();

        GameSettings.MyDebug(nickname + " / " + language0 + " / " + age0);

        RegisterStatus registerStatus = new RegisterStatus();
        registerStatus.user_id = GameSettings.GetUserFBToken();
        registerStatus.nickname = nickname;
        registerStatus.age = age0;
        registerStatus.native_language = language0;

        objProgressCircle.SetActive(true);
        btnRegister.SetActive(false);
        btnPrivacyPolicy.SetActive(false);

        string json = JsonUtility.ToJson(registerStatus);
        StartCoroutine(PostRequest(GameSettings.POSTRegisterForm, json));
    }

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
                GameSettings.MyDebug("Error While Sending: " + uwr.error);
                objProgressCircle.SetActive(false);
                btnRegister.SetActive(true);
                btnPrivacyPolicy.SetActive(true);
            }
            else
            {
                GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);

                RegisterStatusRes registerStatusRes = new RegisterStatusRes();
                registerStatusRes = JsonUtility.FromJson<RegisterStatusRes>(uwr.downloadHandler.text);

                if (registerStatusRes.success)
                {
                    GameSettings.username = nickname;
                    SceneSwitcher.LoadScene2(GameSettings.INTRO_SCREEN);
                } else
                {
                    objProgressCircle.SetActive(false);
                    btnRegister.SetActive(true);
                    btnPrivacyPolicy.SetActive(true);
                    nicknameTaken.SetActive(true);
                }
            }
        }
    }
}

[System.Serializable]
class RegisterStatus
{
    public string user_id;
    public string nickname;
    public string age;
    public string native_language;
}

[System.Serializable]
public class RegisterStatusRes
{
    public bool success;
}
