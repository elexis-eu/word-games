using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviewWordPrefab : MonoBehaviour
{
    public GameObject leftImageObj, rightImageObj, wordTextObj;

    [HideInInspector]
    public Text wordText;

    private void Awake()
    {
        wordText = wordTextObj.GetComponent<Text>();
    }
}
