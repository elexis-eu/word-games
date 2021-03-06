﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputPrefab : MonoBehaviour
{
    public GameObject previewWordLeftObj, previewWordRightObj, inputfieldPlaceholderObj, inputfieldTextObj, inputFieldObj;

    [HideInInspector]
    public Text previewWordLeftText, previewWordRightText, inputfieldPlaceholderText, inputfieldTextText;

    [HideInInspector]
    public InputField inputField;

    [HideInInspector]
    public Image inputFieldBorderImage;

    /*public GameObject insertObj;
    public InsertInsert insert;

    private bool wasFocused;*/

    private void Awake()
    {
        previewWordLeftText = previewWordLeftObj.GetComponent<Text>();
        previewWordRightText = previewWordRightObj.GetComponent<Text>();
        inputfieldPlaceholderText = inputfieldPlaceholderObj.GetComponent<Text>();
        inputfieldTextText = inputfieldTextObj.GetComponent<Text>();
        inputField = inputFieldObj.GetComponent<InputField>();
        inputFieldBorderImage = this.GetComponent<Image>();
    }

    private void Start()
    {
        inputfieldTextText.horizontalOverflow = HorizontalWrapMode.Wrap;
        //insert = insertObj.GetComponent<InsertInsert>();
    }

    /*void Update()
    {
        if (wasFocused && Input.GetKeyDown(KeyCode.Return))
        {
            GameSettings.MyDebug("Boom, it's me!");
        }

        wasFocused = inputField.isFocused;
    }*/
}
