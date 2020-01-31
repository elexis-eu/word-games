using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ChooseGameModeCol : MonoBehaviour
{
    public Text levelText;
    public GameObject CampaignButton;

    private CampaignLevelResponse CampaignLevelResponseMessage;

    private readonly Color colorGray = new Color(193 / 255f, 200 / 255f, 210 / 255f);

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.usertype == "guest")
        {
            Image imgCampaign = CampaignButton.GetComponent<Image>();
            //imgCampaign.color = colorGray;

            Button btnCampaign = CampaignButton.GetComponent<Button>();
            //btnCampaign.interactable = false;

            //levelText.text = "Stopnja x";
            levelText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CAMPAIGN").Replace("{{LEVEL}}", "x");
        } else {
            //GameInfoCollocation.LoadTest();
            //levelText.text = "Stopnja " + GameInfoCollocation.info.campaignLevel;
            levelText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CAMPAIGN").Replace("{{LEVEL}}", GameInfoCollocation.info.campaignLevel.ToString());
            GetCampaignLevelQuery();
        }

        GetCampaignLevelQuery();

        GameSettings.PREVIOUS_MODE = GameSettings.SCENE_GAME_MODE_COL;

    }

    public void StartPractice() {
        GameInfoCollocation.info.gameMode = "practice";
        SceneSwitcher.LoadScene2(GameSettings.SCENE_LEVEL_PICKER_COL);
    }

    public void StartCampaign()
    {
        GameInfoCollocation.info.currentLevel = GameInfoCollocation.info.campaignLevel;
        GameInfoCollocation.info.gameMode = "campaign";
        SceneSwitcher.LoadScene2(GameSettings.SCENE_FILLER_LEVEL_COL);
    }

    void Start()
    {

    }

    
    void Update()
    {

    }

    public void parseCampaignLevel()
    {
        GameInfoCollocation.info.campaignLevel = CampaignLevelResponseMessage.campaignLevel;
        //levelText.text = "Stopnja " + GameInfoCollocation.info.campaignLevel;
        levelText.text = GameSettings.localizationManager.GetTextForKey("COLLOCATION_SOLO_CAMPAIGN").Replace("{{LEVEL}}", GameInfoCollocation.info.campaignLevel.ToString());

    }

    public void GetCampaignLevelQuery()
    {
        string url = GameSettings.GETCollocationsCampaignLevelURL + GameSettings.GetUserFBToken();

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
            CampaignLevelResponseMessage = JsonUtility.FromJson<CampaignLevelResponse>(uwr.downloadHandler.text);

            parseCampaignLevel();
        }
    }

    private void OnApplicationPause(bool pause)
    {

    }

    [System.Serializable]
    class CampaignLevelResponse
    {
        public int campaignLevel;
    }
}
