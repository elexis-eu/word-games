using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public void GoBack()
    {
        GameSettings.MyDebug("Switching to Scene " + GameSettings.PREVIOUS_MODE);
        SceneSwitcher.LoadScene2(GameSettings.PREVIOUS_MODE);
    }

    public void GoGoBack()
    {
        SceneSwitcher.LoadScene2(GameSettings.ANOTHER_PREVIOUS_MODE);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
