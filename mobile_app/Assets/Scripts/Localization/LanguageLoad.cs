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

        if (language_code == "sl")
        {
            GameSettings.username = "Gost";
        }
        else {
            GameSettings.username = "Guest";
        }

        GameSettings.GenerateRandomGuest();

        GameSettings.localizationManager = new LocalizationManager();
        yield return GameSettings.localizationManager.LoadJsonLanguageData(language_code);
        GameSettings.translationLoaded = true;

        LanguageText.SetNativeLanguages();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameSettings.translationLoaded && !GameSettings.localizationManager.IsError())
        {
            SceneSwitcher.LoadScene2(0);
        }
    }
}
