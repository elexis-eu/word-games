using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LocalizationManager
{
    bool ready = false;
    bool loaded = false;
    Dictionary<string, string> localizedDictionary;
    StringBuilder filenameStringBuilder = new StringBuilder();
    string loadedJsonText = "";
    LocalizationData loadedData;
    string loadedLanguage;

    /*
    IEnumerator Start()
    {

        string language_code = PlayerPrefs.GetString("LanguageCode", "none");

        GameSettings.MyDebug("Lang: " + language_code);

        if (language_code != "none")
        {
            yield return StartCoroutine(LoadJsonLanguageData(language_code));
            ready = true;
            filenameStringBuilder = null;
            loadedJsonText = null;
            loadedData = null;
        }
    }
    */

    IEnumerator LoadFileContents(string languageCode)
    {
        string url = GameSettings.GETLanguageJSONURL + languageCode;

        GameSettings.MyDebug(url);

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            // Log an error due to missing file
        }
        
        loadedJsonText = www.downloadHandler.text;

        GameSettings.MyDebug(loadedJsonText);
    }

    public IEnumerator LoadJsonLanguageData(string languageCode)
    {
        yield return LoadFileContents(languageCode);

        try
        {
            loadedLanguage = languageCode;

            loadedData = JsonUtility.FromJson<LocalizationData>(loadedJsonText);
            localizedDictionary = new Dictionary<string, string>(loadedData.strings.Length);

            foreach (LocalizationItem item in loadedData.strings)
            {
                localizedDictionary.Add(item.key, item.value);
            }

            ready = true;
            loaded = true;
        } catch(Exception e){
            GameSettings.MyDebug(e.Message);

            PlayerPrefs.DeleteKey("LanguageCode");
            SceneSwitcher.LoadScene2(GameSettings.SCENE_LANGUAGE_SELECT);
        }
    }

    public bool IsReady()
    {
        return ready;
    }

    public bool IsLoaded()
    {
        return loaded;
    }

    public string GetTextForKey(string localizationKey)
    {
        if (localizedDictionary == null)
        {
            throw new Exception("You are missing LocalizationManager in the scene. Either add it and remove it before commit or run the app from loading screen.");
        }

        if (localizedDictionary.ContainsKey(localizationKey))
        {
            return localizedDictionary[localizationKey];
        }

        throw new Exception(String.Format("Missing localization for key: {0} and language: {1}.", localizationKey, loadedLanguage));
    }

    [System.Serializable]
    class LocalizationData {
        public LocalizationItem[] strings;
    }

    [System.Serializable]
    class LocalizationItem
    {
        public string key;
        public string value;
    }
}