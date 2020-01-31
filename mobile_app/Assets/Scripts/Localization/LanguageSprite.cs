using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LanguageSprite : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GetComponent<Image>().sprite = GameSettings.languageSprite;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
