using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LanguageSelect : MonoBehaviour
{

    public Text gameTitle;
    public Text languageName;

    public Text forwardButton;

    public GameObject FlagSL;
    public GameObject FlagDE;
    public GameObject FlagNL;
    public GameObject FlagPT;
    public GameObject FlagEE;
    public GameObject FlagUK;

    private Image selectedLangImage;

    void Awake()
    {
        string language_code = PlayerPrefs.GetString("LanguageCode", "none");

        if (language_code != "none" && !GameSettings.resetLanguage)
        {
            GameSettings.SetLanguage(language_code);

            if (language_code == "sl")
            {
                this.SetFlagImageSprite(FlagSL);
            }
            else if (language_code == "de")
            {
                this.SetFlagImageSprite(FlagDE);
            }
            else if (language_code == "nl")
            {
                this.SetFlagImageSprite(FlagNL);
            }
            else if (language_code == "pt")
            {
                this.SetFlagImageSprite(FlagPT);
            }
            else if (language_code == "ee")
            {
                this.SetFlagImageSprite(FlagEE);
            }
            else if (language_code == "en")
            {
                this.SetFlagImageSprite(FlagUK);
            }

            SceneSwitcher.LoadScene2(GameSettings.SCENE_LANGUAGE_LOAD);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectLanguage(string language_code){
        GameSettings.SetLanguage(language_code);

        if (language_code == "sl") {
            GameObject flagBorder = GetChildWithName(FlagSL, "Border");

            if (selectedLangImage != flagBorder.GetComponent<Image>() && selectedLangImage != null) {
                selectedLangImage.color = GameSettings.COLOR_DARK_BORDER;
            }

            flagBorder.GetComponent<Image>().color = GameSettings.COLOR_BLUE;
            gameTitle.text = "Igra besed";
            languageName.text = "Slovensko";
            forwardButton.text = "Naprej";

            selectedLangImage = flagBorder.GetComponent<Image>();

            this.SetFlagImageSprite(FlagSL);

        } else if(language_code == "de"){
            GameObject flagBorder = GetChildWithName(FlagDE, "Border");

            if (selectedLangImage != flagBorder.GetComponent<Image>() && selectedLangImage != null)
            {
                selectedLangImage.color = GameSettings.COLOR_DARK_BORDER;
            }

            flagBorder.GetComponent<Image>().color = GameSettings.COLOR_BLUE;
            gameTitle.text = "Wortspiel";
            languageName.text = "Deutsch";
            forwardButton.text = "Weiter";

            selectedLangImage = flagBorder.GetComponent<Image>();

            this.SetFlagImageSprite(FlagDE);

        } else if(language_code == "nl"){
            GameObject flagBorder = GetChildWithName(FlagNL, "Border");

            if (selectedLangImage != flagBorder.GetComponent<Image>() && selectedLangImage != null)
            {
                selectedLangImage.color = GameSettings.COLOR_DARK_BORDER;
            }

            flagBorder.GetComponent<Image>().color = GameSettings.COLOR_BLUE;
            gameTitle.text = "Woordspel";
            languageName.text = "Nederlands";
            forwardButton.text = "Voortgaan";

            selectedLangImage = flagBorder.GetComponent<Image>();

            this.SetFlagImageSprite(FlagNL);

        } else if(language_code == "pt"){
            GameObject flagBorder = GetChildWithName(FlagPT, "Border");

            if (selectedLangImage != flagBorder.GetComponent<Image>() && selectedLangImage != null)
            {
                selectedLangImage.color = GameSettings.COLOR_DARK_BORDER;
            }

            flagBorder.GetComponent<Image>().color = GameSettings.COLOR_BLUE;
            gameTitle.text = "Jogo de palavras";
            languageName.text = "Português";
            forwardButton.text = "Continuar";

            selectedLangImage = flagBorder.GetComponent<Image>();

            this.SetFlagImageSprite(FlagPT);

        } else if(language_code == "ee"){
            GameObject flagBorder = GetChildWithName(FlagEE, "Border");

            if (selectedLangImage != flagBorder.GetComponent<Image>() && selectedLangImage != null)
            {
                selectedLangImage.color = GameSettings.COLOR_DARK_BORDER;
            }

            flagBorder.GetComponent<Image>().color = GameSettings.COLOR_BLUE;
            gameTitle.text = "Sõnamäng";
            languageName.text = "Eesti";
            forwardButton.text = "Jätkama";

            selectedLangImage = flagBorder.GetComponent<Image>();

            this.SetFlagImageSprite(FlagEE);

        } else if(language_code == "en"){
            GameObject flagBorder = GetChildWithName(FlagUK, "Border");

            if (selectedLangImage != flagBorder.GetComponent<Image>() && selectedLangImage != null)
            {
                selectedLangImage.color = GameSettings.COLOR_DARK_BORDER;
            }

            flagBorder.GetComponent<Image>().color = GameSettings.COLOR_BLUE;
            gameTitle.text = "Word games";
            languageName.text = "English";
            forwardButton.text = "Continue";

            selectedLangImage = flagBorder.GetComponent<Image>();

            this.SetFlagImageSprite(FlagUK);
        }

        PlayerPrefs.SetString("LanguageCode", language_code);
    }

    private void SetFlagImageSprite(GameObject flagObject) {
        GameObject flagImage = GetChildWithName(flagObject, "Flag");
        GameSettings.languageSprite = flagImage.GetComponent<Image>().sprite;
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
}
