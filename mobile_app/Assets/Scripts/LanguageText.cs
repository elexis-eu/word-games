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

        AddToNativeLanguage("slovenščina", "albanščina,angleščina,arabščina,baskovščina,beloruščina,bolgarščina,bosanščina,bretonščina,čečenščina,češčina,črnogorščina,danščina,DRUGI JEZIKI,estonščina,finščina,francoščina,furlanščina,grščina,gruzinščina,hrvaščina,irščina,islandščina,italijanščina,japonščina,katalonščina,kitajščina,korejščina,latvijščina,litvanščina,lužiškasrbščina,madžarščina,makedonščina,malteščina,nemščina,nizozemščina,norveščina,poljščina,portugalščina,romščina,romunščina,ruščina,slovaščina,slovenščina,srbščina,španščina,švedščina,turščina,ukrajinščina,VEČ JEZIKOV,znakovni jezik,...");
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
