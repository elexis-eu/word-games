using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClassifyChoose : MonoBehaviour
{
    public GameObject objTimer;
    private TimerUI scrTimer;

    public GameObject soloButtonObj;

    public GameObject[] classyObj, choiceObj;
    private Text[] classyText, choiceText;
    private Button[] classyBtn, choiceBtn;
    private Image[] classyImg, choiceImg;

    private List<ChooseButtons> classyfiedBtns;

    private readonly Color colorGray = new Color(193/255f, 200/255f, 210/255f);
    private readonly Color colorBlack = new Color(61/255f, 72/255f, 90/255f);
    private readonly Color colorRed = new Color(244 / 255f, 67 / 255f, 54 / 255f);

    public GameObject summaryObj, titleObj;

    private bool reorder;
    private List<int> reorderNumbers;

    // Use this for initialization
    void Awake()
    {
        if (GameSettings.SOLO)
        {
            objTimer.SetActive(false);
        }

        scrTimer = objTimer.GetComponent<TimerUI>();

        GameInfoChoose.tempList = GameInfoChoose.chosenButtons;

        classyText = new Text[3];
        classyBtn = new Button[3];
        classyImg = new Image[3];

        choiceText = new Text[3];
        choiceBtn = new Button[3];
        choiceImg = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            classyText[i] = classyObj[i].GetComponentInChildren<Text>();
            classyBtn[i] = classyObj[i].GetComponent<Button>();
            classyImg[i] = classyObj[i].GetComponent<Image>();

            choiceText[i] = choiceObj[i].GetComponentInChildren<Text>();
            choiceBtn[i] = choiceObj[i].GetComponent<Button>();
            choiceImg[i] = choiceObj[i].GetComponent<Image>();
        }

        classyfiedBtns = new List<ChooseButtons>();

        reorder = false;
        reorderNumbers = new List<int>();

        setUpChosenWords();
    }

    private void Start()
    {
        scrTimer.Activate(GameInfoChoose.timeLeft);
    }

    void setUpChosenWords()
    {
        classyImg[0].color = colorBlack;

        string previewWord = GameInfoChoose.info.words[GameInfoChoose.currentRound].word;

        int currentRound = GameInfoChoose.currentRound;

        ChooseButtons choice1 = GameInfoChoose.selectedButtons[currentRound, 0];
        ChooseButtons choice2 = GameInfoChoose.selectedButtons[currentRound, 1];
        ChooseButtons choice3 = GameInfoChoose.selectedButtons[currentRound, 2];


        int wordPosition = GameInfoChoose.info.words[currentRound].position;

        if (choice1 != null)
            choiceText[0].text = wordPosition == 1 ? previewWord + " " + choice1.word :
                                                   choice1.word + " " + previewWord;
        if (choice2 != null)
            choiceText[1].text = wordPosition == 1 ? previewWord + " " + choice2.word :
                                                   choice2.word + " " + previewWord;
        if (choice3 != null)
            choiceText[2].text = wordPosition == 1 ? previewWord + " " + choice3.word :
                                                   choice3.word + " " + previewWord;
    }

    void ChoiceBtnClicked(int no)
    {
        if (choiceBtn[no].IsActive())
        {
            classyfiedBtns.Add(GameInfoChoose.selectedButtons[GameInfoChoose.currentRound, no]);
            choiceObj[no].SetActive(false);
            UpdateUI();
        }

        if (classyfiedBtns.Count == 3)
        {
            RecolorUI();
            reorder = true;

            if (GameSettings.SOLO)
                soloButtonObj.SetActive(true);

            summaryObj.SetActive(true);
            //titleObj.SetActive(true);
        }
    }

    void RecolorUI()
    {
        for (int i = 0; i < classyText.Length; i++)
        {
            classyText[i].color = colorRed;
            classyImg[i].color = colorRed;
        }
    }

    public void Classify1BtnClicked()
    {
        ClassifySelect(0);
    }

    public void Classify2BtnClicked()
    {
        ClassifySelect(1);
    }

    public void Classify3BtnClicked()
    {
        ClassifySelect(2);
    }

    void ClassifySelect(int pos)
    {
        if (reorder)
        {
            reorderNumbers.Add(pos);
            classyImg[pos].color = colorBlack;
            classyText[pos].color = colorBlack;

            if (reorderNumbers.Count == 2)
            {
                RecolorUI();

                ChooseButtons temp = classyfiedBtns[reorderNumbers[0]];
                classyfiedBtns[reorderNumbers[0]] = classyfiedBtns[reorderNumbers[1]];
                classyfiedBtns[reorderNumbers[1]] = temp;

                reorderNumbers.Clear();

                UpdateUI();
            }
        }
    }

    public void Choice1BtnClicked()
    {
        ChoiceBtnClicked(0);
    }

    public void Choice2BtnClicked()
    {
        ChoiceBtnClicked(1);
    }

    public void Choice3BtnClicked()
    {
        ChoiceBtnClicked(2);
    }

    void UpdateUI()
    {
        string previewWord = GameInfoChoose.info.words[GameInfoChoose.currentRound].word;

        int currentRound = GameInfoChoose.currentRound;

        int wordPosition = GameInfoChoose.info.words[currentRound].position;

        for (int i = 0; i < classyfiedBtns.Count; i++)
        {
            classyText[i].text = wordPosition == 1 ? previewWord + " " + classyfiedBtns[i].word:
                                                     classyfiedBtns[i].word + " " + previewWord;
        }

        if (!reorder)
        {
            AllColorsToGray();
            NextClassyShowInBlackColor();
        }
    }

    void AllColorsToGray()
    {
        for (int i = 0; i < classyImg.Length; i++)
        {
            classyImg[i].color = colorGray;
        }
    }

    void NextClassyShowInBlackColor()
    {
        if (classyfiedBtns.Count < 3)
        {
            classyImg[classyfiedBtns.Count].color = colorBlack;
        }
    }

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
        //GameInfoChoose.chosenButtons = classyfiedBtns;
        SetPositions();

        GameSettings.MyDebug("Chosen btns: " +GameInfoChoose.chosenButtons);

        GameInfoChoose.timeLeft = 0;
        SceneSwitcher.LoadScene2(GameSettings.ROUND_SCORE_CHOOSE);
    }

    void SetPositions()
    {
        for (int i = 0; i < classyfiedBtns.Count; i++)
        {
            for (int j = 0; j < GameInfoChoose.chosenButtons.Count; j++)
            {
                if (classyfiedBtns[i].word.Equals(GameInfoChoose.chosenButtons[j].word))
                {
                    GameSettings.MyDebug(classyfiedBtns[i].word + " / " + i);
                    GameInfoChoose.chosenButtons[j].group = i;
                }
            }
        }

        GameInfoChoose.classyfiedBtns = classyfiedBtns;
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            SceneSwitcher.LoadScene2Back2(GameSettings.COMPETITIVE_MODE_SELECTION_MENU);
        }
    }
}
