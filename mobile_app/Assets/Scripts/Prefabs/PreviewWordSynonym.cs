using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewWordSynonym : MonoBehaviour
{
    public GameObject wordTextObj;

    [HideInInspector]
    public Text wordText;

    private void Awake()
    {
        wordText = wordTextObj.GetComponent<Text>();
    }
}
