using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

    private static int mainScreen = -1;

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadScene2Back(int scene)
    {

        if (SceneSwitcher.mainScreen >= 0)
        {
            SceneManager.LoadScene(SceneSwitcher.mainScreen);
            SceneSwitcher.mainScreen = -1;
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
    }

    public static void LoadScene2(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static void LoadScene2Back2(int scene)
    {
        if (SceneSwitcher.mainScreen >= 0)
        {
            SceneManager.LoadScene(SceneSwitcher.mainScreen);
            SceneSwitcher.mainScreen = -1;
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
    }

    public static void SetMainScreen(int scene)
    {
        SceneSwitcher.mainScreen = scene;
    }

    public void OpenWebPrivacyPolicy()
    {
        Application.OpenURL("https://www.fdv.uni-lj.si/raziskovanje/raziskovalni-centri/oddelek-za-komunikologijo/center-za-druzboslovnoterminolosko-in-publicisticno-raziskovanje/obvestila/promocija-jezikovne-igralne-aplikacije-za-mobilne-naprave-");
    }
}
