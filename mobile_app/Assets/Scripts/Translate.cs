using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Translate : MonoBehaviour
{
    public GameObject[] objectsToTranslate;
    public string[] translateToString;

    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < objectsToTranslate.Length; i++)
        {
            LanguageText.Translate(objectsToTranslate[i], translateToString[i]);
        }
    }
}
