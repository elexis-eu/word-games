using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeChooseCheck : MonoBehaviour
{

    public GameObject objConnect;
    private Connect scrConnect;

    public GameObject thematicBtnObj;
    public GameObject colBtnObj;
    public GameObject synBtnObj;

    void Awake()
    {

        SceneSwitcher.SetMainScreen(GameSettings.SCENE_GAME_MODE);

        scrConnect = objConnect.GetComponent<Connect>();

        /*
        if (PlayModeInfo.collocations_solo && !PlayModeInfo.collocations_multi && !PlayModeInfo.synonym_multi && !PlayModeInfo.synonym_solo)
        {
            GameSettings.PREVIOUS_MODE = GameSettings.INTRO_SCREEN;
            SceneSwitcher.LoadScene2(GameSettings.SCENE_GAME_MODE_COL);
        }
        else {
            GameSettings.PREVIOUS_MODE = GameSettings.SCENE_GAME_MODE;
        }

        if (!PlayModeInfo.collocations_solo && !PlayModeInfo.collocations_multi && !PlayModeInfo.synonym_multi && PlayModeInfo.synonym_solo)
        {
            GameSettings.PREVIOUS_MODE = GameSettings.INTRO_SCREEN;
            SceneSwitcher.LoadScene2(GameSettings.SCENE_GAME_MODE_SYN);
        }
        else {
            GameSettings.PREVIOUS_MODE = GameSettings.SCENE_GAME_MODE;
        }
        */

        if (!PlayModeInfo.collocations_solo && !PlayModeInfo.collocations_multi) {
            colBtnObj.SetActive(false);
        }

        if (!PlayModeInfo.synonym_multi && !PlayModeInfo.synonym_solo) {
            synBtnObj.SetActive(false);
        }

        GameSettings.PREVIOUS_MODE = GameSettings.SCENE_GAME_MODE;

        ThematicInfo();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (scrConnect.dataReceived)
        {
            if (GameSettings.CURRENT_MODE == GameSettings.GAME_MODE_CHOOSE) 
            {
                SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_CHOOSE);
                return;
            }

            if (GameSettings.CURRENT_MODE == GameSettings.GAME_MODE_INSERT)
            {
                SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_INSERT);
                return;
            }

            if (GameSettings.CURRENT_MODE == GameSettings.GAME_MODE_DRAG)
            {
                SceneSwitcher.LoadScene2(GameSettings.SHOW_WORD_DRAG);
                return;
            }
        }

    }

    void ThematicInfo()
    {
        Color previousColor = thematicBtnObj.GetComponent<Button>().image.color;
        thematicBtnObj.GetComponent<Button>().image.color = GameSettings.COLOR_GRAY;
        thematicBtnObj.GetComponent<Button>().interactable = false;

        GameObject subText = GetChildWithName(thematicBtnObj, "SubText");

        subText.GetComponent<Text>().text = "";

        //GameSettings.thematicInfoExists = true;
        //GameSettings.thematicName = "Test";
        //GameSettings.username = "aP1qGCkXd7cgtRNJSOMSjtv8pLr1";
        //GameSettings.fromToDate = "15.5.2020";

        if (GameSettings.user != null)
        //if (GameSettings.user == null)
            {
            if (GameSettings.thematicInfoExists)
            {

                thematicBtnObj.GetComponent<Button>().image.color = previousColor;
                thematicBtnObj.GetComponent<Button>().interactable = true;

                //subText.GetComponent<Text>().text = GameSettings.localizationManager.GetTextForKey("MODE_THEMATIC_TOPIC") + " " + GameSettings.thematicName + "\n" + GameSettings.localizationManager.GetTextForKey("MODE_THEMATIC_DATE") + " " + String.Format("{0:d/M/yyyy HH:mm}", GameSettings.ThematicStartDate) + " - " + String.Format("{0:d/M/yyyy HH:mm}", GameSettings.ThematicEndDate);

                subText.GetComponent<Text>().text = GameSettings.localizationManager.GetTextForKey("MODE_THEMATIC_TOPIC") + " " + GameSettings.thematicName + "\n" + GameSettings.localizationManager.GetTextForKey("MODE_THEMATIC_DATE") + " " + String.Format("{0:d/M/yyyy}", GameSettings.ThematicEndDate);

                /*

                //thematicNameObj.SetActive(true);
                //thematicName.text = GameSettings.thematicName;
                //thematicDateObj.SetActive(true);
                //thematicDate.text = GameSettings.fromToDate;

                if (GameSettings.currentThematic)
                {
                    if (GameSettings.numberOfPlayedRounds < GameSettings.numberOfRounds)
                    {
                        //thematicBtn.image.color = GameSettings.COLOR_RED;
                        //thematicBtn.interactable = true;
                    }

                    //playedRoundsObj.SetActive(true);
                    if (GameSettings.numberOfPlayedRounds == 0)
                    {
                        //playedRounds.text = "(" + GameSettings.numberOfRounds + " krogov)";
                    }
                    else
                    {
                        //playedRounds.text = "(odigral/-a si: " + GameSettings.numberOfPlayedRounds + "/" + GameSettings.numberOfRounds + " krogov)";
                    }
                }

                */
            }
        }
    }

    public void ThematicMode()
    {
        GameSettings.THEMATIC = true;
        GameSettings.SOLO = true;

        if (GameSettings.numberOfPlayedRounds == GameSettings.numberOfRounds)
        {
            SceneSwitcher.LoadScene2(GameSettings.THEMATIC_LEADERBOARDS);
            return;
        }

        scrConnect.ConnectToServer(GameSettings.GAME_MODE_THEMATIC);
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
