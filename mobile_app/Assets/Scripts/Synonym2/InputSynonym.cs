using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class InputSynonym : MonoBehaviour
{
    public Text firstNumber;
    public Text secondNumber;
    public Text thirdNumber;
    public Text Headword;
    public Text TipText;

    public GameObject resultStar1;
    public GameObject resultStar2;
    public GameObject resultStar3;

    public GameObject checkButton;
    public GameObject forwardButton;

    public GameObject[] inputFieldsObj;

    [HideInInspector]
    public InputPrefabSimple[] inputFieldsScr;

    private readonly Color colorRed = new Color(244 / 255f, 67 / 255f, 54 / 255f);
    private readonly Color colorGray = new Color(193 / 255f, 200 / 255f, 210 / 255f);

    private LevelCheckResponse LevelCheckResponseMessage;

    private bool editable = true;

    // Use this for initialization
    void Awake()
    {

        int currentLevel = GameInfoSynonym2.info.currentLevel;
        int first = currentLevel / 100;
        int second = (currentLevel / 10) % 10;
        int third = currentLevel % 10;
        firstNumber.text = first.ToString();
        secondNumber.text = second.ToString();
        thirdNumber.text = third.ToString();
        editable = true;
    }

    void Start()
    {
        Headword.text = GameInfoSynonym2.currentGame.currentWord;
        TipText.text = "";

        inputFieldsScr = new InputPrefabSimple[inputFieldsObj.Length];
        for (int i = 0; i < inputFieldsObj.Length; i++)
        {
            inputFieldsScr[i] = inputFieldsObj[i].GetComponent<InputPrefabSimple>();
        }

        GetFocusBtn1();

        CreateInputFields();

        ColorInputfield(0, colorRed);

    }

    
    void Update()
    {

    }

    void FocusInputField(int pos)
    {
            inputFieldsScr[pos].inputField.Select();
            inputFieldsScr[pos].inputField.ActivateInputField();
    }

    void LoseFocuseOnInputField(int pos)
    {
        inputFieldsScr[pos].inputField.DeactivateInputField();
    }

    void CreateInputFields()
    {

    }

    void ColorInputfield(int pos, Color color)
    {
        inputFieldsScr[pos].inputFieldBorderImage.color = color;
        inputFieldsScr[pos].inputfieldTextText.color = color;
        //inputFieldsScr[pos].previewWordLeftText.color = color;
        //inputFieldsScr[pos].previewWordRightText.color = color;
    }

    public void OnSelectedFocusChangedTo(int pos)
    {
        for (int i = 0; i < inputFieldsScr.Length; i++)
        {
            ColorInputfield(i, colorGray);
        }
        ColorInputfield(pos, colorRed);
    }

    private int focused;
    private bool newFocus;

    public void GetFocusBtn1()
    {
        if (editable)
        {
            GameSettings.MyDebug("Focus 1");
            FocusInputField(0);
            OnSelectedFocusChangedTo(0);
        }
    }

    public void LoseFocusBtn1()
    {
        GameSettings.MyDebug("Lose 1");
        ColorInputfield(0, colorGray);
        LoseFocuseOnInputField(1);
        /*if (inputFieldsScr[1].inputfieldTextText.text.Length == 0)
        {
            GetFocusBtn2();
        }*/
    }

    public void GetFocusBtn2()
    {
        if (editable)
        {
            FocusInputField(1);
            OnSelectedFocusChangedTo(1);
        }
    }

    public void LoseFocusBtn2()
    {
        ColorInputfield(1, colorGray);
        LoseFocuseOnInputField(2);
        /*if (inputFieldsScr[2].inputfieldTextText.text.Length == 0)
        {
            GetFocusBtn3();
        }*/
    }

    public void GetFocusBtn3()
    {
        if (editable)
        {
            FocusInputField(2);
            OnSelectedFocusChangedTo(2);
        }
    }

    public void LoseFocusBtn3()
    {
        ColorInputfield(2, colorGray);
    }

    public void toLowerCase(int pos)
    {
        inputFieldsScr[pos].inputField.text = inputFieldsScr[pos].inputField.text.ToLower();
    }

    public void parseLevelCheck() {

        int bravo_count = 0;

        if (LevelCheckResponseMessage.score1 == 1)
        {

            ColorInputfield(0, colorRed);

            Image imageStar1 = resultStar1.GetComponent<Image>();
            imageStar1.color = colorRed;

            GameObject starCenter = GetChildWithName(resultStar1, "Star");

            Image imageStarC = starCenter.GetComponent<Image>();
            imageStarC.color = colorRed;

            bravo_count++;
        }
        else {

            ColorInputfield(0, colorGray);

            Image imageStar1 = resultStar1.GetComponent<Image>();
            imageStar1.color = colorGray;

            GameObject starCenter = GetChildWithName(resultStar1, "Star");

            Image imageStarC = starCenter.GetComponent<Image>();
            imageStarC.color = colorGray;

        }

        if (LevelCheckResponseMessage.score2 == 1)
        {
            ColorInputfield(1, colorRed);

            Image imageStar1 = resultStar2.GetComponent<Image>();
            imageStar1.color = colorRed;

            GameObject starCenter = GetChildWithName(resultStar2, "Star");

            Image imageStarC = starCenter.GetComponent<Image>();
            imageStarC.color = colorRed;

            bravo_count++;
        }
        else {
            ColorInputfield(1, colorGray);

            Image imageStar1 = resultStar2.GetComponent<Image>();
            imageStar1.color = colorGray;

            GameObject starCenter = GetChildWithName(resultStar2, "Star");

            Image imageStarC = starCenter.GetComponent<Image>();
            imageStarC.color = colorGray;
        }

        if (LevelCheckResponseMessage.score3 == 1)
        {
            ColorInputfield(2, colorRed);

            Image imageStar1 = resultStar3.GetComponent<Image>();
            imageStar1.color = colorRed;

            GameObject starCenter = GetChildWithName(resultStar3, "Star");

            Image imageStarC = starCenter.GetComponent<Image>();
            imageStarC.color = colorRed;

            bravo_count++;
        } else {
            ColorInputfield(2, colorGray);

            Image imageStar1 = resultStar3.GetComponent<Image>();
            imageStar1.color = colorGray;

            GameObject starCenter = GetChildWithName(resultStar3, "Star");

            Image imageStarC = starCenter.GetComponent<Image>();
            imageStarC.color = colorGray;
        }

        if (LevelCheckResponseMessage.suggestion != "")
        {
            if (bravo_count >= 2)
            {
                //TipText.text = "Bravo! Možna sopomenka za to besedo bi lahko bila tudi \"" + LevelCheckResponseMessage.suggestion + "\" .";
                TipText.text = GameSettings.localizationManager.GetTextForKey("SYNONYM_SOLO_INPUT_SUGGESTION_TEXT_2_STARS").Replace("{{WORD}}", LevelCheckResponseMessage.suggestion);
            }
            else {
                //TipText.text = "Možna sopomenka za to besedo bi lahko bila tudi \"" + LevelCheckResponseMessage.suggestion + "\" .";
                TipText.text = GameSettings.localizationManager.GetTextForKey("SYNONYM_SOLO_INPUT_SUGGESTION_TEXT").Replace("{{WORD}}", LevelCheckResponseMessage.suggestion);
            }
            
        }

        editable = false;
        checkButton.SetActive(false);
        forwardButton.SetActive(true);
    }

    public void CheckScores()
    {
        string url = GameSettings.GETLevelCheckURL + GameSettings.GetUserFBToken() + "&level_id=" + GameInfoSynonym2.info.currentLevel + "&type=" + GameInfoSynonym2.info.gameMode;

        GameSettings.MyDebug(url);

        LevelCheckRequest postData = new LevelCheckRequest();

        postData.headword = GameInfoSynonym2.currentGame.currentWord;
        postData.headwordID = GameInfoSynonym2.currentGame.currentWordID;
        postData.word1Text = inputFieldsScr[0].inputField.text;
        postData.word2Text = inputFieldsScr[1].inputField.text;
        postData.word3Text = inputFieldsScr[2].inputField.text;

        string json = JsonUtility.ToJson(postData);

        StartCoroutine(PostRequest(url, json));
    }

    IEnumerator PostRequest(string url, string json)
    {

        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        uwr.SetRequestHeader("Content-Type", "application/json");

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
            LevelCheckResponseMessage = JsonUtility.FromJson<LevelCheckResponse>(uwr.downloadHandler.text);

            parseLevelCheck();
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

    private void OnApplicationPause(bool pause)
    {

    }

    [System.Serializable]
    class LevelCheckRequest
    {
        public string headword;
        public int headwordID;
        public string word1Text;
        public string word2Text;
        public string word3Text;
    }

    [System.Serializable]
    class LevelCheckResponse
    {
        public int score1;
        public int score2;
        public int score3;
        public string suggestion;
    }
}
