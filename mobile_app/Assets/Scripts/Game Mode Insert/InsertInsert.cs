using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsertInsert : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject soloButtonObj;

    public GameObject[] inputFieldsObj;

    [HideInInspector]
    public InputPrefab[] inputFieldsScr;

    private readonly Color colorRed = new Color(244/255f, 67/255f, 54/255f);
    private readonly Color colorGray = new Color(193/255f, 200/255f, 210/255f);

    private string previewWord;
    private int previewWordPos;

    public GameObject objSendScore;
    private SendScore scrSendScore;

    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
            soloButtonObj.SetActive(true);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        previewWord = GameInfoInsert.info.words[GameInfoInsert.currentRound].word;
        previewWordPos = GameInfoInsert.info.words[GameInfoInsert.currentRound].position;

        scrSendScore = objSendScore.GetComponent<SendScore>();
    }

    void Start()
    {
        scrTimer.Activate(GameInfoInsert.timeLeft);

        inputFieldsScr = new InputPrefab[inputFieldsObj.Length];
        for (int i = 0; i < inputFieldsObj.Length; i++)
        {
            inputFieldsScr[i] = inputFieldsObj[i].GetComponent<InputPrefab>();
        }

        GetFocusBtn1();

        CreateInputFields();

        ColorInputfield(0, colorRed);
    }

    void FocusInputField(int pos)
    {
        inputFieldsScr[pos].inputField.Select();
        inputFieldsScr[pos].inputField.ActivateInputField();
    }

    void LoseFocuseOnInputField(int pos)
    {
        inputFieldsScr[pos].inputField.DeactivateInputField();
    }

    void CreateInputFields()
    {
        for (int i = 0; i < inputFieldsScr.Length; i++)
        {
            if (previewWordPos == 1)
            {
                inputFieldsScr[i].previewWordLeftText.text = previewWord;
                inputFieldsScr[i].previewWordRightObj.SetActive(false);
            }
            else
            {
                inputFieldsScr[i].previewWordRightText.text = previewWord;
                inputFieldsScr[i].previewWordLeftObj.SetActive(false);
            }
        }
    }

    void ColorInputfield(int pos, Color color)
    {
        inputFieldsScr[pos].inputFieldBorderImage.color = color;
        inputFieldsScr[pos].inputfieldTextText.color = color;
        inputFieldsScr[pos].previewWordLeftText.color = color;
        inputFieldsScr[pos].previewWordRightText.color = color;
    }

    public void OnSelectedFocusChangedTo(int pos)
    {
        for (int i = 0; i < inputFieldsScr.Length; i++)
        {
            ColorInputfield(i, colorGray);
        }
        ColorInputfield(pos, colorRed);
    }

    private int focused;
    private bool newFocus;
    //private float time = 0;
    void Update()
    {
        if (!GameSettings.SOLO && scrTimer.time <= 0)
        {
            TimeOver();
        }
    }

    public void SoloButton()
    {
        TimeOver();
    }

    void TimeOver()
    {
        UpdateWordsToSend();

        SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_INSERT);
    }

    void UpdateWordsToSend()
    {
        for (int i = 0; i < GameInfoInsert.chosenWords.Length; i++)
        {
            GameInfoInsert.chosenWords[i] = inputFieldsScr[i].inputfieldTextText.text;
        }
    }

    //private bool movedField = false;
    public void GetFocusBtn1()
    {
        GameSettings.MyDebug("Focus 1");
        FocusInputField(0);
        OnSelectedFocusChangedTo(0);
    }

    public void LoseFocusBtn1()
    {
        GameSettings.MyDebug("Lose 1");
        ColorInputfield(0, colorGray);
        LoseFocuseOnInputField(1);
        /*if (inputFieldsScr[1].inputfieldTextText.text.Length == 0)
        {
            GetFocusBtn2();
        }*/
    }

    public void GetFocusBtn2()
    {
        GameSettings.MyDebug("Focus 2");
        FocusInputField(1);
        OnSelectedFocusChangedTo(1);
    }

    public void LoseFocusBtn2()
    {
        GameSettings.MyDebug("Lose 2");
        ColorInputfield(1, colorGray);
        LoseFocuseOnInputField(2);
        /*if (inputFieldsScr[2].inputfieldTextText.text.Length == 0)
        {
            GetFocusBtn3();
        }*/
    }

    public void GetFocusBtn3()
    {
        GameSettings.MyDebug("Focus 3");
        FocusInputField(2);
        OnSelectedFocusChangedTo(2);
    }

    public void LoseFocusBtn3()
    {
        GameSettings.MyDebug("Lose 3");
        ColorInputfield(2, colorGray);
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            SceneSwitcher.LoadScene2Back2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }

    public void toLowerCase(int pos)
    {
        inputFieldsScr[pos].inputField.text = inputFieldsScr[pos].inputField.text.ToLower();
    }
}