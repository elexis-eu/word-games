using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckGamePlayMode : MonoBehaviour
{

    public GameObject solo;
    public GameObject multiplayer;
    public bool collocations = false;
    public bool synonym = false;

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

    void disableButton(GameObject ButtonObj) {
        Button solo_button = ButtonObj.GetComponent<Button>();
        solo_button.enabled = false;

        solo_button.GetComponent<Image>().color = GameSettings.COLOR_GRAY;

        GameObject button_text = GetChildWithName(ButtonObj, "Text");
        button_text.GetComponent<Text>().color = GameSettings.COLOR_WHITE;

        GameObject button_subtext = GetChildWithName(ButtonObj, "SubText");
        button_subtext.GetComponent<Text>().color = GameSettings.COLOR_WHITE;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (collocations)
        {
            if (!PlayModeInfo.collocations_solo) {
                disableButton(solo);
            }

            if (!PlayModeInfo.collocations_multi)
            {
                disableButton(multiplayer);
            }
        }

        if (synonym)
        {
            if (!PlayModeInfo.synonym_solo)
            {
                disableButton(solo);
            }

            if (!PlayModeInfo.synonym_multi)
            {
                disableButton(multiplayer);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
