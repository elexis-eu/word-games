using UnityEngine;
using System.Collections;

public class LanguageIcon : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeLanguage() {
        GameSettings.resetLanguage = true;
        SceneSwitcher.LoadScene2(GameSettings.SCENE_LANGUAGE_SELECT);
    }
}
