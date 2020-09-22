using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    string localizationKey = default;
    Text textComponent;

    public string LocalizationKey
    {
        get { return localizationKey; }
    }

    void Awake()
    {
        InvalidateText();
    }
    void Start()
    {
        //InvalidateText();
    }


    public void SetLocalizationKey(string key)
    {
        localizationKey = key;
        InvalidateText();
    }

    void InvalidateText()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<Text>();
        }
        try
        {
            textComponent.text = GameSettings.localizationManager.GetTextForKey(localizationKey);

            GameSettings.MyDebug("Translate");
        }
        catch (Exception e)
        {
            textComponent.text = localizationKey;
            // handle the exception your way
            GameSettings.MyDebug("Translate error");
        }
    }
}