using UnityEngine;
using System.Collections;

public class CheckLanguageLoaded : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.translationLoaded == false)
        {
            SceneSwitcher.LoadScene2(GameSettings.SCENE_LANGUAGE_SELECT);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
