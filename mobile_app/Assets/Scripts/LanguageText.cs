using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LanguageText
{
    private static Dictionary<string, string> dict;

    private static string currentLanguage = "svn";

    // Register screen options
    public static List<string> nativeLanguages;
    public static int defaultOption = 0;

    static LanguageText()
    {
        dict = new Dictionary<string, string>();

        nativeLanguages = new List<string>();

        switch (currentLanguage)
        {
            case "svn":
                CreateSloLanguage();
                break;
        }
    }

    public static void Translate(GameObject gameObject, string translation)
    {
        if (dict[translation] == null || dict[translation].Length == 0)
            return;
        gameObject.GetComponentInChildren<Text>().text = dict[translation];
    }

    public static string Translate(string translation)
    {
        return dict[translation];
    }

    private static void CreateSloLanguage()
    {
        // DO NOT CHANGE FIRST PAIR OF KEY VALUES, second one is the translation
        // COPY PASTE method for different languages
        dict.Add("game_name", "Igra besed");
        dict.Add("login", "Prijava");
        dict.Add("register", "Registracija");
        dict.Add("guest", "Igraj kot gost");

        dict.Add("multiplayer", "Tekma");
        dict.Add("solo", "Vaja");

        dict.Add("choose", "Izberi");
        dict.Add("insert", "Vtipkaj");
        dict.Add("drag", "Povleci");
        dict.Add("compit", "Tematsko");

        dict.Add("nickname", "VZDEVEK");
        dict.Add("age", "STAROST");
        dict.Add("native", "MATERNI JEZIK");
        dict.Add("registerInfo", "S pritiskom na gumb <color=blue>“Registracija”</color> potrjujem, da sem prebral / -a informacije o projektu, v katerem je igra nastala, in pravni poduk ter se strinjam s pogoji uporabe.");

        dict.Add("score", "točk");

        SetNativeLanguages();
    }

    public static void SetNativeLanguages() {
        nativeLanguages = new List<string>();

        string language_code = PlayerPrefs.GetString("LanguageCode", GameSettings.defaultLanguageCode);

        if (language_code == "sl") {
            AddToNativeLanguage("slovenščina", "čeština,dansk,Deutsch,eesti keel,English,español,français,Frysk,Gaelige,hrvatski,íslenska,italiano,język polski,latviešu valoda,lietuvių kalba,limba română,magyar,Malti,Nederlands,norsk,português,pусский язык,shqip,slovenčina,slovenščina,suomi,svenska,Türkçe,Ivrit,ελληνικά,Български,македонски,српски,українська мова,DRUGO,");
        }

        if (language_code == "en")
        {
            AddToNativeLanguage("English", "čeština,dansk,Deutsch,eesti keel,English,español,français,Frysk,Gaelige,hrvatski,íslenska,italiano,język polski,latviešu valoda,lietuvių kalba,limba română,magyar,Malti,Nederlands,norsk,português,pусский язык,shqip,slovenčina,slovenščina,suomi,svenska,Türkçe,Ivrit,ελληνικά,Български,македонски,српски,українська мова,OTHER,");
        }

        if (language_code == "et")
        {
            AddToNativeLanguage("eesti keel", "čeština,dansk,Deutsch,eesti keel,English,español,français,Frysk,Gaelige,hrvatski,íslenska,italiano,język polski,latviešu valoda,lietuvių kalba,limba română,magyar,Malti,Nederlands,norsk,português,pусский язык,shqip,slovenčina,slovenščina,suomi,svenska,Türkçe,Ivrit,ελληνικά,Български,македонски,српски,українська мова,MUU,");
        }

        if (language_code == "pt")
        {
            AddToNativeLanguage("português", "čeština,dansk,Deutsch,eesti keel,English,español,français,Frysk,Gaelige,hrvatski,íslenska,italiano,język polski,latviešu valoda,lietuvių kalba,limba română,magyar,Malti,Nederlands,norsk,português,pусский язык,shqip,slovenčina,slovenščina,suomi,svenska,Türkçe,Ivrit,ελληνικά,Български,македонски,српски,українська мова,OUTROS,");
        }

        if (language_code == "nl")
        {
            AddToNativeLanguage("Nederlands", "čeština,dansk,Deutsch,eesti keel,English,español,français,Frysk,Gaelige,hrvatski,íslenska,italiano,język polski,latviešu valoda,lietuvių kalba,limba română,magyar,Malti,Nederlands,norsk,português,pусский язык,shqip,slovenčina,slovenščina,suomi,svenska,Türkçe,Ivrit,ελληνικά,Български,македонски,српски,українська мова,ANDERE,");
        }

    }

    private static void AddToNativeLanguage(string defaultLanguage, string listOfLanguages)
    {
        string[] list = listOfLanguages.Split(',');

        for (int i = 0; i < list.Length; i++)
        {
            nativeLanguages.Add(list[i]);

            if (list[i].Equals(defaultLanguage))
                defaultOption = i;
        }
    }
}
