using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static void LoadScene2(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OpenWebPrivacyPolicy()
    {
        Application.OpenURL("https://www.fdv.uni-lj.si/raziskovanje/raziskovalni-centri/oddelek-za-komunikologijo/center-za-druzboslovnoterminolosko-in-publicisticno-raziskovanje/obvestila/promocija-jezikovne-igralne-aplikacije-za-mobilne-naprave-");
    }
}
