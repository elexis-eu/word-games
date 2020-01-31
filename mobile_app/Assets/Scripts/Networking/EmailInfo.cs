using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Google;
using System.Linq;

public class EmailInfo : MonoBehaviour
{
    public GameObject emailInputObj, emailInputTxtObj;
    private InputField emailInput;
    private Text emailText;

    public GameObject btnSendObj;
    private Button btnSend;

    public GameObject objProgressCircle;

    private void Awake()
    {
        objProgressCircle.SetActive(false);
        btnSend = btnSendObj.GetComponent<Button>();
        btnSend.image.color = GameSettings.COLOR_WHITE;
        btnSend.interactable = false;

        emailInput = emailInputObj.GetComponent<InputField>();
        emailText = emailInputTxtObj.GetComponent<Text>();
    }

    private void Start()
    {
        emailText.horizontalOverflow = HorizontalWrapMode.Wrap;
    }

    public void OnValueEdited()
    {
        bool activate = false;
        int indexAt = emailInput.text.IndexOf('@');

        if (indexAt > -1 && (indexAt+1) < emailInput.text.Length)
        {
            activate = true;
        }

        if (activate)
        {
            btnSend.image.color = GameSettings.COLOR_BLUE;
            btnSend.interactable = true;
        }
        else
        {
            btnSend.image.color = GameSettings.COLOR_WHITE;
            btnSend.interactable = false;
        }
    }

    string email;
    public void SendEmail()
    {
        email = emailInput.text;

        EmailSend emailSend = new EmailSend();
        emailSend.user_id = GameSettings.GetUserFBToken();
        emailSend.email = email;

        objProgressCircle.SetActive(true);
        btnSendObj.SetActive(false);

        string json = JsonUtility.ToJson(emailSend);
        StartCoroutine(PostRequest(GameSettings.POSTEmailForm, json));
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
                btnSendObj.SetActive(true);
            }
            else
            {
                GameSettings.MyDebug("Received: " + uwr.downloadHandler.text);

                EmailStatusRes emailStatusRes = new EmailStatusRes();
                emailStatusRes = JsonUtility.FromJson<EmailStatusRes>(uwr.downloadHandler.text);

                if (emailStatusRes.success)
                {
                    GameSettings.email = email;
                    SceneSwitcher.LoadScene2(GameSettings.THEMATIC_LEADERBOARDS);
                } else
                {
                    objProgressCircle.SetActive(false);
                    btnSendObj.SetActive(true);
                }
            }
        }
    }
}

[System.Serializable]
class EmailSend
{
    public string user_id;
    public string email;
}

[System.Serializable]
public class EmailStatusRes
{
    public bool success;
}
