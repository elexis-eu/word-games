using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LanguageLoad : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        string language_code = PlayerPrefs.GetString("LanguageCode", GameSettings.defaultLanguageCode);
        
        GameSettings.localizationManager = new LocalizationManager();
        yield return GameSettings.localizationManager.LoadJsonLanguageData(language_code);
        GameSettings.translationLoaded = true;

        SceneSwitcher.LoadScene2(0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
